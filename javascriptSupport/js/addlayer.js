/**
 * Created by jiangmb on 2016/5/4.
 */
var map;
require(["dojo/_base/declare",
    "esri/map",
    "dojo/on",
    "esri/dijit/InfoWindowLite",
    "esri/InfoTemplate",
    "esri/layers/FeatureLayer",
    "esri/layers/ArcGISDynamicMapServiceLayer",
    "esri/layers/ArcGISTiledMapServiceLayer",
    "esri/geometry/Extent",
    "esri/SpatialReference",
    "esri/dijit/LayerList",
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
             LayerList,
             domConstruct) {
    map = new Map("map");
    var dynamicLayer = new ArcGISDynamicMapServiceLayer("http://192.168.220.64:6080/arcgis/rest/services/test/MyMapService/MapServer", {id: "www"});
    map.addLayer(dynamicLayer);

    var myWidget = new LayerList({
        map: map,
        layers: []
    }, "layerList");

    myWidget.startup();

});