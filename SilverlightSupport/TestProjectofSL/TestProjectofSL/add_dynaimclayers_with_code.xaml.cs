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
using ESRI.ArcGIS.Client.Symbols;

namespace TestProjectofSL
{
    public partial class add_dynaimclayers_with_code : UserControl
    {
        /*
         
         用户反馈使用代码的形式加载动态图层没有办法成功。
         * 通过测试用户的问题是因为其在图层initialize使用CreateDynamicLayerInfosFromLayerInfos
         */
        public add_dynaimclayers_with_code()
        {
            InitializeComponent();
        }
       
        private void ArcGISDynamicMapServiceLayer_Initialized(object sender, EventArgs e)
        {

          
        }
        ArcGISDynamicMapServiceLayer pArcGISMapServicelayer = null;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            pArcGISMapServicelayer = new ArcGISDynamicMapServiceLayer();
            pArcGISMapServicelayer.Url = "http://localhost:6080/arcgis/rest/services/dynamicServices/ChinaTest/MapServer";
            pArcGISMapServicelayer.ID = "myLayer";
            myMap.Layers.Add(pArcGISMapServicelayer);           

            pArcGISMapServicelayer.Initialized += pArcGISMapServicelayer_Initialized;


        }

        void pArcGISMapServicelayer_Initialized(object sender, EventArgs e)
        {
            DynamicLayerInfoCollection pDynamicLayerInfoCollection = pArcGISMapServicelayer.CreateDynamicLayerInfosFromLayerInfos();

            //获取相对应的

            
          

            SimpleRenderer pSimpleRender = new SimpleRenderer();
        
            pSimpleRender.Symbol=new SimpleFillSymbol(){
            Fill=new SolidColorBrush(Color.FromArgb(255,100,255,100))
            };

            LayerDrawingOptions layeDrawingOption = new LayerDrawingOptions();

            layeDrawingOption.LayerID = 0;
            layeDrawingOption.Renderer = pSimpleRender;

            (myMap.Layers["myLayer"] as ArcGISDynamicMapServiceLayer).LayerDrawingOptions = new LayerDrawingOptionsCollection() { layeDrawingOption };

            (myMap.Layers["myLayer"] as ArcGISDynamicMapServiceLayer).Refresh();


        }
    }
}
