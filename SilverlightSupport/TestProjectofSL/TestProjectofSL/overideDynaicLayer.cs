using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client;

namespace TestProjectofSL
{
    public class overideDynaicLayer : DynamicMapServiceLayer
    {

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void GetSource(DynamicLayer.ImageParameters properties, DynamicLayer.OnImageComplete onComplete)
        {

            base.GetSource(properties, onComplete);
        }

        public override void GetUrl(ImageParameters properties, OnUrlComplete onComplete)
        {
            

        }

    }

}
