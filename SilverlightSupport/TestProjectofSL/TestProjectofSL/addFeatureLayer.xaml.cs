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
using ESRI.ArcGIS.Client;

namespace TestProjectofSL
{
    public partial class addFeatureLayer : UserControl
    {
        public addFeatureLayer()
        {
            InitializeComponent();

            FeatureLayer pFeaturelayer = new FeatureLayer();
            pFeaturelayer.Url = "http://localhost:6080/arcgis/rest/services/wind3/MapServer/0";
            myMap.Layers.Add(pFeaturelayer);

            
        }
    }
}
