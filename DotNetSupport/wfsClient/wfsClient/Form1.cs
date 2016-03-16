using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace wfsClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = "http://localhost:6080/arcgis/services/wfs/wfsttest2/MapServer/WFSServer?";
            url += "request=getfeature&typename=time";
            this.richTextBox1.Text = SendGetRequest(url);


        }

        private string SendGetRequest(string url)
        {
            string pReply = "";
            using (WebClient pWebClient = new WebClient())
            {
                pReply = pWebClient.DownloadString(url);

            }
            return pReply;
        }

        private string SendPostRequest(string url, string parm)
        {
            string responsebody = "";
            using (WebClient pWebClient = new WebClient())
            {
                System.Collections.Specialized.NameValueCollection reqparm = new System.Collections.Specialized.NameValueCollection();
                //reqparm.Add("filter", parm);
                //reqparm.Add("request", 'Transaction');


                //reqparm.Add("param2", "escaping is already handled");
                byte[] senddata = Encoding.UTF8.GetBytes(parm);
              
                pWebClient.Headers.Add("Content-Type", "application/xml;charset=UTF-8");
                pWebClient.Headers.Add("ContentLength", senddata.Length.ToString());
                byte[] responsebytes = pWebClient.UploadData(url, "POST", senddata);
                responsebody = Encoding.UTF8.GetString(responsebytes);
            }
            return responsebody;
        }

        private string ReadXmlFile(string xmlPath)
        {
            XmlDocument pDoc = new XmlDocument();
            pDoc.Load(xmlPath);
            string dd = pDoc.OuterXml;
            return dd;



        }

        private void button2_Click(object sender, EventArgs e)
        {
            string url = "http://localhost:6080/arcgis/services/wfs/MyMapServicewfs/MapServer/WFSServer";
            string senddata = ReadXmlFile(@"D:\vsproject\testprojectofwpf\wfsClient\wfsClient\filter.xml");
            //string reponse = SendPostRequest(url, senddata);
            XDocument pXDocument = WfsWebRequest(url, senddata, "post");
        }

        private XDocument WfsWebRequest(string url, string xmlBody, string type)
        {
            XDocument docReturn;

            WebRequest request = WebRequest.Create(url);
            request.Method = type.ToUpper();

            if (type == "post")
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(xmlBody);
                request.ContentType = "text/xml";
                request.ContentLength = byteArray.Length;
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();
            }

            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
           {
                StreamReader read = new StreamReader(stream, System.Text.Encoding.UTF8);
                string json = read.ReadToEnd();

                docReturn = XDocument.Parse(json);
                string owsNameSpace = "{http://www.opengis.net/ows}";
                var exElement = docReturn.Element(owsNameSpace + "ExceptionReport");
                if (exElement != null)
                {
                    //_log.Error("发送wfs服务请求异常信息:" + exElement);
                    docReturn = null;
                }
            }
            response.Close();

            return docReturn;
        }

    }
}
