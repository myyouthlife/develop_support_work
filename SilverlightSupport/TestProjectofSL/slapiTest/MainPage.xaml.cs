using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Tasks;
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

namespace slapiTest
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }
        FeatureLayer pFeatureLayer;
        private void ArcGISDynamicMapServiceLayer_Initialized(object sender, EventArgs e)
        {

            //QueryTask pQueryTask = new QueryTask("http://192.168.220.64:6080/arcgis/rest/services/test/fields_with_domain_query/MapServer/0");

            //Query pquery = new Query();
            //pquery.Where = "1=1";
            //pquery.OutFields.Add("*");

            //pQueryTask.ExecuteCompleted += pQueryTask_ExecuteCompleted;

            //pQueryTask.ExecuteAsync(pquery);


        }

        void pQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
         


        }

        private void FeatureLayer_Initialized(object sender, EventArgs e)
        {
            pFeatureLayer = sender as FeatureLayer;
            foreach (Field field in pFeatureLayer.LayerInfo.Fields)
            {
                if (field.Name == "NAME")
                {

                    CodedValueDomain cvD = field.Domain as CodedValueDomain;

                }
            }


        }
    }
}
