using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
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
    public partial class identify_layers_set : UserControl
    {
        public identify_layers_set()
        {
            InitializeComponent();
        }

        //用户反馈使用identify的时候，不能设置图层，该问题需要设置LayerIds
        private void Map_MouseClick(object sender, ESRI.ArcGIS.Client.Map.MouseEventArgs e)
        {

            ESRI.ArcGIS.Client.Geometry.MapPoint clickPoint = e.MapPoint;
            ESRI.ArcGIS.Client.Tasks.IdentifyParameters identifyParameter = new ESRI.ArcGIS.Client.Tasks.IdentifyParameters()
            {
                Geometry = clickPoint,
                MapExtent = myMap.Extent,
                Height = (int)myMap.ActualHeight,
                Width = (int)myMap.ActualWidth,
                LayerOption = LayerOption.all,
              //  LayerIds = {0,1},


                SpatialReference = myMap.SpatialReference
            };
            IdentifyTask identifyTask = new IdentifyTask("http://192.168.220.120:6080/arcgis/rest/services/test_identify/MapServer/identify");
            identifyTask.ExecuteCompleted+=identifyTask_ExecuteCompleted;

            identifyTask.ExecuteAsync(identifyParameter);

            ///
            var dynamicLayer=new ArcGISDynamicMapServiceLayer();
            dynamicLayer=myMap.Layers["mydynamiclayer"] as ArcGISDynamicMapServiceLayer;

           DynamicLayerInfoCollection pDynamicLayerCollection=dynamicLayer.CreateDynamicLayerInfosFromLayerInfos();
            int numLayers=dynamicLayer.Layers.Length;

            for (int i = 0; i < numLayers; i++)
            {

                bool visibleOrNot = dynamicLayer.GetLayerVisibility(i);
            }

            for (int i = 0; i < pDynamicLayerCollection.Count; i++)
            {

                DynamicLayerInfo pDynamicLayerInfo = pDynamicLayerCollection[i];
            }

        }

        private void identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {

        }
    }
}
