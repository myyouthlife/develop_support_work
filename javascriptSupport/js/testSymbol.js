/**
 * Created by jiangmb on 2016/3/25.
 */

define(["dojo/_base/declare",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/symbols/PictureFillSymbol",
    "esri/symbols/CartographicLineSymbol",
    "esri/symbols/SimpleFillSymbol",
     "esri/symbols/TextSymbol",
    "esri/graphic",
    "esri/renderers/SimpleRenderer",
        "esri/Color",


], function (declare,
             SimpleMarkerSymbol,
             SimpleLineSymbol,
             PictureFillSymbol,
             CartographicLineSymbol,
             SimpleFillSymbol,
             TextSymbol,
             graphic,
             SimpleRenderer,
             Color) {

    var markerSymbol = new SimpleMarkerSymbol();
    markerSymbol.setPath("M16,4.938c-7.732,0-14,4.701-14,10.5c0,1.981,0.741,3.833,2.016,5.414L2,25.272l5.613-1.44c2.339,1.316,5.237,2.106,8.387,2.106c7.732,0,14-4.701,14-10.5S23.732,4.938,16,4.938zM16.868,21.375h-1.969v-1.889h1.969V21.375zM16.772,18.094h-1.777l-0.176-8.083h2.113L16.772,18.094z");
    markerSymbol.setColor(new Color("#00FFFF"));
    var lineSymbol = new CartographicLineSymbol(
        CartographicLineSymbol.STYLE_SOLID,
        new Color([255, 0, 0]), 10,
        CartographicLineSymbol.CAP_ROUND,
        CartographicLineSymbol.JOIN_MITER, 5
    );
    var pictureFillsymbol = new PictureFillSymbol(
        "images/mangrove.png",
        new SimpleLineSymbol(
            SimpleLineSymbol.STYLE_SOLID,
            new Color('#000'), 1),
        42,
        42
    );
    var simplefillSymbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID, lineSymbol,
        new Color('#000'));

    // lineSymbol used for freehand polyline, polyline and line.
    var simplMarkRenderer = new SimpleRenderer(markerSymbol);

     var textSymbol =  new TextSymbol("Hello World").setColor(
    new Color([128,0,0])).setAlign(Font.ALIGN_START).setAngle(45).setFont(
    new Font("12pt").setWeight(Font.WEIGHT_BOLD)) ;


    return {
        markerSymbol: markerSymbol,
        lineSymbol: lineSymbol,
        pictureFillsymbol: pictureFillsymbol,
        simplefillSymbol: simplefillSymbol,
        simplMarkRenderer: simplMarkRenderer,
        textSymbol:textSymbol

    }




});


