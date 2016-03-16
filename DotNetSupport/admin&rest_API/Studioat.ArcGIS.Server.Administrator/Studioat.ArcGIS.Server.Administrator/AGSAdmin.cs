using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


////

/////

namespace Studioat.ArcGIS.Server.Administrator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using ESRI.ArcGIS.SOESupport;

    /// <summary>
    /// 服务类型
    /// </summary>
    public enum ServiceType
    {
        MapServer,
        GeocodeServer,
        SearchServer,
        IndexingLauncher,
        IndexGenerator,
        GeometryServer,
        GeoDataServer,
        GPServer,
        GlobeServer,
        ImageServer
    }

    /// <summary>
    /// 负载平衡
    /// </summary>
    public enum LoadBalancing
    {
        ROUND_ROBIN,
        FAIL_OVER
    }

    /// <summary>
    /// isolation level
    /// 
    /// </summary>
    public enum IsolationLevel
    {
        LOW,
        HIGH
    }

    /// <summary>
    /// administrative API Rest
    /// </summary>
    public class AGSAdmin
    {
        private string username;
        private string password;
        private string urlRestAdmin;
        private string urlRestServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AGSAdmin"/> class.
        /// </summary>
        /// <param name="serverName">server name</param>
        /// <param name="port">port of server</param>
        /// <param name="username">username administrator</param>
        /// <param name="password">password administrator</param>
        public AGSAdmin(string serverName, int port, string username, string password)
        {
            this.username = username;
            this.password = password;
            string url = string.Format("http://{0}:{1}/arcgis", serverName, port.ToString());
            this.urlRestAdmin = url + "/admin";
            this.urlRestServer = url + "/server";
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="AGSAdmin"/> class from being created.
        /// </summary>
        private AGSAdmin()
        {
        }

        /// <summary>
        /// Create arcgis server folder
        /// </summary>
        /// <param name="folderName">Folder name</param>
        /// <param name="description">Description of the folder</param>
        /// <returns>True if successfully created</returns>
        public bool CreateServerFolder(string folderName, string description)
        {
            try
            {
                string token = this.GenerateAGSToken();
                string folderUrl = this.urlRestAdmin + "/services/" + folderName + "?f=json&token=" + token;
                string resultExistsFolder = this.GetResult(folderUrl);
                if (!this.HasError(resultExistsFolder))
                {
                    return true; // exists
                }
                else
                {
                    string createFolderUrl = this.urlRestAdmin + "/services/createFolder";
                    string postContent = string.Format("folderName={0}&description={1}&f=json&token={2}", folderName, description, token);
                    string result = this.GetResult(createFolderUrl, postContent);
                    return this.HasSuccess(result);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get physical Path and virtual Path from directory ags
        /// </summary>
        /// <param name="directory">directory ags</param>
        /// <param name="physicalPath">physical Path</param>
        /// <param name="virtualPath">virtual Path</param>
        /// <returns>True if successfully return path</returns>
        public bool GetServerDirectory(string directory, out string physicalPath, out string virtualPath)
        {
            physicalPath = null;
            virtualPath = null;
            try
            {
                string token = this.GenerateAGSToken();
                string directoryUrl = this.urlRestAdmin + "/system/directories/" + directory + "?f=json&token=" + token;

                string result = this.GetResult(directoryUrl);

                JsonObject jsonObject = new JsonObject(result);
                if (!jsonObject.Exists("physicalPath") || !jsonObject.TryGetString("physicalPath", out physicalPath))
                {
                    throw new Exception();
                }

                jsonObject = new JsonObject(result);
                if (!jsonObject.Exists("virtualPath") || !jsonObject.TryGetString("virtualPath", out virtualPath))
                {
                    throw new Exception();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete Service
        /// </summary>
        /// <param name="serviceName">Service Name</param>
        /// <param name="serviceType">Server Type</param>
        /// <returns>True if successfully deleted</returns>
        public bool DeleteService(string serviceName, ServiceType serviceType)
        {
            try
            {
                string token = this.GenerateAGSToken();
                string serviceUrl = this.urlRestAdmin + "/services/" + serviceName + "." + Enum.GetName(typeof(ServiceType), serviceType) + "/delete";
                string result = this.GetResult(serviceUrl, "f=json&token=" + token);
                return this.HasSuccess(result);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Start Service
        /// </summary>
        /// <param name="serviceName">Service Name</param>
        /// <param name="serviceType">Server Type</param>
        /// <returns>True if successfully started</returns>
        public bool StartService(string serviceName, ServiceType serviceType)
        {
            try
            {
                string token = this.GenerateAGSToken();
                string serviceUrl = this.urlRestAdmin + "/services/" + serviceName + "." + Enum.GetName(typeof(ServiceType), serviceType) + "/start";
                string result = this.GetResult(serviceUrl, "f=json&token=" + token);
                return this.HasSuccess(result);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviceName">Service Name</param>
        /// <param name="serviceType">Server Type</param>
        /// <returns>True if successfully stopped</returns>
        public bool StopService(string serviceName, ServiceType serviceType)
        {
            try
            {
                string token = this.GenerateAGSToken();
                string serviceUrl = this.urlRestAdmin + "/services/" + serviceName + "." + Enum.GetName(typeof(ServiceType), serviceType) + "/stop";
                string result = this.GetResult(serviceUrl, "f=json&token=" + token);
                return this.HasSuccess(result);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 列举服务
        /// </summary>
        public void ListServices()
        {
            this.ListServices(null);
        }

        /// <summary>
        /// list of services in folder
        /// </summary>
        /// <param name="folder">name of folder</param>
        public void ListServices(string folder)
        {
            try
            {
                string token = this.GenerateAGSToken();
                string serviceUrl = this.urlRestAdmin + "/services/" + folder;
                string postcontent = "f=json&token=" + token;
                string result = this.GetResult(serviceUrl, postcontent);

                JsonObject jsonObject = new JsonObject(result);
                object[] folders = null;
                if (jsonObject.Exists("folders") && jsonObject.TryGetArray("folders", out folders))
                {
                    foreach (string subfolder in folders)
                    {
                        this.ListServices(subfolder);
                    }
                }

                object[] services = null;
                if (jsonObject.Exists("services") && jsonObject.TryGetArray("services", out services))
                {
                    IEnumerable<JsonObject> jsonObjectService = services.Cast<JsonObject>();
                    jsonObjectService.ToList().ForEach(jo =>
                    {
                        string serviceName;
                        jo.TryGetString("serviceName", out serviceName);
                        string folderName;
                        jo.TryGetString("folderName", out folderName);
                        Console.WriteLine(folderName + "/" + serviceName);
                    });
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// create service type MapServer
        /// </summary>
        /// <returns>>True if successfully created</returns>
        public bool CreateService()
        {
            try
            {
                string token = this.GenerateAGSToken();
                string serviceUrl = this.urlRestAdmin + "/services/createService";

                JsonObject jsonObject = new JsonObject();
                jsonObject.AddString("serviceName", "Test");
                //服务类型
                jsonObject.AddString("type", Enum.GetName(typeof(ServiceType), ServiceType.GPServer));
                jsonObject.AddString("description", "This is an example");
                //不同的服务类型，其capabilities是不同的，地图服务的为Map，query和data
              //  jsonObject.AddString("capabilities", "Map,Query,Data");

                jsonObject.AddString("capabilities","Uploads");//gp 服务的capabilities
                jsonObject.AddString("clusterName", "default");
                jsonObject.AddLong("minInstancesPerNode", 1);
                jsonObject.AddLong("maxInstancesPerNode", 2);
                jsonObject.AddLong("maxWaitTime", 60);
                jsonObject.AddLong("maxStartupTime", 300);
                jsonObject.AddLong("maxIdleTime", 1800);
                jsonObject.AddLong("maxUsageTime", 600);
                jsonObject.AddLong("recycleInterval", 24);
                jsonObject.AddString("loadBalancing", Enum.GetName(typeof(LoadBalancing), LoadBalancing.ROUND_ROBIN));
                jsonObject.AddString("isolationLevel", Enum.GetName(typeof(IsolationLevel), IsolationLevel.HIGH));

                JsonObject jsonObjectProperties = new JsonObject();

                // see for a list complete http://resources.arcgis.com/en/help/server-admin-api/serviceTypes.html
                jsonObjectProperties.AddLong("maxBufferCount", 100); // optional 100
                jsonObjectProperties.AddString("virtualCacheDir", this.urlRestServer + "/arcgiscache"); // optional
                jsonObjectProperties.AddLong("maxImageHeight", 2048); // optional 2048
                jsonObjectProperties.AddLong("maxRecordCount", 1000); // optional 500

               //10.1中服务是通过msd的形式发布的，所以创建地图服务时候将mxd转换成msd的形式，创建msd的形式而其他服务的数据发布形式，参考上面的链接
               
              //  jsonObjectProperties.AddString("filePath", @"C:\AvGis\Test\mappa\UTM_ReteFognaria.msd"); //地图服务 required
                        
               
                jsonObjectProperties.AddString( "toolbox",@"d:\Buffer.tbx");//gp服务使用的是路径创建gp服务的路径

                jsonObjectProperties.AddLong("maxImageWidth", 2048); // optional 2048
                jsonObjectProperties.AddBoolean("cacheOnDemand", false); // optional false
                jsonObjectProperties.AddString("virtualOutputDir", this.urlRestServer + "/arcgisoutput");
                jsonObjectProperties.AddString("outputDir", @"C:\arcgisserver\directories\arcgisoutput");
                jsonObjectProperties.AddString("jobsDirectory", @"C:\arcgisserver\directories\arcgisjobs");                                                                             // required
                jsonObjectProperties.AddString("supportedImageReturnTypes", "MIME+URL"); // optional MIME+URL
                jsonObjectProperties.AddBoolean("isCached", false); // optional false
                jsonObjectProperties.AddBoolean("ignoreCache", false); // optional false 
                jsonObjectProperties.AddBoolean("clientCachingAllowed", false); // optional true 
                jsonObjectProperties.AddString("cacheDir", @"C:\arcgisserver\directories\arcgiscache"); // optional

                jsonObject.AddJsonObject("properties", jsonObjectProperties);

                string result = this.GetResult(serviceUrl, "service=" +HttpUtility.UrlEncode(jsonObject.ToJson()) + "&f=json&token=" + token);
                return this.HasSuccess(result);
               
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// check is status is equal success
        /// </summary>
        /// <param name="result">result of request</param>
        /// <returns>True if status is equal success</returns>
        private bool HasSuccess(string result)
        {
            JsonObject jsonObject = new JsonObject(result);
            string status = null;
            if (!jsonObject.Exists("status") || !jsonObject.TryGetString("status", out status))
            {
                return false;
            }

            return status == "success";
        }

        /// <summary>
        ///错误信息判断
        /// </summary>
        /// <param name="result">result of request</param>
        /// <returns>True if status is equal error</returns>
        private bool HasError(string result)
        {
            JsonObject jsonObject = new JsonObject(result);
            string status = null;
            if (!jsonObject.Exists("status") || !jsonObject.TryGetString("status", out status))
            {
                return false;
            }

            return status == "error";
        }

        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url">get 请求url</param>
        /// <returns>return response</returns>
        private string GetResult(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="postContent">post content</param>
        /// <returns></returns>
        private string GetResult(string url, string postContent)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                byte[] content = Encoding.UTF8.GetBytes(postContent);
                request.ContentLength = content.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = WebRequestMethods.Http.Post;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(content, 0, content.Length);
                    requestStream.Close();
                    WebResponse response = request.GetResponse();
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 产生token
        /// </summary>
        /// <returns>返回一个toke，采用默认的过期时间令牌</returns>
        private string GenerateAGSToken()
        {
            try
            {
                string urlGenerateToken = string.Format("{0}/generateToken", this.urlRestAdmin);
                string credential = string.Format("username={0}&password={1}&client=requestip&expiration=&f=json", this.username, this.password);
                string result = this.GetResult(urlGenerateToken, credential);

                JsonObject jsonObject = new JsonObject(result);
                string token = null;
                if (!jsonObject.Exists("token") || !jsonObject.TryGetString("token", out token))
                {
                    throw new Exception("Token not found!");
                }

                return token;
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
