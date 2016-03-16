using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Studioat.ArcGIS.Server.Administrator
{
    class Program
    {
        static void Main(string[] args)
        {
            AGSAdmin agsAdmin = new AGSAdmin("localhost", 6080, "esri-jmb", "jmb");

            //string serviceName = "mapServices/California_dynamic";
            //bool result = agsAdmin.StopService(serviceName, ServiceType.MapServer);
            //Console.WriteLine("stop service {0}, result: {1}", serviceName, result);

            ////string serviceName = "SampleWorldCities";
            //result = agsAdmin.StartService(serviceName, ServiceType.MapServer);
            //Console.WriteLine("stop service {0}, result: {1}", serviceName, result);

            //string folder = "mapServices";
            //result = agsAdmin.CreateServerFolder(folder, "Prova 1");
            //Console.WriteLine("create folder {0}, result: {1}", folder, result);

            //string physicalPath;
            //string virtualPath;
            //result = agsAdmin.GetServerDirectory("arcgisoutput", out physicalPath, out virtualPath);
            //Console.WriteLine("physicalPath {0}, virtualPath {1}, result: {2}", physicalPath, virtualPath, result);

            //agsAdmin.ListServices();
            //Console.Read();


           agsAdmin.CreateService();
            Console.ReadKey();
        }
    }
}
