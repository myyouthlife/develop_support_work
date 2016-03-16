using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Specialized;

using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SOESupport;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.SpatialAnalyst;


//TODO: sign the project (project properties > signing tab > sign the assembly)
//      this is strongly suggested if the dll will be registered using regasm.exe <your>.dll /codebase


namespace Image_Services_SOE
{
    [ComVisible(true)]
    [Guid("627507c4-bbc6-4d2c-93f2-8bda7ed040e0")]
    [ClassInterface(ClassInterfaceType.None)]
    [ServerObjectExtension("ImageServer",//use "MapServer" if SOE extends a Map service and "ImageServer" if it extends an Image service.
        AllCapabilities = "",
        DefaultCapabilities = "",
        Description = "",
        DisplayName = "Image_Services_SOE",
        Properties = "",
        SupportsREST = true,
        SupportsSOAP = false)]
    public class Image_Services_SOE : IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        private string _soename;

        private IPropertySet _configProps;
        private IServerObjectHelper _serverObjectHelper;
        private ServerLogger _logger;
        private IRESTRequestHandler reqHandler;

        private IFeatureClass _mosaicCatalog;
        private bool _supportRasterItemAccess;

        IImageServerInit3 imageServerInit = null;
        ImageServer pImageSever = null;

        public Image_Services_SOE()
        {
            _soename = this.GetType().Name;
            _logger = new ServerLogger();
            reqHandler = new SoeRestImpl(_soename, CreateRestSchema()) as IRESTRequestHandler;
        }

        #region IServerObjectExtension Members

        public void Init(IServerObjectHelper pSOH)
        {
            _serverObjectHelper = pSOH;
            pImageSever = _serverObjectHelper.ServerObject as ImageServer;

        }

        public void Shutdown()
        {
        }

        #endregion

        #region IObjectConstruct Members

        public void Construct(IPropertySet props)
        {
            _configProps = props;
            imageServerInit = (IImageServerInit3)_serverObjectHelper.ServerObject;



            IName mosaicName = imageServerInit.ImageDataSourceName;
            if (mosaicName is IMosaicDatasetName)
            {
                IMosaicDataset md = (IMosaicDataset)mosaicName.Open();
                _mosaicCatalog = md.Catalog;
                _supportRasterItemAccess = true;
            }
            else

                _supportRasterItemAccess = false;
        }

        #endregion

        #region IRESTRequestHandler Members

        public string GetSchema()
        {
            return reqHandler.GetSchema();
        }

        public byte[] HandleRESTRequest(string Capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            return reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        #endregion

        private RestResource CreateRestSchema()
        {
            RestResource rootRes = new RestResource(_soename, true, RootResHandler);

            RestOperation getRasterStaticticsOper = new RestOperation("GetRasterStatistics",
                                                      new string[] { "objectID" },
                                                      new string[] { "json" },
                                                      GetRasterStatisticsOperHandler);

            RestOperation doClassify = new RestOperation("DoClassify",
                new string[] { "objectID", "classnumber" },
                new string[] { "html", "json" },
                DoClassifyHandler);

            RestOperation ExcuteFunc = new RestOperation("ExcuteFunc",
               new string[] { "objectID", "classnumber" },
               new string[] { "html", "json" },
               ExcuteFuncHandler);

            rootRes.operations.Add(getRasterStaticticsOper);
            rootRes.operations.Add(doClassify);
            rootRes.operations.Add(ExcuteFunc);

            return rootRes;
        }

        /*
         *通过调用影像服务的形式实现最大最大似然分类
         */
        private byte[] ExcuteFuncHandler(NameValueCollection boundVariables, JsonObject operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request received");
            if (!_supportRasterItemAccess)
                throw new ArgumentException("The image service does not have a catalog and does not support this operation");
            responseProperties = null;

            long? objectID;
            long? classCount;
            //case insensitive
            bool found = operationInput.TryGetAsLong("objectID", out objectID);
            if (!found || (objectID == null))
                throw new ArgumentNullException("ObjectID");

            found = operationInput.TryGetAsLong("classnumber", out classCount);
            if (!found || (objectID == null))
                throw new ArgumentNullException("classnumber");

            IRasterCatalogItem rasterCatlogItem = null;
            try
            {
                rasterCatlogItem = _mosaicCatalog.GetFeature((int)objectID) as IRasterCatalogItem;
                if (rasterCatlogItem == null)
                {
                    _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request finished with exception");
                    throw new ArgumentException("The input ObjectID does not exist");
                }
            }
            catch
            {
                _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request finished with exception");
                throw new ArgumentException("The input ObjectID does not exist");
            }
            JsonObject result = new JsonObject();
            string outputurl = "";
            try
            {
                // rasterBandsCol = (IRasterBandCollection)rasterCatlogItem.RasterDataset;
                IRasterDataset pRasterDataSet = rasterCatlogItem.RasterDataset;

                string gsgname = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + ".gsg";
                string gsgpath = System.IO.Path.Combine(@"d:\", gsgname);

                bool bcreatesignaturefile = Classify.CreateSignaturefile(pRasterDataSet, Convert.ToInt32(classCount), gsgpath);
                if (bcreatesignaturefile)
                {
                    IRaster pRaster = pRasterDataSet.CreateDefaultRaster();
                    string imagePath = Classify.ApplyMLClassifyFunction(pImageSever, pRaster, gsgpath);

                    outputurl = "http://localhost:6080/arcgis/" + imagePath;
                }
            }

            catch
            {

            }
            result.AddString("url", outputurl);
            return Encoding.UTF8.GetBytes(result.ToJson());
        }
        /*
         *该方法通过纯粹的Arcobject的方式实现最大似然分类        *
         * 
         * *
         */


        private byte[] DoClassifyHandler(NameValueCollection boundVariables, JsonObject operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request received");
            if (!_supportRasterItemAccess)
                throw new ArgumentException("The image service does not have a catalog and does not support this operation");
            responseProperties = null;

            long? objectID;
            long? classCount;
            //case insensitive
            bool found = operationInput.TryGetAsLong("objectID", out objectID);
            if (!found || (objectID == null))
                throw new ArgumentNullException("ObjectID");

            found = operationInput.TryGetAsLong("classnumber", out classCount);
            if (!found || (objectID == null))
                throw new ArgumentNullException("classnumber");
            IRasterCatalogItem rasterCatlogItem = null;
            try
            {
                rasterCatlogItem = _mosaicCatalog.GetFeature((int)objectID) as IRasterCatalogItem;
                if (rasterCatlogItem == null)
                {
                    _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request finished with exception");
                    throw new ArgumentException("The input ObjectID does not exist");
                }
            }
            catch
            {
                _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request finished with exception");
                throw new ArgumentException("The input ObjectID does not exist");
            }
            JsonObject result = new JsonObject();
            string outputurl = "";
            try
            {

                IRasterDataset pRasterDataSet = rasterCatlogItem.RasterDataset;


                IGeoDataset pGeo = pRasterDataSet as IGeoDataset;

                string inPath = @"D:\arcgisserver\directories\arcgisoutput\imageserver\test2_ImageServer";
                string gsgname = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + ".gsg";
                string gsgPath = System.IO.Path.Combine(inPath, gsgname);

                bool bcreatesignaturefile = Classify.CreateSignaturefile(pRasterDataSet, Convert.ToInt32(classCount), gsgPath);
                if (bcreatesignaturefile)
                {
                    IMultivariateOp pMultivarateOp = new RasterMultivariateOpClass();
                    IGeoDataset pGeoDatasetResult = pMultivarateOp.MLClassify(pGeo, gsgPath, false, esriGeoAnalysisAPrioriEnum.esriGeoAnalysisAPrioriEqual, Type.Missing, Type.Missing);

                    IEnvelope pEnvelp = new EnvelopeClass();

                    string outurl = "http://localhost:6080/arcgis/rest/directories/arcgisoutput/imageserver/test2_ImageServer/";
                    pEnvelp.PutCoords(116.56075474, 40.29407147, 116.63105347, 40.34514666);
                    // string dd = ExportImage.ExportLayerImage((IRaster)pGeoDatasetResult, bbox, new string[] { "400", "400" }, outurl, fileDir);
                    outputurl = ExportImage.CreateJPEGFromActiveView((IRaster)pGeoDatasetResult, pEnvelp, outurl, inPath);
                }
            }

            catch
            {

            }
            result.AddString("url", outputurl);
            return Encoding.UTF8.GetBytes(result.ToJson());

        }
        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            JsonObject result = new JsonObject();
            result.AddString("Description", "Get raster item statistics in a mosaic dataset");
            result.AddBoolean("SupportRasterItemAccess", _supportRasterItemAccess);

            return Encoding.UTF8.GetBytes(result.ToJson());
        }


        /*
         *该函数用来实现对影像服务中的影像进行统计，统计各波段值中的最大最小值
         * 
         * 
         */
        private byte[] GetRasterStatisticsOperHandler(NameValueCollection boundVariables,
                                                JsonObject operationInput,
                                                    string outputFormat,
                                                    string requestProperties,
                                                out string responseProperties)
        {
            _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request received");
            if (!_supportRasterItemAccess)
                throw new ArgumentException("The image service does not have a catalog and does not support this operation");
            responseProperties = null;

            long? objectID;
            //case insensitive
            bool found = operationInput.TryGetAsLong("objectID", out objectID);
            if (!found || (objectID == null))
                throw new ArgumentNullException("ObjectID");
            IRasterCatalogItem rasterCatlogItem = null;
            IRasterBandCollection rasterBandsCol = null;
            IRasterStatistics statistics = null;
            try
            {
                //获取栅格目录
                rasterCatlogItem = _mosaicCatalog.GetFeature((int)objectID) as IRasterCatalogItem;
                if (rasterCatlogItem == null)
                {
                    _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request finished with exception");
                    throw new ArgumentException("The input ObjectID does not exist");
                }
            }
            catch
            {
                _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request finished with exception");
                throw new ArgumentException("The input ObjectID does not exist");
            }
            JsonObject result = new JsonObject();
            try
            {
                rasterBandsCol = (IRasterBandCollection)rasterCatlogItem.RasterDataset;
                List<object> maxvalues = new List<object>();
                List<object> minvalues = new List<object>();
                List<object> standarddeviationvalues = new List<object>();
                List<object> meanvalues = new List<object>();
                for (int i = 0; i < rasterBandsCol.Count; i++)
                {
                    statistics = rasterBandsCol.Item(i).Statistics;
                    maxvalues.Add(statistics.Maximum);
                    minvalues.Add(statistics.Minimum);
                    standarddeviationvalues.Add(statistics.StandardDeviation);
                    meanvalues.Add(statistics.Mean);
                    Marshal.ReleaseComObject(statistics);
                }

                //结果序列号
                result.AddArray("maxValues", maxvalues.ToArray());
                result.AddArray("minValues", minvalues.ToArray());
                result.AddArray("meanValues", meanvalues.ToArray());
                result.AddArray("stdvValues", standarddeviationvalues.ToArray());
            }
            catch
            {
                _logger.LogMessage(ServerLogger.msgType.infoDetailed, "GetRasterStatistics", 8000, "request completed. statistics does not exist");
            }
            finally
            {
                if (rasterBandsCol != null)
                    Marshal.ReleaseComObject(rasterBandsCol);
                if (rasterCatlogItem != null)
                    Marshal.ReleaseComObject(rasterCatlogItem);
            }
            _logger.LogMessage(ServerLogger.msgType.infoDetailed, _soename + ".GetRasterStatistics", 8000, "request completed successfully");
            return Encoding.UTF8.GetBytes(result.ToJson());
        }




    }
}
