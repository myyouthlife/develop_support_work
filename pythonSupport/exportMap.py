__author__ = 'Administrator'
import httplib, urllib
params = urllib.urlencode({'bbox':'787305.7985999587,817601.0343623444,833196.2452000414,840656.3066949999','layerDefs':{'0':'id<10000'},'format':'image','f':'html'})
headers = {"Content-type": "application/x-www-form-urlencoded","Accept": "text/plain"}
conn = httplib.HTTPConnection("192.168.220.167:6080")
conn.request("POST", "/arcgis/rest/services/huaweiTest/MapServer/export", params, headers)
response = conn.getresponse()
print response
print response.status, response.reason


__author__ = 'jiangmb'
