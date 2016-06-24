dojo.require("dojo.dom");
dojo.require("dojo.request");
dojo.require("dojo.json");
dojo.require("esri.map");
dojo.require("esri.graphic");
dojo.require("esri.symbols.SimpleFillSymbol");
dojo.require("esri.symbols.SimpleLineSymbol");
dojo.require("dojo._base.array");
dojo.require("esri.tasks.query");
dojo.require("esri.dijit.InfoWindow");
dojo.require("esri.geometry.Circle");
dojo.require("esri.geometry.jsonUtils");
// 天地图类
dojo.require("tdt.TDTLayer");
dojo.require("tdt.TDTAnnoLayer");
dojo.require("tdt.TDTImgLayer");
dojo.require("tdt.TDTImgAnnoLayer");
dojo.require("esri.dijit.kysPopup");

dojo.require("esri.layers.GraphicsLayer");
dojo.require("esri.layers.FeatureLayer");

var editingLayerServerUrl;// 编辑的要素服务地址
var editingLayer;// 要编辑的图层
var map;
var editingFeature;
var editingGraphic;
var normalGraphicWidth = 4;// 正常道路宽度
var boldGraphicWidth = 4;// 加粗道路宽度
var normalGraphicColor = new dojo.Color([226,0,0]);// 正常道路颜色
var boldGraphicColor = new dojo.Color([0,0,255]);;// 加粗道路颜色
dojo.addOnLoad(function() {
	popup = new esri.dijit.kysPopup({
		titleInBody : false,
		anchor : "top"
	}, dojo.create("div"));
	// **********获取地图对象
	esriConfig.defaults.io.proxyUrl = "/proxy";
	var extent = new esri.geometry.Extent(540976.862, 4733549.842,
			542976.862, 4753549.842, new esri.SpatialReference({
				wkid : 4540
			}));

	map = new esri.Map("mapPane", {
		logo : false,
		infoWindow : popup,
		extent : extent
	});
	loadLayer(dlgPath);
	loadLayer(dlgzjPath);
	loadLayer(dlgPath1);
	loadLayer(dlgzjPath1);
	loadLayer(dlgPath2);
	loadLayer(dlgzjPath2);
	map.setZoom(3);// 初始级别
	initFeature();
});
function resetEditingGraphicWidth() {
	if (editingGraphic != null) {
		setGraphicWidth(editingGraphic, normalGraphicWidth, normalGraphicColor);
	}
}
function boldEditingGraphicWidth() {
	if (editingGraphic != null) {
		setGraphicWidth(editingGraphic, boldGraphicWidth, boldGraphicColor);
	}
}
function setGraphicWidth(graphic, width,color) {
	graphic.symbol.setWidth(width);
	graphic.symbol.setColor( color);
	graphic.draw();
}
function setBgColor(obj,styleName){
	obj.className=styleName;
}
function loadLayer(layerPath) {
	
		var pathLayer = new esri.layers.ArcGISTiledMapServiceLayer(
				layerPath);
		map.addLayer(pathLayer);
}
/**
 * 显示当前编辑的图层
 */
function initFeature() {
	dojo.request.post(
		"a/ajax_getServiceByName",
		{
			data : {
				jsonParam : '{"serviceName":"道路服务"}'
			}
		}).then(function(response) {
			var result = dojo.json.parse(response);
			editingLayerServerUrl =result.serviceUrl;
			editingLayer = new esri.layers.FeatureLayer(editingLayerServerUrl, {
				mode : esri.layers.FeatureLayer.MODE_SNAPSHOT,
				outFields : [ "*" ],
				orderByFields:["LXMC ASC"]
			});
			map.addLayers([ editingLayer ]);
			editingLayer.on("graphic-node-add", function(evt) {
				var r = 226;//parseInt(Math.random() * 255);
				var g = 0;//parseInt(Math.random() * 255);
				var b = 0;//parseInt(Math.random() * 255);
				var symbol = new esri.symbols.SimpleLineSymbol(
						esri.symbols.SimpleLineSymbol.STYLE_SOLID, new dojo.Color([ r,
								g, b ]), normalGraphicWidth);
				evt.graphic.setSymbol(symbol);
				dojo.byId("roadList").innerHTML+=("<a href='#' style='width:100%;' onMouseOver='setBgColor(this,\"mouseOverBg\")' onMouseOut='setBgColor(this,\"mouseOutBg\")' onclick='queryFeature("+evt.graphic.attributes["OBJECTID_1"]+")'>"+evt.graphic.attributes["LXMC"])+"</a><br>";
			});
			editingLayer.on("click", function(evt) {
				resetEditingGraphicWidth();
 
				editingGraphic = evt.graphic;
				boldEditingGraphicWidth();

				editingFeature = evt.graphic.attributes;
				for ( var prop in editingFeature) {
					if (dojo.byId(prop) != null) {
						dojo.byId(prop).value = editingFeature[prop];
					}
				}
			});
			editingLayer.on("mouse-over", function(evt) {// 鼠标经过加粗宽度
				setGraphicWidth(evt.graphic, boldGraphicWidth,boldGraphicColor);
			});
			editingLayer.on("mouse-out", function(evt) {// 鼠标离开正常宽度
				if (evt.graphic != editingGraphic) {
					setGraphicWidth(evt.graphic, normalGraphicWidth, normalGraphicColor);
				}
			});
			map.on("layers-add-result", initEditing);
	}, function(error) {
		console.log(error);
	});
}
function queryFeature(objectId){
	 var query = new esri.tasks.Query();
	  query.objectIds = [objectId];
	  query.outFields = [ "*" ];
	  editingLayer.queryFeatures(query, function(featureSet) {
		   
		  resetEditingGraphicWidth();
			editingGraphic =featureSet.features[0];
			boldEditingGraphicWidth();
			map.centerAt(new esri.geometry.Point(editingGraphic.geometry.paths[0][0][0],editingGraphic.geometry.paths[0][0][1],map.spatialReference));
			editingFeature = editingGraphic.attributes;
			for ( var prop in editingFeature) {
				if (dojo.byId(prop) != null) {
					dojo.byId(prop).value = editingFeature[prop];
				}
			}
	  });
	selectRoadDialog.hide();
}

dojo.require("esri.toolbars.edit");
dojo.require("dojo._base.event");
/**
 * 初始化编辑工具
 * 
 * @param evt
 */
function initEditing(evt) {
	var editToolbar = new esri.toolbars.edit(map);
	editToolbar
			.on(
					"deactivate",
					function(evt) {
						dojo.byId("QDWZ_JD").value = evt.graphic.geometry.paths[0][0][0];
						dojo.byId("QDWZ_WD").value= evt.graphic.geometry.paths[0][0][1];
						dojo.byId("ZDWZ_JD").value = evt.graphic.geometry.paths[0][evt.graphic.geometry.paths[0].length - 1][0];
						dojo.byId("ZDWZ_WD").value = evt.graphic.geometry.paths[0][evt.graphic.geometry.paths[0].length - 1][1];
						
						evt.graphic.attributes.QDWZ_JD = evt.graphic.geometry.paths[0][0][0];
						evt.graphic.attributes.QDWZ_WD = evt.graphic.geometry.paths[0][0][1];
						evt.graphic.attributes.ZDWZ_JD = evt.graphic.geometry.paths[0][evt.graphic.geometry.paths[0].length - 1][0];
						evt.graphic.attributes.ZDWZ_WD = evt.graphic.geometry.paths[0][evt.graphic.geometry.paths[0].length - 1][1];
						editingLayer.applyEdits(null, [ evt.graphic ], null);
					});
	var editingEnabled = false;
	editingLayer.on("dbl-click",
			function(evt) {
				dojo._base.event.stop(evt);
				if (editingEnabled === false) {
					editingEnabled = true;
					editToolbar.activate(esri.toolbars.Edit.EDIT_VERTICES,
							evt.graphic);
				} else {
					editingLayer = this;
					editToolbar.deactivate();
					editingEnabled = false;
				}
			}

	);
}

dojo.require("esri.toolbars.draw");
/**
 * 添加要素
 */
function addFeature() {
	clearPropertyText();
	resetEditingGraphicWidth();
	var drawToolbar = new esri.toolbars.draw(map);
	drawToolbar.activate(esri.toolbars.draw.POLYLINE);
	var newGraphic;
	drawToolbar
			.on(
					"draw-end",
					function(evt) {
						var startPointX = evt.geometry.paths[0][0][0];
						var startPointY = evt.geometry.paths[0][0][1];
						var endPointX = evt.geometry.paths[0][evt.geometry.paths[0].length - 1][0];
						var endPointY = evt.geometry.paths[0][evt.geometry.paths[0].length - 1][1];
						drawToolbar.deactivate();
						var newAttributes = {
							LDBM : "新建路道",
							QDZH : 0,
							ZDZH : 0,
							LDLC : 0,
							LMKD : 0,
							LJKD : 0,
							HDSL : 0,
							KLHLC : 0,
							YLHLC : 0,
							YHLC : 0,
							CDSL : 0,
							SJSS : 0,
							QDWZ_JD : startPointX,
							QDWZ_WD : startPointY,
							ZDWZ_JD : endPointX,
							ZDWZ_WD : endPointY
						};
						newGraphic = new esri.Graphic(evt.geometry, null,
								newAttributes);

						editingLayer.applyEdits([ newGraphic ], null, null);
						editingLayer.on("edits-complete", function(result) {
							console.log(result);
							editingLayer.refresh();
						});
					});

}
/**
 * 保存要素更新
 */
function updateFeature() {
	dojo.query('.featurePropertyTextarea').forEach(function(tag) {
		editingGraphic.attributes[tag.id] = tag.value;
	});
	editingLayer.applyEdits(null, [ editingGraphic ], null);
	editingLayer.on("edits-complete", function(result) {
		console.log(result.updates[0]);
		if (result.updates[0].success == true) {
			editingLayer.refresh();
		} else {
			alert("保存失败,原因:" + result.updates[0].error);
		}

	});

}
/**
 * 清空属性文本框
 */
function clearPropertyText() {
	dojo.query('.featurePropertyTextarea').forEach(function(tag) {
		tag.value = "";
	});

}
/**
 * 删除要素
 */
function deleteFeature() {
	if (editingGraphic == null) {
		alert("请点击要删除的道路");
	} else if (confirm("确定要删除'" + editingGraphic.attributes.LXMC + "'吗?")) {
		editingLayer.applyEdits(null, null, [ editingGraphic ]);
		editingLayer.on("edits-complete", function(result) {
			editingLayer.refresh();
			clearPropertyText();
		});
	}

}