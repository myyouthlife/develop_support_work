using System;
using System.Collections.Generic;
using System.Json;
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
    public partial class callSOEWithToken : UserControl
    {
        public callSOEWithToken()
        {
            InitializeComponent();
        }
    /*
     * 用户的问题是，soe加密了token怎么调用
     * 通过对用户问题的测试，可以使用两者方式：
     * 1.是加载服务的时候，使用token参数。在cookie有效期内，使用soe无需登录。
     * 2.另外一种方式，是在soe的请求的url中添加token的参数
     * 
     * 用户出现invalid的token的参数，通过对用户的分析，用户出现该问题的原因是，因为其生成token的方式，有问题，建议其使用
     * requestip的形式而不要使用http referer
     */
         
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string soeUrl = "http://192.168.254.142:6080/arcgis/rest/services/Yellowstonemap/MapServer/exts/soeTokenTest/sampleOperation";
            soeUrl += string.Format("?parm1={0}&parm2={1}&f=json&Token={2}", "hello", "jmb","APcSqQPcHSqix9x_jz_A2jvXaj5SCzeZjbBfqTt0aSVTThDd2yXOHMns0ryp0TlU");

            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += (s, a) =>
            {

                JsonValue jsonResponse = JsonObject.Load(a.Result);
                if (jsonResponse != null)
                {
                    this.tb1.Text = jsonResponse["parm1"].ToString();
                    this.tb2.Text = jsonResponse["parm2"].ToString();
                }



            };

            webClient.OpenReadAsync(new Uri(soeUrl));


        }
    }
}
