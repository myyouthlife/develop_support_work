using ESRI.ArcGIS.Client;
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
    public partial class featurelayer_mouseLeftdownTest : UserControl
    {
        public featurelayer_mouseLeftdownTest()
        {
            InitializeComponent();
        }

        private void FeatureLayer_MouseLeftButtonDown(object sender, ESRI.ArcGIS.Client.GraphicMouseButtonEventArgs e)
        {
            Graphic pGraphic = e.Graphic;

           
        }

        private void FeatureLayer_MouseLeftButtonUp(object sender, GraphicMouseButtonEventArgs e)
        {
            Graphic pGraphic = e.Graphic;
        }
    }
}
