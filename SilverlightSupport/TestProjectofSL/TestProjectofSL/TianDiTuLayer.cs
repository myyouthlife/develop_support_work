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
    public class TianDiTuLayer:TiledMapServiceLayer
    {
        public override void Initialize()
        {



            this.FullExtent = new
            ESRI.ArcGIS.Client.Geometry.Envelope(-180, -90, 180, 90);
            {

                SpatialReference = new ESRI.ArcGIS.Client.Geometry.SpatialReference(4326);
            };

            this.SpatialReference = new ESRI.ArcGIS.Client.Geometry.SpatialReference(4326);
            this.TileInfo = new TileInfo()
            {

                Height = 256,
                Width = 256,
                Origin = new ESRI.ArcGIS.Client.Geometry.MapPoint(-180, 90)
                 {

                     SpatialReference = new ESRI.ArcGIS.Client.Geometry.SpatialReference(4326)

                 },
                Lods = new Lod[16]

            };

            TileInfo.Lods[0] = new Lod() { Resolution = 0.703125 };
            TileInfo.Lods[1] = new Lod() { Resolution = 0.3515625 };
            TileInfo.Lods[2] = new Lod() { Resolution = 0.17578125 };
            TileInfo.Lods[3] = new Lod() { Resolution = 0.087890625 };
            TileInfo.Lods[4] = new Lod() { Resolution = 0.0439453125 };
            TileInfo.Lods[5] = new Lod() { Resolution = 0.02197265625 };
            TileInfo.Lods[6] = new Lod() { Resolution = 0.010986328125 };
            TileInfo.Lods[7] = new Lod() { Resolution = 0.0054931640625 };
            TileInfo.Lods[8] = new Lod() { Resolution = 0.00274658203124999 };
            TileInfo.Lods[9] = new Lod() { Resolution = 0.001373291015625 };
            TileInfo.Lods[10] = new Lod() { Resolution = 0.0006866455078125 };
            TileInfo.Lods[11] = new Lod() { Resolution = 0.000343322753906249 };
            TileInfo.Lods[12] = new Lod() { Resolution = 0.000171661376953125 };
            TileInfo.Lods[13] = new Lod() { Resolution = 0.0000858306884765626 };
            TileInfo.Lods[14] = new Lod() { Resolution = 0.0000429153442382813 };
            TileInfo.Lods[15] = new Lod() { Resolution = 0.0000214576721191406 };
            base.Initialize();

        }

        private string _url = "";

        public override string GetTileUrl(int level, int row, int col)
        {

            string url = _url + "&tilecol=" + col + "&tilerow=" + row;
            return url;

        }

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {  
                _url = value;

            }

        }

    }



}
