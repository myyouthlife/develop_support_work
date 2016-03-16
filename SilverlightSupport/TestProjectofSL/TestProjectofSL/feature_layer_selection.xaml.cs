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
    public partial class feature_layer_selection : UserControl
    {
        private static ESRI.ArcGIS.Client.Projection.WebMercator mercator = new ESRI.ArcGIS.Client.Projection.WebMercator();

        ESRI.ArcGIS.Client.Geometry.Envelope initialExtent = new ESRI.ArcGIS.Client.Geometry.Envelope(
            mercator.FromGeographic(new ESRI.ArcGIS.Client.Geometry.MapPoint(-117.190346717, 34.0514888762)) as ESRI.ArcGIS.Client.Geometry.MapPoint,
            mercator.FromGeographic(new ESRI.ArcGIS.Client.Geometry.MapPoint(-117.160305976, 34.072946548)) as ESRI.ArcGIS.Client.Geometry.MapPoint)
        {
            SpatialReference = new ESRI.ArcGIS.Client.Geometry.SpatialReference(102100)
        };



        public feature_layer_selection()
        {
            InitializeComponent();
            MyMap.Extent = initialExtent;

        }

        private void Editor_EditCompleted(object sender, ESRI.ArcGIS.Client.Editor.EditEventArgs e)
        {
            List<string> name = new List<string>();
            var editor = sender as Editor;
            if(e.Action==Editor.EditAction.Select)
            {
                foreach (var edit in e.Edits)
                {
                    if (edit.Graphic != null && edit.Graphic.Selected)
                    {
                        var layer = edit.Layer as FeatureLayer;
                        name.Add(layer.ID);
                    }
                    
                }
            
            }


        }
    }
}
