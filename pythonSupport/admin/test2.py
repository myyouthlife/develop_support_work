import urllib
import json
from datetime import *

print "begin to request services:"
t_begin2=datetime.now()
url="	http://192.168.220.80:6080/arcgis/rest/services/Untitled/MapServer?f=pjson"



jsonResponse = urllib.urlopen(url, urllib.urlencode(''))
if jsonResponse.getcode()==200:
    result=json.loads(jsonResponse.read())
    print result
    t_end2=datetime.now()
         
    print "instantiated time:"+str((t_end2-t_begin2).seconds)
