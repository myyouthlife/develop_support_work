# -*- coding: utf-8 -*-
import urllib
import urllib2
import arcpy,httplib,json,sys

#url = "http://192.168.1.24:6080/arcgis/rest/services"
#req = urllib2.Request(url)
#print req
#res_data = urllib2.urlopen(req)
#res = res_data.read()
#print res

def getToken(username, password, serverName, serverPort):
    # Token URL is typically http://server[:port]/arcgis/admin/generateToken
    tokenURL = "/arcgis/admin/generateToken"
    
    params = urllib.urlencode({'username': username, 'password': password, 'client': 'requestip', 'f': 'json'})
    
    headers = {"Content-type": "application/x-www-form-urlencoded", "Accept": "text/plain"}
    
    # Connect to URL and post parameters
    httpConn = httplib.HTTPConnection(serverName, serverPort)
    httpConn.request("POST", tokenURL, params, headers)
    
    # Read response
    response = httpConn.getresponse()
    if (response.status != 200):
        httpConn.close()
        print "Error while fetching tokens from admin URL. Please check the URL and try again."
        return
    else:
        data = response.read()
        httpConn.close()
        
        # Check that data returned is not an error object
        if not assertJsonSuccess(data):            
            return
        
        # Extract the token from it
        token = json.loads(data)        
        return token['token']

def assertJsonSuccess(data):
    obj = json.loads(data)
    if 'status' in obj and obj['status'] == "error":
        print "Error: JSON object returns an error. " + str(obj)
        return False
    else:
        return True    

#username="siteadmin"
#password="siteadmin24"
#serverName="192.168.1.24"
#serverPort="6080"
#serviceURL = "/arcgis/rest/services"

username=str(sys.argv[1])
password=str(sys.argv[2])
serverName=str(sys.argv[3])
serverPort=str(sys.argv[4])
serviceURL=str(sys.argv[5])

arcpy.AddMessage("程序刚进入")
print "application has come in."
token=getToken(username,password,serverName,serverPort)
arcpy.AddMessage("程序获得了令牌")
print token
if token == "":
    print "Could not generate a token with the username and password provided." 
params = urllib.urlencode({'token': token, 'f': 'json'})
arcpy.AddMessage("程序已经解析令牌。")
print params
headers = {"Content-type": "application/x-www-form-urlencoded", "Accept": "text/plain"}
# Connect to service to get its current JSON definition    
httpConn = httplib.HTTPConnection(serverName, serverPort)
httpConn.request("POST", serviceURL, params, headers)
 # Read response
response = httpConn.getresponse()
arcpy.AddMessage("程序已经获得响应。")
print response
if (response.status != 200):
    httpConn.close()
    print "Could not read service information."
else:
    data = response.read()
    arcpy.AddMessage("程序已经读到数据。")
    dataTwo=data.decode("UTF-8")
    dataThree=dataTwo.encode("GBK")
    print "dataThree",dataThree
    # Check that data returned is not an error object
    if not assertJsonSuccess(data):          
        print "Error when reading service information. " + str(data)
    else:
        print "Service information read successfully. Now changing properties..."
    # Deserialize response into Python object
    dataObj = json.loads(data)    
    httpConn.close()

    
