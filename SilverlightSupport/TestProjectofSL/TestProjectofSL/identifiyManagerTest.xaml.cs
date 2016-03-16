using ESRI.ArcGIS.Client;
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
using ESRI.ArcGIS.Client.Toolkit;

namespace TestProjectofSL
{
    public partial class identifiyManagerTest : UserControl
    {
        public identifiyManagerTest()
        {
            InitializeComponent();
            IdentityManager.Current.ChallengeMethod = SignInDialog.DoSignIn;
            
        }

        private void Layer_InitializationFailed(object sender, EventArgs e)
        {

        }

        private void Layer_Initialized(object sender, EventArgs e)
        {
            MyMap.Extent = (sender as Layer).FullExtent;

        }
    }
}
