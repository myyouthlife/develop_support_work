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
    public partial class addTianditu : UserControl
    {
        public addTianditu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TianDiTuLayer pTianTituLayer = new TianDiTuLayer();
            pTianTituLayer.Url = "http://t0.tianditu.com/vec_c/wmts?request=GetCapabilities&service=wmts";
            myMap.Layers.Add(pTianTituLayer);


        }
    }


}
