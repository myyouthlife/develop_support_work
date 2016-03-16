using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Specialized;

using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SOESupport;


//TODO: sign the project (project properties > signing tab > sign the assembly)
//      this is strongly suggested if the dll will be registered using regasm.exe <your>.dll /codebase


namespace Get_UserName_Roler
{
    [ComVisible(true)]
    [Guid("f61d5642-babb-41ba-9a66-e12c5f9832e8")]
    [ClassInterface(ClassInterfaceType.None)]
    [ServerObjectExtension("MapServer",//use "MapServer" if SOE extends a Map service and "ImageServer" if it extends an Image service.
        AllCapabilities = "",
        DefaultCapabilities = "",
        Description = "",
        DisplayName = "Get_UserName_Roler",
        Properties = "",
        SupportsREST = true,
        SupportsSOAP = false)]
    public class Get_UserName_Roler : IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        private string soe_name;

        private IPropertySet configProps;
        private IServerObjectHelper serverObjectHelper;
        private ServerLogger logger;
        private IRESTRequestHandler reqHandler;

        public Get_UserName_Roler()
        {
            soe_name = this.GetType().Name;
            logger = new ServerLogger();
            reqHandler = new SoeRestImpl(soe_name, CreateRestSchema()) as IRESTRequestHandler;
        }

        #region IServerObjectExtension Members

        public void Init(IServerObjectHelper pSOH)
        {
            serverObjectHelper = pSOH;
        }

        public void Shutdown()
        {
        }

        #endregion

        #region IObjectConstruct Members

        public void Construct(IPropertySet props)
        {
            configProps = props;
        }

        #endregion

        #region IRESTRequestHandler Members

        public string GetSchema()
        {
            return reqHandler.GetSchema();
        }

        public byte[] HandleRESTRequest(string Capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            return reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        #endregion

        private RestResource CreateRestSchema()
        {
            RestResource rootRes = new RestResource(soe_name, false, RootResHandler);

            RestOperation sampleOper = new RestOperation("sampleOperation",
                                                    new string[] { "parm1", "parm2" },
                                                    new string[] { "json" },
                                                    SampleOperHandler);

            rootRes.operations.Add(sampleOper);

            return rootRes;
        }

        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            IServerEnvironment3 senv = GetServerEnvironment() as IServerEnvironment3;

            JsonObject result = new JsonObject();

            JsonObject suinfoj = new JsonObject();

            //get user info and serialize into JSON
            IServerUserInfo suinfo = senv.UserInfo;
            if (null != suinfo)
            {
                suinfoj.AddString("currentUser", suinfo.Name);
                IEnumBSTR roles = suinfo.Roles;
                List<string> rolelist = new List<string>();
                if (null != roles)
                {
                    string role = roles.Next();
                    while (!string.IsNullOrEmpty(role))
                    {
                        rolelist.Add(role);
                        role = roles.Next();
                    }
                }

                suinfoj.AddArray("roles", rolelist.ToArray());
                result.AddJsonObject("serverUserInfo", suinfoj);
            }
            else
            {
                result.AddJsonObject("serverUserInfo", null);
            }

            return Encoding.UTF8.GetBytes(result.ToJson());

        }

        private byte[] SampleOperHandler(NameValueCollection boundVariables,
                                                  JsonObject operationInput,
                                                      string outputFormat,
                                                      string requestProperties,
                                                  out string responseProperties)
        {
            responseProperties = null;

            string parm1Value;
            bool found = operationInput.TryGetString("parm1", out parm1Value);
            if (!found || string.IsNullOrEmpty(parm1Value))
                throw new ArgumentNullException("parm1");

            string parm2Value;
            found = operationInput.TryGetString("parm2", out parm2Value);
            if (!found || string.IsNullOrEmpty(parm2Value))
                throw new ArgumentNullException("parm2");

            JsonObject result = new JsonObject();
            result.AddString("parm1", parm1Value);
            result.AddString("parm2", parm2Value);

            return Encoding.UTF8.GetBytes(result.ToJson());
        }

        private IServerEnvironment GetServerEnvironment()
        {
            IEnvironmentManager em = new EnvironmentManagerClass();
            if (em != null)
            {
                UID iseUid = new UIDClass();
                iseUid.Value = "{32d4c328-e473-4615-922c-63c108f55e60}:0";

                try
                {
                    object o = em.GetEnvironment(iseUid);
                    return o as IServerEnvironment;

                }
                catch { }

                return null;
            }

            return null;
        }


    }
}
