using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace Image_Services_SOE
{
    class ExportImage
    {

        private static Image SaveCurrentToImage(IMap pMap, Size outRect, IEnvelope pEnvelope)
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

        public static string ExportLayerImage(IRaster pRaster, string[] bbox, string[] size,string outputUrl,string serveroutDir)
        {
            //判断栅格文件是否已经存在，存在删除，原因是如果存在，不删除的话，保存图片出错

            //创建rasterlayer
            IRasterLayer pRasterLayer = new RasterLayerClass();       

        

            //获取mapServer中所有的图层           
            IMap pMap = new MapClass();
            pRasterLayer.CreateFromRaster(pRaster);
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
            //outputUrl="http://localhost:6080/arcgis/rest/directories/arcgisoutput/imageserver/miyunspot_ImageServer/"

    
            // serveroutDir="D:\arcgisserver\directories\arcgisoutput\imageserver\miyunspot_ImageServer";
            string url = outputUrl + imageName;
            string path = System.IO.Path.Combine(serveroutDir, imageName);
            pImage.Save(path);
            return url;

        }

        public static string ExportImageServer(ImageServer imageServer)
        {
            IGeoImageDescription2 geoImageDesc = new GeoImageDescriptionClass();
            geoImageDesc.Width = 800;
            geoImageDesc.Height = 600;
            geoImageDesc.Extent = imageServer.ServiceInfo.Extent;
           // geoImageDesc.RenderingRule = rule;

            //Export an image using service's color map.
            IImageType imageType = new ImageTypeClass();
            imageType.Format = esriImageFormat.esriImageJPGPNG;
            imageType.ReturnType = esriImageReturnType.esriImageReturnURL;
            IMapImage mapImage = ((IImageServer2)imageServer).ExportMapImage(geoImageDesc, imageType);
            return mapImage.URL;
        }
        public  static string CreateJPEGFromActiveView(IRaster pRaster,IEnvelope pEnv,string outurl, System.String pathFileName)
        {
            //创建rasterlayer
            IRasterLayer pRasterLayer = new RasterLayerClass();



            //获取mapServer中所有的图层           
            IMap pMap = new MapClass();
            pRasterLayer.CreateFromRaster(pRaster);
            pMap.AddLayer(pRasterLayer as ILayer);

            IActiveView activeView = pMap as IActiveView;
            activeView.Extent = pEnv;
            //parameter check
            if (activeView == null)
            {
                return null;
            }
            string imageName = System.DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") + ".png";
            pathFileName = System.IO.Path.Combine(pathFileName, imageName);

            ESRI.ArcGIS.Output.IExport export = new ESRI.ArcGIS.Output.ExportPNGClass();
            export.ExportFileName = pathFileName;

            // Microsoft Windows default DPI resolution

            ESRI.ArcGIS.esriSystem.tagRECT exportRECT = new ESRI.ArcGIS.esriSystem.tagRECT();
            exportRECT.top = 0;
            exportRECT.left = 0;
            exportRECT.right = 800;
            exportRECT.bottom = 600;
            ESRI.ArcGIS.Geometry.IEnvelope envelope = new ESRI.ArcGIS.Geometry.EnvelopeClass();
            envelope.PutCoords(exportRECT.top,exportRECT.left,exportRECT.right,exportRECT.bottom);
            export.PixelBounds = envelope;
            System.Int32 hDC = export.StartExporting();
            activeView.Output(hDC, (System.Int16)export.Resolution, ref exportRECT, null, null);

            // Finish writing the export file and cleanup any intermediate files
            export.FinishExporting();
            export.Cleanup();

            return outurl + "/" + imageName;

        }


    }
}
