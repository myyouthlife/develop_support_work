using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SpatialAnalyst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Image_Services_SOE
{
    class Classify
    {

        public static IRaster ClassifyMethod(IRasterDataset pRasterDataset, int classCount, string gsgPath)
        {


             IGeoDataset pGeoDatasetResult=null;

            if (CreateSignaturefile(pRasterDataset, classCount, gsgPath))
            {
                IGeoDataset pGeo = pRasterDataset as IGeoDataset;
                IMultivariateOp pMultivarateOp = new RasterMultivariateOpClass();
                 pGeoDatasetResult = pMultivarateOp.MLClassify(pGeo, gsgPath, false, esriGeoAnalysisAPrioriEnum.esriGeoAnalysisAPrioriEqual, Type.Missing, Type.Missing);            
            }
            return (IRaster)pGeoDatasetResult;


        }

        public static bool CreateSignaturefile(IRasterDataset pRasterDataset, int classCount, string gsgPath)
        {
            

            IGeoDataset pGeo = pRasterDataset as IGeoDataset;
            IMultivariateOp pMultivarateOp = new RasterMultivariateOpClass();
            // string gsgPath = "e:\\222.gsg";
            pMultivarateOp.IsoCluster(pGeo, gsgPath, classCount);
            return true;
        
        }

        public static string ApplyMLClassifyFunction(IImageServer imageServer, IRaster pRaster, string signaturfile)
        {
            //Define a  function.
            IRasterFunction hillshadeFunction = new MLClassifyFunctionClass();          

            IRasterFunctionArguments functionArgument = new MLClassifyFunctionArgumentsClass();
            functionArgument.PutValue("Raster", pRaster);
            functionArgument.PutValue("SignatureFile", signaturfile);

            //Attach the function to a rendering rule.
            IRenderingRule renderRule = new RenderingRuleClass();
            renderRule.Function = hillshadeFunction;
            renderRule.Arguments = functionArgument;
            renderRule.VariableName = "DEM";

            //Define the image description.
            IGeoImageDescription geoImageDesc = new GeoImageDescriptionClass();
            geoImageDesc.Compression = "LZ77";
            geoImageDesc.Extent = imageServer.ServiceInfo.Extent;
            geoImageDesc.Width = 800;
            geoImageDesc.Height = 600;
            geoImageDesc.Interpolation = rstResamplingTypes.RSP_BilinearInterpolation;
            IGeoImageDescription2 geoImageDesc2 = (IGeoImageDescription2)geoImageDesc;
            
            geoImageDesc2.RenderingRule = renderRule;

            //Define the return image type.
            IImageType imgType = new ImageTypeClass();
            imgType.Format = esriImageFormat.esriImagePNG;
            imgType.ReturnType = esriImageReturnType.esriImageReturnURL;

            //Export the image.
            IImageResult imgResult = imageServer.ExportImage(geoImageDesc2, imgType);
            return imgResult.URL;
        }

    }
}
