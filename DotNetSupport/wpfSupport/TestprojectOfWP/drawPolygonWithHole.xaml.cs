using ESRI.ArcGIS.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;

namespace TestprojectOfWP
{
    /// <summary>
    /// Interaction logic for drawPolygonWithHole.xaml
    /// </summary>
    public partial class drawPolygonWithHole : Window
    {

        GraphicsLayer _myToJsonGraphicsLayer;
        GraphicsLayer _myFromJsonGraphicsLayer;

        public drawPolygonWithHole()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

//            string jsonPolygon = @"{""rings"":[[[110.039,-20.303],
//[132.539,-7.0137],
//[153.281,-13.923],
//[162.773,-35.174],
//[133.594,-43.180],
//[111.797,-36.032],
//[110.039,-20.303]]],
//""spatialReference"":{""wkid"":4326}}";
            string jsonPolygon = @"{""rings"":
            [
              [[-11214840,4858704],[-10520181,4853812],[-10510397,4149368],[-11219732,4144476],[-11214840,4858704]], 
              [[-11097433,4770648],[-10916430,4770648],[-10916430,4609213],[-10984918,4560294],[-11097433,4614105],[-11097433,4770648]], 
              [[-10779455,4472238],[-10622912,4349939],[-10750103,4242315],[-10833267,4296127],[-10779455,4472238]], 
              [[-11298004,4614105],[-11293112,4310803],[-11571954,4305911],[-11542602,4584753],[-11298004,4614105]] 
            ],
            ""spatialReference"":{""wkid"":102100}}";
            _myFromJsonGraphicsLayer = MyMap.Layers["MyFromJsonGraphicsLayer"] as GraphicsLayer;
            _myToJsonGraphicsLayer = MyMap.Layers["MyToJsonGraphicsLayer"] as GraphicsLayer;
            ESRI.ArcGIS.Client.Geometry.Geometry geometry = ESRI.ArcGIS.Client.Geometry.Polygon.FromJson(jsonPolygon);
           
            var pGrapic = new Graphic ();
            pGrapic.Geometry = geometry;
            pGrapic.Symbol = LayoutRoot.Resources["RedFillSymbol"] as SimpleFillSymbol;

            _myFromJsonGraphicsLayer.Graphics.Add(pGrapic);

           

        }
    }
}
