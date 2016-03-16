using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace slapiTest
{
    public partial class SilverlightControl2 : UserControl
    {
        public SilverlightControl2()
        {
            InitializeComponent();


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GetImageLayer()
        {
            //WebClient pWebClient = new WebClient();
            ////  pWebClient.OpenReadCompleted += pWebClient_OpenReadCompleted;
            ////pWebClient.DownloadStringCompleted += PWebClient_DownloadStringCompleted;
            //pWebClient.OpenReadCompleted += PWebClient_OpenReadCompleted;
            //string soeUrl = "http://localhost/test.jpg";
            //string soeUrl2 = "http://localhost:6080/arcgis/rest/services/test3/neimeng/MapServer/export?bbox=6362144.533630894%2C2890953.0248808847%2C1.801441783817751E7%2C8955215.153405145&bboxSR=&layers=&layerDefs=&size=800%2C800&imageSR=&format=png&transparent=false&dpi=&time=&layerTimeOptions=&dynamicLayers=&gdbVersion=&mapScale=&f=image";

            ////pWebClient.DownloadStringAsync(new Uri(soeUrl));
            //pWebClient.OpenReadAsync(new Uri(soeUrl));

            ElementLayer myElementLayer = new ElementLayer();
            BitmapImage bi = new BitmapImage(new Uri("http://localhost/test.jpg"));
            //bi.SetSource(e.Result as Stream);
            Image img = new Image();
            img.Source = bi;



            //string wld = imageUrl.Remove(imageUrl.LastIndexOf(".")) + ".wld";
            string envJson = SetB(bi.PixelWidth, bi.PixelHeight);
            ElementLayer.SetEnvelope(img, (ESRI.ArcGIS.Client.Geometry.Geometry.FromJson(envJson).Extent) as Envelope);


            myElementLayer.Children.Add(img);
            MyMap.Layers.Add(myElementLayer);
        }

        private void PWebClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            ElementLayer myElementLayer = new ElementLayer();
            BitmapImage bi = new BitmapImage(new Uri("http://localhost/test.jpg"));
            //bi.SetSource(e.Result as Stream);
            Image img = new Image();
            img.Source = bi;



            //string wld = imageUrl.Remove(imageUrl.LastIndexOf(".")) + ".wld";
            string envJson = SetB(bi.PixelWidth, bi.PixelHeight);
            ElementLayer.SetEnvelope(img, (ESRI.ArcGIS.Client.Geometry.Geometry.FromJson(envJson).Extent) as Envelope);


            myElementLayer.Children.Add(img);
            MyMap.Layers.Add(myElementLayer);
            //  MyMap.ZoomTo((img.GetValue(ElementLayer.EnvelopeProperty) as Envelope).Expand(1));
        }

        private void PWebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            byte[] array = Encoding.UTF8.GetBytes(e.Result);
            MemoryStream stream = new MemoryStream(array);            //convert stream 2 string      


            DataContractJsonSerializer serializer =
       new DataContractJsonSerializer(typeof(MyClass));
            MyClass user = (MyClass)serializer.ReadObject(stream);




            //    resulturl = @"http://localhost:6080/arcgis/rest/directories/arcgisoutput/"+tmparry[tmparry.Length-1];
            //   MessageBox.1Show(resulturl.ToString());
            ESRI.ArcGIS.Client.ElementLayer pElmentLayer = MyMap.Layers["myElementLayer"] as ESRI.ArcGIS.Client.ElementLayer;
            Image outputimage = pElmentLayer.Children[0] as Image;
            outputimage.Source = new BitmapImage(new Uri(user.resulturl));


        }


        public class MyClass
        {
            public string resulturl { get; set; }
        }

        private void ArcGISDynamicMapServiceLayer_Initialized(object sender, EventArgs e)
        {

            GetImageLayer();
        }

        private string SetB(double imageX, double iamgeY)
        {
            string envelopJson = string.Empty;
            double Y = iamgeY;
            double X = imageX;

            double A, B, C, D, E, F;
            A = Convert.ToDouble("13226.189903004106000");


            D = Convert.ToDouble("0.000000000000000");


            B = Convert.ToDouble("0.000000000000000");

            E = Convert.ToDouble("-13221.655123272267000");

            C = Convert.ToDouble("6368757.628582395600000");

            F = Convert.ToDouble("8948604.325843509300000");


            double x = A * X + B * Y + C;
            double y = D * X + E * Y + F;
            Envelope env = new Envelope(C, F, x, y);
            envelopJson = env.ToJson();
            //

            return envelopJson;
        }
    }

}

