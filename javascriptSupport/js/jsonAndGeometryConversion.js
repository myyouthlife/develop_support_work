/**
 * Created by jiangmb on 2016/3/22.
 */
 require(["dojo/parser",
            "esri/map",
            "esri/toolbars/draw",
            "esri/symbols/SimpleMarkerSymbol", "esri/symbols/SimpleLineSymbol", "esri/symbols/PictureFillSymbol", "esri/symbols/CartographicLineSymbol", "esri/symbols/SimpleFillSymbol",
            "esri/graphic",
            "dojo/i18n!esri/nls/jsapi",
            "esri/Color", "dojo/dom", "dojo/on", "dijit/registry", "dojo/domReady!",

            "dijit/layout/ContentPane", "dijit/layout/BorderContainer", "dijit/layout/TabContainer"
        ], function (parser, Map, Draw, SimpleMarkerSymbol, SimpleLineSymbol, PictureFillSymbol, CartographicLineSymbol, SimpleFillSymbol, Graphic, bundle, Color, dom, on, registry) {
            parser.parse();


            var map
            map = new Map("myMap", {
                basemap: "streets",
                center: [-25.312, 34.307],
                zoom: 3
            });

            map.on("load", initToolbar);

            on(document.getElementById("btnaddLayer"),"click",function(){

                console.log("wwww");

            });
            var markerSymbol = new SimpleMarkerSymbol();
            markerSymbol.setPath("M16,4.938c-7.732,0-14,4.701-14,10.5c0,1.981,0.741,3.833,2.016,5.414L2,25.272l5.613-1.44c2.339,1.316,5.237,2.106,8.387,2.106c7.732,0,14-4.701,14-10.5S23.732,4.938,16,4.938zM16.868,21.375h-1.969v-1.889h1.969V21.375zM16.772,18.094h-1.777l-0.176-8.083h2.113L16.772,18.094z");
            markerSymbol.setColor(new Color("#00FFFF"));

            // lineSymbol used for freehand polyline, polyline and line.
            var lineSymbol = new CartographicLineSymbol(
                    CartographicLineSymbol.STYLE_SOLID,
                    new Color([255, 0, 0]), 10,
                    CartographicLineSymbol.CAP_ROUND,
                    CartographicLineSymbol.JOIN_MITER, 5
            );
            var fillSymbol = new PictureFillSymbol(
                    "images/mangrove.png",
                    new SimpleLineSymbol(
                            SimpleLineSymbol.STYLE_SOLID,
                            new Color('#000'), 1),
                    42,
                    42
            );


            var simplefillSymbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, lineSymbol,
                    new Color('#000'));


            function initToolbar() {

                tb = new Draw(map);
                tb.on("draw-end", addGraphic);
                on(dom.byId("info"), "click", function (evt) {

                    if (evt.target.id === "info") {
                        return;
                    }
                    var tool = evt.target.id.toLowerCase();
                    setChineseToolTip();
                    map.disableMapNavigation();
                    tb.activate(tool);
                });
            }

            function setChineseToolTip() {
                bundle.toolbars.draw.complete = "双击结束绘制";
                bundle.toolbars.draw.addShape = "点击绘制图形，松�?鼠标绘制结束";
                bundle.toolbars.draw.start = "单击�?始绘�?";
                bundle.toolbars.draw.resume = "点击继续";
                bundle.toolbars.draw.finish = "双击结束";
                bundle.toolbars.draw.freehand = "按住绘制，松�?完成";
                bundle.toolbars.draw.addPoint = "点击绘制�?";
                bundle.toolbars.draw.addMultipoint = "点击绘制多点";
                bundle.toolbars.draw.invalidType = "不支持的几何类型";
                bundle.toolbars.draw.convertAntiClockwisePolygon = "逆时针绘制的多边形自动转换顺时针";

            }


            function addGraphic(evt) {
                //deactivate the toolbar and clear existing graphics
                tb.deactivate();
                map.enableMapNavigation();
                map.graphics.clear();
                var symbol;
                /*
                 // figure out which symbol to use


                 if ( evt.geometry.type === "point" || evt.geometry.type === "multipoint") {
                 symbol = markerSymbol;
                 } else if ( evt.geometry.type === "line" || evt.geometry.type === "polyline") {
                 symbol = lineSymbol;
                 }
                 else {
                 symbol = fillSymbol;
                 }
                 */
                symbol = checkGeometryType(evt.geometry)
                map.graphics.add(new Graphic(evt.geometry, symbol));


                //create the json of the geometry
                document.getElementById("divjson").innerHTML = JSON.stringify(evt.geometry.toJson());

                //convert geometry to geojson
                var geojson = Terraformer.ArcGIS.parse(evt.geometry);

                document.getElementById("divGeoJson").innerHTML = JSON.stringify(geojson);

                //convert geometry to wkt
                var wktobject = Terraformer.WKT.convert(geojson);
                document.getElementById("divWKT").innerHTML = JSON.stringify(wktobject);


            }

            function checkGeometryType(jsonGeometry) {
                var symbol;
                if (jsonGeometry.type === "point" || jsonGeometry.type === "multipoint") {
                    symbol = markerSymbol;
                } else if (jsonGeometry.type === "line" || jsonGeometry.type === "polyline") {
                    symbol = lineSymbol;
                }
                else {
                    symbol = fillSymbol;
                }
                return symbol;
            }

            function getSymbol(jsonGemetry) {
                var symbol;

                if ("rings" in jsonGemetry) {

                    symbol = fillSymbol;
                }

                return symbol;

            }


            on(dom.byId("ConvertToGeometry"), "click", function (evt) {
                if (dom.byId(jsonText).value) {
                    jsonGeometry = JSON.parse(dom.byId(jsonText).value);
                    symbol = getSymbol(jsonGeometry);

                    var graphicJson = {geometry: jsonGeometry, "symbol": symbol.toJson()};
                    var grapic = new Graphic(graphicJson);
                    map.graphics.add(grapic);


                }
                else {
                    alert("please input valid geometry json");
                }


            });
        });
