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
    public partial class addToken : UserControl
    {
        ArcGISDynamicMapServiceLayer layer = null;
        public addToken()
        {
            InitializeComponent();
             layer = new ArcGISDynamicMapServiceLayer()
            {
                Url = "http://localhost:6080/arcgis/rest/services/MyMapService/MapServer?ton==sdfsdf...export=xxxx"
             
               
                
               
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string tokenurl =
                string.Format("https://localhost:6080/arcgis/tokens?request=getToken&username={0}&password={1}&timeout={2}",
                "arcgis", "Super123", "3600");

            Uri testUri = new Uri("http://localhost:6080/arcgis/rest/services/MyMapService/MapServer?token=");
          
            WebClient client = new WebClient();
            client.Headers["referer"] = "http://localhost:6080";
            client.DownloadStringCompleted += (sender2, args) =>
                {
                    

                    
                };
            client.DownloadStringAsync(testUri);
        }
      

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ArcGISDynamicMapServiceLayer player = new ArcGISDynamicMapServiceLayer();
          

        }

        void downloader_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ESRI.ArcGIS.Client.ArcGISDynamicMapServiceLayer pDynamicMaplayer = new ArcGISDynamicMapServiceLayer();
            pDynamicMaplayer.Url = "http://192.168.100.82:6080/arcgis/rest/services/MyMapService/MapServer";
             pDynamicMaplayer.ProxyURL = "http://192.168.100.82/proxy.ashx";
            //pDynamicMaplayer.Token = "VQTzrWZ6RhESBvab-RORbrA6iTQ5OtLBGnsyLScRdG6Q6saIKUTuvbas_L8LXR-6";
            myMap.Layers.Add(pDynamicMaplayer);


        }
    }
}
