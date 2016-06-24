/**
 * Created by jiangmb on 2016/4/29.
 */
//leaflet中vectorlayer根据几何类型不同，而不同，分为polyline,
latlngs=[]
var polyline=L.polyline(latlngs,{

    color: 'red'
}).addTo(map);

map.fitBounds(polyline.getBounds());