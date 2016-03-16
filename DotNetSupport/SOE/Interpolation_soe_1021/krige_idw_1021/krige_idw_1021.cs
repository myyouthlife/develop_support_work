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
using ESRI.ArcGIS.GeoAnalyst;
using System.Drawing;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.SpatialAnalyst;


//TODO: sign the project (project properties > signing tab > sign the assembly)
//      this is strongly suggested if the dll will be registered using regasm.exe <your>.dll /codebase


namespace krige_idw_1021
{
    [ComVisible(true)]
    [Guid("92471be2-45bc-4625-aab4-a5376aa7a798")]
    [ClassInterface(ClassInterfaceType.None)]
    [ServerObjectExtension("MapServer",//use "MapServer" if SOE extends a Map service and "ImageServer" if it extends an Image service.
        AllCapabilities = "",
        DefaultCapabilities = "",
        Description = "",
        DisplayName = "krige_idw_1021",
        Properties = "",
        SupportsREST = true,
        SupportsSOAP = false)]
    public class krige_idw_1021 : IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        private string soe_name;
        private IPropertySet configProps;
        private IServerObjectHelper serverObjectHelper;
        private ServerLogger logger;
        private IRESTRequestHandler reqHandler;
        private IMapDescription pMapDescription;
        private IMapServer pMapServer;

        public krige_idw_1021()
        {
            soe_name = this.GetType().Name;
            logger = new ServerLogger();
            reqHandler = new SoeRestImpl(soe_name, CreateRestSchema()) as IRESTRequestHandler;
        }

        #region IServerObjectExtension Members

        public void Init(IServerObjectHelper pSOH)
        {
            System.Diagnostics.Debugger.Launch();
            serverObjectHelper = pSOH;
        }

        public void Shutdown()
        {
            soe_name = null;
            serverObjectHelper = null;
            logger = null;
        }

        #endregion

        #region IObjectConstruct Members

        public void Construct(IPropertySet props)
        {
            configProps = props;
            this.pMapServer = serverObjectHelper.ServerObject as IMapServer;
            this.pMapDescription = this.pMapServer.GetServerInfo(this.pMapServer.DefaultMapName).DefaultMapDescription;

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
            RestResource rootRes = new RestResource(soe_name, false, RootResHandler);

            RestOperation sampleOper = new RestOperation("DoInterpolate",
                                                      new string[] { "method", "cellsize", "LayerName", "FieldName", "bbox", "size" },
                                                      new string[] { "json" },
                                                      DoInteroplatHandler);

            rootRes.operations.Add(sampleOper);

            return rootRes;
        }

        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            JsonObject result = new JsonObject();

            result.AddString("名称", "高程插值SOE");
            result.AddString("描述", "通过改程序可以对制定图层中的制定字段采用Krige或者IDW方法进行插值，并将插值结果返回");
            result.AddString("方法", "通过输入方法名Krige或者IDW方法和栅格单元大小进行插值");
            return Encoding.UTF8.GetBytes(result.ToJson());
        }


        private byte[] DoInteroplatHandler(NameValueCollection boundVariables,
                                                  JsonObject operationInput,
                                                      string outputFormat,
                                                      string requestProperties,
                                                  out string responseProperties)
        {
            responseProperties = null;
            // 序列化插值方法
            string method;
            if (!operationInput.TryGetString("method", out method))
                throw new ArgumentNullException("invalid interpolation method", "method");
            // 序列化像元大小
            double? cellsize;
            if (!operationInput.TryGetAsDouble("cellsize", out cellsize) || !cellsize.HasValue)
                throw new ArgumentException("invalid cell sieze", "cellsize");


            //从地图服务中获取插值的featureClass
            string m_mapLayerNameToQuery = ""; // 查询图层名

            if (!operationInput.TryGetString("LayerName", out m_mapLayerNameToQuery) || string.IsNullOrEmpty(m_mapLayerNameToQuery))
                throw new ArgumentException("invalid LayerName", "LayerName");

            string m_mapFieldToQuery = "";//查询字段
            if (!operationInput.TryGetString("FieldName", out m_mapFieldToQuery) || string.IsNullOrEmpty(m_mapFieldToQuery))
                throw new ArgumentException("invalid LayerName", "LayerName");

            //

            string strbbox = "";//图片大小
            if (!operationInput.TryGetString("bbox", out strbbox) || string.IsNullOrEmpty(strbbox))
                throw new ArgumentException("invalid bbox", "bbox");
            string[] bbox_array = strbbox.Split(',');

            string strsize = "";


            if (!operationInput.TryGetString("size", out strsize) || string.IsNullOrEmpty(strsize))
                throw new ArgumentException("invalid size", "size");
            string[] size_array = strsize.Split(',');

            IFeatureClass _featureClass = GetFeatureClass(m_mapLayerNameToQuery, m_mapFieldToQuery);

            IRaster pRaster = DoInteroplate(_featureClass, m_mapFieldToQuery, cellsize, method);
            Resample(ref pRaster,Convert.ToDouble(cellsize)/12);

            ////将rasterlayer保存成图片导出
            string imageName = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + ".png";
            string url = @"D:\arcgisserver\directories\arcgisoutput\SOE\MyMapService_MapServer\" + imageName;
            string resultUrl = ExportLayer(pRaster, bbox_array, size_array);
            //  bool flage = CreatePNGromActiveView(pRaster, url, size_array, bbox_array);

            //   string resultUrl = "http://localhost:6080/arcgis/rest/directories/arcgisoutput/SOE/MyMapService_MapServer/" + imageName;
            JsonObject resultJsonObject = new JsonObject();
            resultJsonObject.AddString("resulturl", resultUrl);

            return Encoding.UTF8.GetBytes(resultJsonObject.ToJson());

        }
        private IFeatureClass GetFeatureClass(string m_mapLayerNameToQuery, string m_mapFieldToQuery)
        {
            IFeatureClass _featureClass = null;
            try
            {
                //获取数据              
                IMapServer3 mapServer = (IMapServer3)serverObjectHelper.ServerObject;
                string mapName = mapServer.DefaultMapName;
                IMapLayerInfo layerInfo;
                IMapLayerInfos layerInfos = mapServer.GetServerInfo(mapName).MapLayerInfos;
                // 获取查询图层id
                int layercount = layerInfos.Count;
                int layerIndex = 0;
                for (int i = 0; i < layercount; i++)
                {
                    layerInfo = layerInfos.get_Element(i);
                    if (layerInfo.Name == m_mapLayerNameToQuery)
                    {
                        layerIndex = i;
                        break;
                    }
                }

                IMapServerDataAccess dataAccess = (IMapServerDataAccess)mapServer;
                //获取要素          

                _featureClass = (IFeatureClass)dataAccess.GetDataSource(mapName, layerIndex);
                //确保获取到要素
                if (_featureClass == null)
                {
                    logger.LogMessage(ServerLogger.msgType.error, "GetFeatureClass", 8000, "SOE custom error: Layer name not found.");


                }
                // soe插值字段
                if (_featureClass.FindField(m_mapFieldToQuery) == -1)
                {
                    logger.LogMessage(ServerLogger.msgType.error, "GetFeatureClass", 8000, "SOE custom error: Field not found in layer.");
                }

            }
            catch
            {
                logger.LogMessage(ServerLogger.msgType.error, "GetFeatureClass", 80001, "can't not get the featureclass");
            }

            return _featureClass;

        }

        private IRaster DoInteroplate(IFeatureClass inputFclass, string inputField, object cellSize, string method)
        {
            if (inputFclass == null)
            {
                logger.LogMessage(ServerLogger.msgType.error, "SOE构建错误", 80004, "无效的的FeatureClass输入.");

            }
            if (string.IsNullOrEmpty(inputField))
            {
                logger.LogMessage(ServerLogger.msgType.error, "SOE构建错误", 80004, "无效的字段输入.");
            }
            if (cellSize == null)
            {

                logger.LogMessage(ServerLogger.msgType.error, "SOE构建错误", 80004, "无效的栅格单元大小.");

            }
            if (string.IsNullOrEmpty(method))
            {

                logger.LogMessage(ServerLogger.msgType.error, "SOE构建错误", 80004, "无效的插值方法.");

            }


            //生成IFeatureClassDescriptor
            IFeatureClassDescriptor pFcDescriptor = new FeatureClassDescriptorClass();
            pFcDescriptor.Create(inputFclass, null, inputField);
            //设置分析环境
            IInterpolationOp pInterpolationOp = new RasterInterpolationOpClass();
            IRasterAnalysisEnvironment pEnv = pInterpolationOp as IRasterAnalysisEnvironment;
            pEnv.Reset();
            //栅格单元大小
            pEnv.SetCellSize(esriRasterEnvSettingEnum.esriRasterEnvValue, ref cellSize);
            //定义搜索半径,可变搜索半径
            IRasterRadius pRadius = new RasterRadius();
            object missing = Type.Missing;
            pRadius.SetVariable(12, ref missing);
            IRaster pRaster = null;
            IGeoDataset pGeoDataset = null;

            //执行插值
            method = method.ToLower();
            switch (method)
            {
                case "krige":
                    pGeoDataset = pInterpolationOp.Krige((IGeoDataset)pFcDescriptor, esriGeoAnalysisSemiVariogramEnum.esriGeoAnalysisSphericalSemiVariogram,
                                   pRadius, false, ref missing);
                    pRaster = pGeoDataset as IRaster;
                    break;
                case "idw":
                    pRaster = pInterpolationOp.IDW(pFcDescriptor as IGeoDataset, 2, pRadius, ref missing) as IRaster;
                    break;
            }

            //   IRasterLayer pRasterLayer = new RasterLayerClass();
            //对插值结果进行重分类，采用等间距分为4类
          //  pRaster = DoReClassify(pRaster, 4);
            //pRasterLayer.CreateFromRaster(pRaster);
            //pRasterLayer.Name = "插值结果";
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pFcDescriptor);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pInterpolationOp);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pEnv);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pRadius);
         //   System.Runtime.InteropServices.Marshal.FinalReleaseComObject(pGeoDataset);

            return pRaster;

        }

        private IRaster DoReClassify(IRaster inputRaster, int pClassNo)
        {

            //获取栅格分类数组和频度数组
            object dataValues = null, dataCounts = null;
            GetRasterClass(inputRaster, out dataValues, out dataCounts);

            //获取栅格分类间隔数组
            IClassifyGEN pEqualIntervalClass = new EqualIntervalClass();
            pEqualIntervalClass.Classify(dataValues, dataCounts, ref pClassNo);
            double[] breaks = pEqualIntervalClass.ClassBreaks as double[];

            //设置新分类值
            INumberRemap pNemRemap = new NumberRemapClass();
            for (int i = 0; i < breaks.Length - 1; i++)
            {
                pNemRemap.MapRange(breaks[i], breaks[i + 1], i + 1);
            }
            IRemap pRemap = pNemRemap as IRemap;

            //设置环境
            IReclassOp pReclassOp = new RasterReclassOpClass();
            IGeoDataset pGeodataset = inputRaster as IGeoDataset;
            IRasterAnalysisEnvironment pEnv = pReclassOp as IRasterAnalysisEnvironment;
            object obj = Type.Missing; IEnvelope pRasterExt = new EnvelopeClass();
            //重分类      
            IRaster pRaster = pReclassOp.ReclassByRemap(pGeodataset, pRemap, false) as IRaster;




            System.Runtime.InteropServices.Marshal.ReleaseComObject(pEqualIntervalClass);

            System.Runtime.InteropServices.Marshal.ReleaseComObject(pNemRemap);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pRemap);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pReclassOp);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pGeodataset);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pRasterExt);
            return pRaster;
        }


        public void funColorForRaster_Classify(IRasterLayer pRasterLayer)
        {
            IRasterClassifyColorRampRenderer pRClassRend = new RasterClassifyColorRampRenderer() as IRasterClassifyColorRampRenderer;
            IRasterRenderer pRRend = pRClassRend as IRasterRenderer;

            IRaster pRaster = pRasterLayer.Raster;
            IRasterBandCollection pRBandCol = pRaster as IRasterBandCollection;
            IRasterBand pRBand = pRBandCol.Item(0);
            if (pRBand.Histogram == null)
            {
                pRBand.ComputeStatsAndHist();
            }
            pRRend.Raster = pRaster;
            pRClassRend.ClassCount = 9;
            pRRend.Update();

            IRgbColor pFromColor = new RgbColor() as IRgbColor;
            pFromColor.Red = 255;
            pFromColor.Green = 0;
            pFromColor.Blue = 0;
            IRgbColor pToColor = new RgbColor() as IRgbColor;
            pToColor.Red = 0;
            pToColor.Green = 0;
            pToColor.Blue = 255;

            IAlgorithmicColorRamp colorRamp = new AlgorithmicColorRamp() as IAlgorithmicColorRamp;
            colorRamp.Size = 10;
            colorRamp.FromColor = pFromColor;
            colorRamp.ToColor = pToColor;
            bool createColorRamp;

            colorRamp.CreateRamp(out createColorRamp);

            IFillSymbol fillSymbol = new SimpleFillSymbol() as IFillSymbol;
            for (int i = 0; i < pRClassRend.ClassCount; i++)
            {
                fillSymbol.Color = colorRamp.get_Color(i);
                pRClassRend.set_Symbol(i, fillSymbol as ISymbol);
                pRClassRend.set_Label(i, pRClassRend.get_Break(i).ToString("0.00"));
            }
            pRasterLayer.Renderer = pRRend;
        }
        private string ExportLayer(IRaster pRaster, string[] bbox, string[] size)
        {
            //判断栅格文件是否已经存在，存在删除，原因是如果存在，不删除的话，保存图片出错

            //创建rasterlayer
            IRasterLayer pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromRaster(pRaster);
            //对栅格图层添加渲染信息

            funColorForRaster_Classify(pRasterLayer);

            //获取mapServer中所有的图层           
            IMap pMap = new MapClass();
            //IRasterLayer pRasterLayer = GetRasterLayer(rasterLayerPath);
            pMap.AddLayer(pRasterLayer as ILayer);            //         
            //
            Size pSize = new Size();
            pSize.Width = Convert.ToInt32(size[0]);
            pSize.Height = Convert.ToInt32(size[1]);

            //
            IEnvelope pEnvelop = new EnvelopeClass();
            pEnvelop.PutCoords(Convert.ToDouble(bbox[0]), Convert.ToDouble(bbox[1]), Convert.ToDouble(bbox[2]), Convert.ToDouble(bbox[3]));
            Image pImage = SaveCurrentToImage(pMap, pSize, pEnvelop);
            string imageName = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + ".png";

            string url = "http://localhost:6080/arcgis/rest/directories/arcgisoutput/" + imageName;
            pImage.Save(@"D:\arcgisserver\directories\arcgisoutput\" + imageName);
            return url;

        }

        private void GetRasterClass(IRaster inputRaster, out object dataValues, out object dataCounts)
        {
            IRasterBandCollection pRasBandCol = inputRaster as IRasterBandCollection;
            IRasterBand pRsBand = pRasBandCol.Item(0);
            pRsBand.ComputeStatsAndHist();
            //IRasterBand中本无统计直方图，必须先进行ComputeStatsAndHist()
            IRasterStatistics pRasterStatistic = pRsBand.Statistics;

            double mMean = pRasterStatistic.Mean;
            double mStandsrdDeviation = pRasterStatistic.StandardDeviation;

            IRasterHistogram pRasterHistogram = pRsBand.Histogram;
            double[] dblValues;
            dblValues = pRasterHistogram.Counts as double[];
            int intValueCount = dblValues.GetUpperBound(0) + 1;
            double[] vValues = new double[intValueCount];

            double dMaxValue = pRasterStatistic.Maximum;
            double dMinValue = pRasterStatistic.Minimum;
            double BinInterval = Convert.ToDouble((dMaxValue - dMinValue) / intValueCount);
            for (int i = 0; i < intValueCount; i++)
            {
                vValues[i] = i * BinInterval + pRasterStatistic.Minimum;
            }

            dataValues = vValues as object;
            dataCounts = dblValues as object;
        }


        public Image SaveCurrentToImage(IMap pMap, Size outRect, IEnvelope pEnvelope)
        {
            //赋值  
            tagRECT rect = new tagRECT();
            rect.left = rect.top = 0;
            rect.right = outRect.Width;
            rect.bottom = outRect.Height;



            try
            {
                //转换成activeView，若为ILayout，则将Layout转换为IActiveView  
                IActiveView pActiveView = (IActiveView)pMap;
                // 创建图像,为24位色  
                Image image = new Bitmap(outRect.Width, outRect.Height); //, System.Drawing.Imaging.PixelFormat.Format24bppRgb);  
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(image);
                // 填充背景色(白色)  
                g.FillRectangle(Brushes.White, 0, 0, outRect.Width, outRect.Height);
                int dpi = (int)(outRect.Width / pEnvelope.Width);
                pActiveView.Output(g.GetHdc().ToInt32(), dpi, ref rect, pEnvelope, null);
                g.ReleaseHdc();
                return image;
            }


            catch (Exception excp)
            {
                return null;
            }
        }

        private void Resample(ref IRaster pRaster,double newsize)
        {
            IGeoDataset pGeoDataset = pRaster as IGeoDataset;
            IGeneralizeOp pGeneralizeOp = new RasterGeneralizeOpClass();
            pGeoDataset = pGeneralizeOp.Resample(pGeoDataset, newsize, esriGeoAnalysisResampleEnum.esriGeoAnalysisResampleNearest);
            pRaster = pGeoDataset as IRaster;
                      
        }
    }
}
