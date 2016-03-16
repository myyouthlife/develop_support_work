using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Toolkit;
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

namespace TestProjectofSL
{
    public partial class custom_infor_window : UserControl
    {
        public custom_infor_window()
        {
            InitializeComponent();
        }

        private void MyInfoWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyInfoWindow.IsOpen = false;

        }

        private void MyMap_MouseClick(object sender, ESRI.ArcGIS.Client.Map.MouseEventArgs e)
        {

            FeatureLayer featureLayer = MyMap.Layers["MyFeatureLayer"] as FeatureLayer;
            System.Windows.Point screenPnt = MyMap.MapToScreen(e.MapPoint);

            // Account for difference between Map and application origin
            GeneralTransform generalTransform = MyMap.TransformToVisual(Application.Current.RootVisual);
            System.Windows.Point transformScreenPnt = generalTransform.Transform(screenPnt);

            IEnumerable<Graphic> selected =
                featureLayer.FindGraphicsInHostCoordinates(transformScreenPnt);

            foreach (Graphic g in selected)
            {

                MyInfoWindow.Anchor.X = e.MapPoint.X+1;
                MyInfoWindow.Anchor.X = e.MapPoint.Y + 1;
                MyInfoWindow.IsOpen = true;
                //Since a ContentTemplate is defined, Content will define the DataContext for the ContentTemplate
                MyInfoWindow.Content = g.Attributes;
                return;
            }

            InfoWindow window = new InfoWindow()
            {
                Anchor = e.MapPoint,
                Map = MyMap,
                IsOpen = true,
                Placement = InfoWindow.PlacementMode.Auto,
                ContentTemplate = LayoutRoot.Resources["LocationInfoWindowTemplate"] as System.Windows.DataTemplate,
                //Since a ContentTemplate is defined, Content will define the DataContext for the ContentTemplate
                Content = e.MapPoint
            };
            LayoutRoot.Children.Add(window);

        }
    }
}
