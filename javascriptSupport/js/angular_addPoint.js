/**
 * Created by jiangmb on 2016/4/7.
 */
var app = angular.module('myApp', []);
app.controller('myCtrl', function($scope,$http) {
    $scope.name = "point";
    $scope.addLayer = "AddLayer";
    $scope.createMap=function() {

        require(["dojo/parser", "esri/map", "esri/layers/FeatureLayer", "esri/geometry/Extent"], function (parser, Map, FeatureLayer, Extent) {
            var map;
            parser.parse();
            var extent = new Extent(extent_json);
            map = new Map("myMap");
        });
    }
    $scope.addLayerEvent = function () {
        $http.get($scope.layerUrl + "?f=json").then(function mysuccess(response){

            //get the extent of the adding layer
            var extent_json=response.data.extent;
            var map;
            require(["dojo/parser","esri/map","esri/layers/FeatureLayer","esri/geometry/Extent"],function(parser,Map,FeatureLayer,Extent){

              parser.parse();
                var extent=new Extent(extent_json);
                map=new Map("myMap",{
                    extent:extent
                });

                var featureLayer=new FeatureLayer($scope.layerUrl,{
                    mode: FeatureLayer.MODE_ONDEMAND,
                });
                map.addLayer(featureLayer);

            });
        },function myError(response){

            alert("invalid url");

        });




    }
});


