/**
 * Created by jiangmb on 2016/3/25.
 */
require([
        "esri/map",
        "esri/tasks/query",
        "esri/layers/FeatureLayer",
        "../jsapi/js/testSymbol.js",
        "dojo/domReady!"
    ],
    function (Map,
              Query,
              FeatureLayer,testSymbol) {

        var map = new Map("map", {
            basemap: "hybrid",
            center: [-82.44109, 35.6122],
            zoom: 17
        });
        /****************************************************************
         * Add feature layer - A FeatureLayer at minimum should point
         * to a URL to a feature service or point to a feature collection
         * object.
         ***************************************************************/
        // Carbon storage of trees in Warren Wilson College.
        var featureLayer = new FeatureLayer("https://services.arcgis.com/V6ZHFr6zdgNZuVG0/arcgis/rest/services/Landscape_Trees/FeatureServer/0");
        /***
         * Add query to this feauturelayer
         */
        featureLayer.setRenderer(testSymbol.simplMarkRenderer);
        map.addLayer(featureLayer);
/*

        var query = new Query();
        query.outFields = ["*"];
        query.where = "FID>10";

        featureLayer.queryExtent(query, function (evt) {

            //hightlight the selected feature


        })
*/


    });