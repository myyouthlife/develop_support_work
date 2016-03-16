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
    public partial class zoom_factor : UserControl
    {

        public zoom_factor()
        {
            InitializeComponent();
        }

        private void Map_ExtentChanged(object sender, ESRI.ArcGIS.Client.ExtentEventArgs e)
        {

            ESRI.ArcGIS.Client.Geometry.Envelope pEnvelopeOld = e.OldExtent;
            ESRI.ArcGIS.Client.Geometry.Envelope pEnvelopeNew = e.NewExtent;

            
        }
    }
}
