using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISSOERasterAccess
{
    class AddRaster
    {

        public static void AddRastersToImageService(IImageServer imageServer, List<string> fileNames, List<string> fileUrls, IPropertySet attributes)
        {    //Construct an item description.    
            IRasterItemDescription itemDescription = new RasterItemDescriptionClass();
            //Define source raster names, locations, and type.
            IStringArray dataFileNames = new StrArrayClass();
            //File names example: Image32.tif, Image32.tfw, Image32.aux.
            foreach (string fileName in fileNames) dataFileNames.Add(fileName);
            itemDescription.DataFileNames = dataFileNames;
            IStringArray dataFileURLs = new StrArrayClass();
            //File URL examples: c:\temp\Image32.tif, c:\temp\Image32.tfw, http:\\host\Image32.aux.
            foreach (string fileurl in fileUrls) dataFileURLs.Add(fileurl);
            itemDescription.DataFileURLs = dataFileURLs;
            itemDescription.Type = "Raster Dataset";
            //Raster pyramids,statistics,thumbnail.
            itemDescription.BuildPyramids = false;
            itemDescription.BuildThumbnail = false;
            itemDescription.ComputeStatistics = false;
            //If you need to overwrite default raster properties/metadata, provide here.
            itemDescription.Properties = attributes;    //Construct item descriptions.
            IRasterItemDescriptions itemDescriptions = new RasterItemDescriptionsClass();
            itemDescriptions.Add(itemDescription);    //Add.
            IImageServerEditResults isEditResults = ((IImageServer4)imageServer).Add(itemDescriptions);

        }
    }
}
