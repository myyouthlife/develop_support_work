using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;

namespace TestProjectofSL
{
    public partial class label_featurelayer : UserControl
    {
        GeometryService geometryService = null;
        List<Graphic> pListGrapic = null;
        public label_featurelayer()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            geometryService = new GeometryService("http://localhost:6080/arcgis/rest/services/Utilities/Geometry/GeometryServer");
            geometryService.SimplifyCompleted += GeometryService_SimplifyCompleted;
            geometryService.LabelPointsCompleted += GeometryService_LabelPointsCompleted;
            geometryService.Failed += GeometryService_Failed;

            FeatureLayer mylayer = myMap.Layers["myFeatuerLayer"] as FeatureLayer;
            GraphicCollection gs = mylayer.Graphics;
            GraphicsLayer labelPointGraphicLayer = new GraphicsLayer();


             pListGrapic = new List<Graphic>();

            for (int i = 0; i < gs.Count; i++)
            {
                pListGrapic.Add(gs[i]);
                
            }
            geometryService.SimplifyAsync(pListGrapic);


               
        }

        private void GeometryService_Failed(object sender, TaskFailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GeometryService_LabelPointsCompleted(object sender, GraphicsEventArgs e)
        {
            GraphicsLayer pGrapicsLayer = new GraphicsLayer();
             int i=0;
            foreach (var graphic in e.Results)
            {
                Graphic newGraphicPoint = new Graphic();
               
                newGraphicPoint.Geometry = graphic.Geometry;
                Brush pBrush = new SolidColorBrush(Color.FromArgb(100, 0, 0, 255));
                newGraphicPoint.Symbol = new TextSymbol() { Text = pListGrapic[i].Attributes["NAME"].ToString(), Foreground = pBrush, FontSize = 8 };
                i++;
                pGrapicsLayer.Graphics.Add(newGraphicPoint);
            }
            myMap.Layers.Add(pGrapicsLayer);
          

        }

        private void GeometryService_SimplifyCompleted(object sender, GraphicsEventArgs e)
        {
            geometryService.LabelPointsAsync(e.Results);
        }
    }
}
