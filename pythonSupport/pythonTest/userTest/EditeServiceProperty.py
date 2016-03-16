# -*- coding: utf-8 -*-

import arcpy,httplib, urllib, json

# For system tools
import sys

# For reading passwords without echoing
import getpass


# Defines the entry point into the script
def MainWay(minInstances,maxInstances,username,password,serverName,serverPort,service):
    # Print some info
    print
    print "This tool is a sample script that resets the minimum and maximum instances allowed for a service."
    print

    # Ask for admin/publisher user name and password
    #username = "siteadmin"
    #password = "siteadmin24"
    
    # Ask for server name
    #serverName = "192.168.1.24"
    #serverPort = 6080

    print r"Enter the service name in the format <folder>/<name>.<type>."
    #service = r"XJSLYW/031705.MapServer"
    #minInstances = "2"
    #maxInstances = "5"

    # Check to make sure the minimum and maximum are numerical
    try:
        minInstancesNum = int(minInstances)
        maxInstancesNum = int(maxInstances)
    except ValueError:
        print "Numerical value not entered for minimum, maximum, or both."
        return
    
    # Check to make sure that the minimum is not greater than the maximum
    if minInstancesNum > maxInstancesNum:
        print "Maximum number of instances must be greater or equal to minimum number."
        return
    
    # Get a token
    token = getToken(username, password, serverName, serverPort)
    if token == "":
        print "Could not generate a token with the username and password provided."    
        return
    
    serviceURL = "/arcgis/admin/services/" + service
    
    # This request only needs the token and the response formatting parameter 
    params = urllib.urlencode({'token': token, 'f': 'json'})
    
    headers = {"Content-type": "application/x-www-form-urlencoded", "Accept": "text/plain"}
    
    # Connect to service to get its current JSON definition    
    httpConn = httplib.HTTPConnection(serverName, serverPort)
    httpConn.request("POST", serviceURL, params, headers)
    
    # Read response
    response = httpConn.getresponse()
    if (response.status != 200):
        httpConn.close()
        print "Could not read service information."
        return
    else:
        data = response.read()
        
        # Check that data returned is not an error object
        if not assertJsonSuccess(data):          
            print "Error when reading service information. " + str(data)
        else:
            print "Service information read successfully. Now changing properties..."

        # Deserialize response into Python object
        dataObj = json.loads(data)
        httpConn.close()

        # Edit desired properties of the service
        dataObj["minInstancesPerNode"] = minInstancesNum
        dataObj["maxInstancesPerNode"] = maxInstancesNum

        # Serialize back into JSON
        updatedSvcJson = json.dumps(dataObj)

        # Call the edit operation on the service. Pass in modified JSON.
        editSvcURL = "/arcgis/admin/services/" + service + "/edit"
        params = urllib.urlencode({'token': token, 'f': 'json', 'service': updatedSvcJson})
        httpConn.request("POST", editSvcURL, params, headers)
        
        # Read service edit response
        editResponse = httpConn.getresponse()
        if (editResponse.status != 200):
            httpConn.close()
            print "Error while executing edit."
            return
        else:
            editData = editResponse.read()
            
            # Check that data returned is not an error object
            if not assertJsonSuccess(editData):
                print "Error returned while editing service" + str(editData)        
            else:
                print "Service edited successfully."

        httpConn.close()  

        return

# A function to generate a token given username, password and the adminURL.

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
        

# A function that checks that the input JSON object 
#  is not an error object.
    
def assertJsonSuccess(data):
    obj = json.loads(data)
    if 'status' in obj and obj['status'] == "error":
        print "Error: JSON object returns an error. " + str(obj)
        return False
    else:
        return True
    
        
# Script start
NumMinInstances=str(sys.argv[1])
NumMaxInstances=str(sys.argv[2])
UserName=str(sys.argv[3])
Password=str(sys.argv[4])
ServerName=str(sys.argv[5])
ServerPort=str(sys.argv[6])
Service=str(sys.argv[7])

MainWay(NumMinInstances,NumMaxInstances,UserName,Password,ServerName,ServerPort,Service)
