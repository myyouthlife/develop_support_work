using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Printing;
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
    public partial class printing_on_server : UserControl
    {
        PrintTask printTask;
        public printing_on_server()
        {
            InitializeComponent();
            printTask = new PrintTask("http://192.168.220.64:6080/arcgis/rest/services/Utilities/PrintingTools/GPServer/Export%20Web%20Map%20Task");
            printTask.DisableClientCaching = true;
            printTask.ExecuteCompleted += printTask_PrintCompleted;
            printTask.GetServiceInfoCompleted += printTask_GetServiceInfoCompleted;
            printTask.GetServiceInfoAsync();

        }


        private void printTask_GetServiceInfoCompleted(object sender, ServiceInfoEventArgs e)
        {
            LayoutTemplates.ItemsSource = e.ServiceInfo.LayoutTemplates;
            Formats.ItemsSource = e.ServiceInfo.Formats;
        }

        private void printTask_PrintCompleted(object sender, PrintEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Navigate(e.PrintResult.Url, "_blank");
        }

        private void ExportMap_Click(object sender, RoutedEventArgs e)
        {
            if (printTask == null || printTask.IsBusy) return;


            MapLayers pMapLayer = new MapLayers();

            ArcGISDynamicMapServiceLayer pDynamicLayer = new ArcGISDynamicMapServiceLayer();
            pDynamicLayer.Url = "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Petroleum/KSFields/MapServer";
            pMapLayer.add(0, pDynamicLayer);


            FeatureLayer pFeatureLayer = new FeatureLayer();
            pFeatureLayer.Url = "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/Petroleum/KSWells/MapServer/0";
            pMapLayer.add(1, pFeatureLayer);

           
            ArcGISTiledMapServiceLayer layer3 = new ArcGISTiledMapServiceLayer();
            layer3.Url = "http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer";

            pMapLayer.add(2, layer3);

            PrintParameters printParameters = new PrintParameters(pMapLayer,MyMap.Extent)
            {
                ExportOptions = new ExportOptions() { Dpi = 96, OutputSize = new Size(MyMap.ActualWidth, MyMap.ActualHeight) },
                LayoutTemplate = (string)LayoutTemplates.SelectedItem ?? string.Empty,
                Format = (string)Formats.SelectedItem,

            };
            printTask.ExecuteAsync(printParameters);

        }
    }

  public  class MapLayers : IEnumerable<Layer>
    {
        List<Layer> myList = new List<Layer>();

        public void add(int index, Layer layer)
        {
            myList.Insert(index, layer);
        }

        public IEnumerator<Layer> GetEnumerator()
        {
            return myList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

 
}

