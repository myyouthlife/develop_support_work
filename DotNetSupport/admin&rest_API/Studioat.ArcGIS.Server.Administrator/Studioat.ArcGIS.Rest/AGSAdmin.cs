using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studioat.ArcGIS.Rest
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

     / / /  <summary> 
    / / /  arcgis server service types (map table service type) 
    / / /  </ summary> 
    public  enum  ServiceType
    {
        MapServer,
        GeocodeServer,
        SearchServer,
        IndexingLauncher,
        Index generator,
        GeometryServer,
        GeoDataServer,
        GPServer,
        GlobeServer,
        ImageServer
    }
 
    /// <summary>
    /// Load Balancing
    /// </summary>
    public enum LoadBalancing
    { 
       ROUND_ROBIN,
       FAIL_OVER 
    }
 
    /// <summary>
    /// isolation level
    /// </summary>
    public enum IsolationLevel
    {
        LOW,
        HIGH
    }
  /// <summary>
    /// administrative API Rest
    /// </summary>
    class AGSAdmin
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
                String  token =  this . GenerateAGSToken ();
                 string  folderUrl =  this . urlRestAdmin +  "/ services /"  + foldername +  "? f = json & token = '  + token;
                 string  resultExistsFolder =  this . getResult (folderUrl),
                 if  (! this . HasError (resultExistsFolder))
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
            physicalPath =  null ;
            virtualPath =  null ;
             try
            {
                String  token =  this . GenerateAGSToken ();
                 string  directoryUrl =  this . urlRestAdmin +  "/ system / directories /"  + directory +  "? f = json & token = '  + token;
 
                string result = this.GetResult(directoryUrl);
 
                JsonObject  jsonObject =  new  JsonObject (result)
                 if  (! jsonObject.Exists ( "PhysicalPath" ) | |! jsonObject.TryGetString ( "PhysicalPath" ,  out  PhysicalPath))
                {
                    throw new Exception();
                }
 
                jsonObject =  new  JsonObject (result)
                 if  (! jsonObject.Exists ( "virtualPath" ) | |! jsonObject.TryGetString ( "virtualPath" ,  out  virtualPath))
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
                String  token =  this . GenerateAGSToken ();
                 string  serviceUrl =  this . urlRestAdmin +  "/ services /"  + service name +  "."  +  Enum . getName ( typeof ( Service Type ), service type) +  "/ delete" ;
                 String  result =  this . getResult (serviceUrl,  "f = json & token ="  + token);
                 return  this . HasSuccess (result);
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
                String  token =  this . GenerateAGSToken ();
                 string  serviceUrl =  this . urlRestAdmin +  "/ services /"  + service name +  "."  +  Enum . getName ( typeof ( Service Type ), service type) +  "/ start" ;
                 String  result =  this . getResult (serviceUrl,  "f = json & token ="  + token);
                 return  this . HasSuccess (result);
            }
            catch
            {
                return false;
            }
        }
 
        /// <summary>
        /// Stop Service
        /// </summary>
        /// <param name="serviceName">Service Name</param>
        /// <param name="serviceType">Server Type</param>
        /// <returns>True if successfully stopped</returns>
        public bool StopService(string serviceName, ServiceType serviceType)
        {
            try
            {
                String  token =  this . GenerateAGSToken ();
                 string  serviceUrl =  this . urlRestAdmin +  "/ services /"  + service name +  "."  +  Enum . getName ( typeof ( Service Type ), service type) +  "/ stop" ;
                 String  result =  this . getResult (serviceUrl,  "f = json & token ="  + token);
                 return  this . HasSuccess (result);
            }
            catch
            {
                return false;
            }
        }
 
        /// <summary>
        /// list of services
        /// </summary>
        public void ListServices()
        {
            this . List Services ( null );
        }
 
        /// <summary>
        /// list of services in folder
        /// </summary>
        /// <param name="folder">name of folder</param>
        public void ListServices(string folder)
        {
            try
            {
                String  token =  this . GenerateAGSToken ();
                 string  serviceUrl =  this . urlRestAdmin +  "/ services /"  + folder;
                 thong  post content =  "f = json & token = '  + token;
                 String  result =  this . getResult (serviceUrl, mail content);
 
                JsonObject  jsonObject =  new  JsonObject (result);
                 object [] folders =  null ,
                 f  (jsonObject.Exists ( "folders" ) && jsonObject.TryGetArray ( "folders" ,  out  folders))
                {
                    foreach (string subfolder in folders)
                    {
                        this.ListServices(subfolder);
                    }
                }
 
                object [] services =  null ,
                 f  (jsonObject.Exists ( "services" ) && jsonObject.TryGetArray ( "services" ,  out  services))
                {
                    IEnumerable < JsonObject > jsonObjectService = services.Cast < JsonObject > ();
                    jsonObjectService.ToList().ForEach(jo =>
                        {
                            string  service name;
                            jo.TryGetString ( "service name" ,  out  service name);
                             string  foldername;
                            jo.TryGetString ( "foldername" ,  out  foldername);
                             Console . Write Line (folder name +  "/"  + service name);
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
                String  token =  this . GenerateAGSToken ();
                 string  serviceUrl =  this . urlRestAdmin +  "/ services / create service" ;
 
                JsonObject jsonObject = new JsonObject();
                jsonObject.AddString ( "serviceName" ,  "Test" );
                jsonObject.AddString ( "type" ,  Enum . GetName ( typeof ( ServiceType )  ServiceType . MapServer));
                jsonObject.AddString ( "description" ,  "This is an example" );
                jsonObject.AddString ( "capabilities" ,  "Map, Query, Data" );
                jsonObject.AddString ( "ClusterName" ,  "default" );
                jsonObject.AddLong ( "minInstancesPerNode" , 1);
                jsonObject.AddLong ( "maxInstancesPerNode" , 2);
                jsonObject.AddLong ( "maxWaitTime" , 60); 
                jsonObject.AddLong ( "maxStartupTime" , 300); 
                jsonObject.AddLong ( "MaxIdleTime" , 1800);
                jsonObject.AddLong ( "maxUsageTime" , 600);
                jsonObject.AddLong ( "recycleInterval" , 24);
                jsonObject.AddString ( "loadBalancing" ,  Enum . GetName ( typeof ( LoadBalancing )  LoadBalancing . ROUND_ROBIN));
                jsonObject.AddString ( "isolationLevel" ,  Enum GetName (. typeof ( IsolationLevel )  IsolationLevel HIGH).)
 
                JsonObject  jsonObjectProperties =  new  JsonObject ();
 
                // see for a list complete http://resources.arcgis.com/en/help/server-admin-api/serviceTypes.html
                jsonObjectProperties.AddLong("maxBufferCount", 100); // optional 100
                jsonObjectProperties.AddString("virtualCacheDir", this.urlRestServer + "/arcgiscache"); // optional
                jsonObjectProperties.AddLong("maxImageHeight", 2048); // optional 2048
                jsonObjectProperties.AddLong("maxRecordCount", 1000); // optional 500
 
                // Starting at ArcGIS 10.1, Map Server Definition ( .msd ) files have been
                // replaced with Service Definition Draft ( .sddraft ) and Service Definition ( .sd ) files. 
                // In the case of a map service, you must specify a map service definition (MSD) file in your JSON. 
                // This file synthesizes information from your ArcMap document (MXD) in a format that can be understood and 
                // drawn by ArcGIS Server. You must use the arcpy.mapping module to analyze your map and create the MSD before 
                // you can go ahead with creating the service. This part requires a machine licensed for ArcGIS for Desktop. 
                // Other service types do not require you to use arcpy.mapping or create an MSD.
                jsonObjectProperties.AddString("filePath", @"C:\AvGis\Test\mappa\UTM_ReteFognaria.msd"); // required
                
                jsonObjectProperties.AddLong("maxImageWidth", 2048); // optional 2048
                jsonObjectProperties.AddBoolean("cacheOnDemand", false); // optional false
                jsonObjectProperties.AddString("virtualOutputDir", this.urlRestServer + "/arcgisoutput");
                jsonObjectProperties.AddString ( "outputDir" ,  @ "C: \ arcgisserver \ directories \ arcgisoutput" )  / / required 
                jsonObjectProperties.AddString ( "supportedImageReturnTypes" ,  "MIME + URL" ),  / / optional MIME + URL 
                jsonObjectProperties.AddBoolean ( " isCached " ,  false )  / / false optional 
                jsonObjectProperties.AddBoolean ( "ignoreCache" ,  false )  / / false optional  
                jsonObjectProperties.AddBoolean ( "clientCachingAllowed" ,  false )  / / optional true  
                jsonObjectProperties.AddString ( "Cached" ,  @ " C: \ arcgisserver \ directories \ arcgiscache " )  / / optional
 
                jsonObject.AddJsonObject ( "properties" , jsonObjectProperties);
 
                String  result =  this . GetResult (serviceUrl,  "service ="  +  HttpUtility . UrlEncode (jsonObject.ToJson ()) +  "& f = json & token ="  + token)
                 return  this . HasSuccess (result);
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
            JsonObject  jsonObject =  new  JsonObject (result);
             String  status =  null ,
             if  (! jsonObject.Exists ( "status" ) | |! jsonObject.TryGetString ( "status" ,  out  status))
            {
                return false;
            }
 
            return status == "success";
        }
 
        /// <summary>
        /// check is status is equal error
        /// </summary>
        /// <param name="result">result of request</param>
        /// <returns>True if status is equal error</returns>
        private bool HasError(string result)
        {
            JsonObject  jsonObject =  new  JsonObject (result);
             String  status =  null ,
             if  (! jsonObject.Exists ( "status" ) | |! jsonObject.TryGetString ( "status" ,  out  status))
            {
                return false;
            }
 
            return status == "error";
        }
 
        /// <summary>
        /// Get request rest
        /// </summary>
        /// <param name="url">url of request</param>
        /// <returns>return response</returns>
        private string GetResult(string url)
        {
            try
            {
                WebRequest  request =  WebRequest . Create (url);
                 WebResponse  response = request.GetResponse ()
                 using  ( Stream  responseStream = response.GetResponseStream ())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        return  reader.ReadToEnd ();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
 
        /// <summary>
        /// Post request rest
        /// </summary>
        /// <param name="url">url of request</param>
        /// <param name="postContent">content of post</param>
        /// <returns>return response</returns>
        private string GetResult(string url, string postContent)
        {
            try
            {
                WebRequest  request =  WebRequest . Create (url);
                 byte [] content =  Encoding . UTF8.GetBytes (postContent);
                request.ContentLength = content.Length;
                request.ContentType =  "application / x-www-form-urlencoded" ;
                request.Method =  WebRequestMethods . Http . Post,
                 using  ( Stream  requestStream request.GetRequestStream = ())
                {
                    requestStream.Write(content, 0, content.Length);
                    requestStream.Close ();
                    WebResponse  response = request.GetResponse ()
                     using  ( Stream  responseStream = response.GetResponseStream ())
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            return  reader.ReadToEnd ();
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
        /// Generate a token
        /// </summary>
        /// <returns>A token that has default expiration time</returns>
        private string GenerateAGSToken()
        {
            try
            {
                string urlGenerateToken = string.Format("{0}/generateToken", this.urlRestAdmin);
                string credential = string.Format("username={0}&password={1}&client=requestip&expiration=&f=json", this.username, this.password);
                string result = this.GetResult(urlGenerateToken, credential);
 
                JsonObject  jsonObject =  new  JsonObject (result)
                 string  token =  null ;
                 if  (! jsonObject.Exists ( "token" ) | |! jsonObject.TryGetString ( "token" ,  out  token))
                {
                    throw new Exception("Token not found!");
                }
 
                return token;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
