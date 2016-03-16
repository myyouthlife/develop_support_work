using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISSOERasterAccess
{
    class GetAndSetColorRample
    {

        public static string GetAndApplyColormap(IImageServer imageServer)
        {
            //Get the color map.
            IImageServiceInfo3 isInfo = imageServer.ServiceInfo as IImageServiceInfo3;
            IRasterColormap colormap = isInfo.Colormap;
            //Create the color map function using the color map.
            IRenderingRule rule = new RenderingRuleClass();
            ColormapFunction colormapfunction = new ColormapFunctionClass();
            IColormapFunctionArguments colormapargs = new ColormapFunctionArgumentsClass();
            colormapargs.Colormap = colormap;
            rule.Function = colormapfunction;
            rule.Arguments = (IRasterFunctionArguments)colormapargs;
            rule.VariableName = "Raster";    //Define export image request.
            IGeoImageDescription2 geoImageDesc = new GeoImageDescriptionClass();
            geoImageDesc.Width = 800; 
            geoImageDesc.Height = 600;
            geoImageDesc.Extent = isInfo.Extent; 
            geoImageDesc.RenderingRule = rule;
            
            //Export an image using service's color map.
            IImageType imageType = new ImageTypeClass();
            imageType.Format = esriImageFormat.esriImageJPGPNG;
            imageType.ReturnType = esriImageReturnType.esriImageReturnURL;
            IMapImage mapImage = ((IImageServer2)imageServer).ExportMapImage(geoImageDesc, imageType);
            return mapImage.URL;
        }
    }
}
