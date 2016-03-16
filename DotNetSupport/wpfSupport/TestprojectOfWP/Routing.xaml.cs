using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestprojectOfWP
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
         GraphicsLayer stopsGraphicsLayer;
        GraphicsLayer routeGraphicsLayer;
        RouteTask routeTask;
        public UserControl1()
        {
            InitializeComponent();


            stopsGraphicsLayer = MyMap.Layers["MyStopsGraphicsLayer"] as GraphicsLayer;
            routeGraphicsLayer = MyMap.Layers["MyRouteGraphicsLayer"] as GraphicsLayer;
            //routeTask = new RouteTask("http://192.168.220.124:6080/arcgis/rest/services/MyMapService1/NAServer/Route");
            routeTask = new RouteTask("  http://sampleserver6.arcgisonline.com/arcgis/rest/services/NetworkAnalysis/SanDiego/NAServer/Route");
      
            routeTask.SolveCompleted += MyRouteTask_SolveCompleted;
            routeTask.Failed += MyRouteTask_Failed;

        }

        private void MyRouteTask_SolveCompleted(object sender, RouteEventArgs e)
        {
            routeGraphicsLayer.Graphics.Clear();

            RouteResult routeResult = e.RouteResults[0];

            Graphic lastRoute = routeResult.Route;

            decimal totalTime = (decimal)lastRoute.Attributes["Total_TravelTime"];
            TotalTimeTextBlock.Text = string.Format("Total time: {0} minutes", totalTime.ToString("#0.000"));

            routeGraphicsLayer.Graphics.Add(lastRoute);


        DirectionsFeatureSet _directionsFeatureSet = routeResult.Directions;


        }

        private void MyRouteTask_Failed(object sender, TaskFailedEventArgs e)
        {
            string errorMessage = "Routing error: ";
            errorMessage += e.Error.Message;
            foreach (string detail in (e.Error as ServiceException).Details)
                errorMessage += "," + detail;

            MessageBox.Show(errorMessage);

            stopsGraphicsLayer.Graphics.RemoveAt(stopsGraphicsLayer.Graphics.Count - 1);

        }

        private void MyMap_MouseClick(object sender, ESRI.ArcGIS.Client.Map.MouseEventArgs e)
        {
            Graphic stop = new Graphic() { Geometry = e.MapPoint };
            stopsGraphicsLayer.Graphics.Add(stop);
            if (stopsGraphicsLayer.Graphics.Count > 1)
            {
                if (routeTask.IsBusy)
                {
                    routeTask.CancelAsync();
                    stopsGraphicsLayer.Graphics.RemoveAt(stopsGraphicsLayer.Graphics.Count - 1);
                }
                routeTask.SolveAsync(new RouteParameters()
                {
                    Stops = stopsGraphicsLayer,
                    UseTimeWindows = false,
                    OutSpatialReference = MyMap.SpatialReference
                });
            }

        }
    }
}
