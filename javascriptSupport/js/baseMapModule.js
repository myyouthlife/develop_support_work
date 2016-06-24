define(["dojo/_base/declare",
    "esri/map",
    "dojo/on",
    "esri/dijit/InfoWindowLite",
    "esri/InfoTemplate",
    "esri/layers/FeatureLayer",
    "esri/layers/ArcGISDynamicMapServiceLayer",
    "esri/layers/ArcGISTiledMapServiceLayer",
    "esri/geometry/Extent",
    "esri/SpatialReference",
    "dojo/dom-construct",

    "dojo/domReady!"
], function (declare,
             Map,
             on,
             InfoWindowLite,
             InfoTemplate,
             FeatureLayer,
             ArcGISDynamicMapServiceLayer,
             ArcGISTiledMapServiceLayer,
             Extent,
             SpatialReference,
             domConstruct) {

    var jmbTiledMapServiceLayer=declare(null,{
        url: "ddd",
        constructor:function(){
            var  tiledlayer=new ArcGISTiledMapServiceLayer(this.url);
            return tiledlayer;
        }
    });

    var jmbDynamicMapServiceLayer=declare(null,{

        url:"http://192.168.220.64:6080/arcgis/rest/services/test/cadToServices/MapServer",
        constructor:function(url){
            var dynamicLayer=new ArcGISDynamicMapServiceLayer(url);
            return dynamicLayer;
        }

    });



});


