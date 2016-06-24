using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace elevationDislapy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
          //MySceneView.SpatialReferenceChanged += MySceneView_SpatialReferenceChanged;
        }
        private async void MySceneView_SpatialReferenceChanged(object sender, System.EventArgs e)
        {
            MySceneView.SpatialReferenceChanged -= MySceneView_SpatialReferenceChanged;

            try
            {
                // Set camera and navigate to it
                //var viewpoint = new Camera(
                //    new MapPoint(
                //        -122.41213238640989,
                //        37.78073901800655,
                //        80.497554714791477),
                //         53.719780233659428,
                //         73.16171159612496);
                var viewpoint = new Camera(
                  new MapPoint(
                     -8111229.209427015,
                      5523575.875123956,
                        5.497554714791477),
                       53.719780233659428,
                       73.16171159612496);


                await MySceneView.SetViewAsync(viewpoint, 1, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured while navigating to the target viewpoint",
                    "An error occured");
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
