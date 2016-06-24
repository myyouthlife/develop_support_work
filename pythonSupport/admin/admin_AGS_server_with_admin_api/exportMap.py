#coding:cp936
__author__ = 'jiangmb'

from Get_site_information_helper import *

#coding:cp936
__author__ = 'Administrator'

# Required imports
import urllib
import urllib2
import json
import sys
import time


class ADMINself(object):

    def __init__(self, username, password, server, port):
        self.username = username
        self.password = password
        self.server = server
        self.port= port
        self.token, self.expires, self.URL = self.getToken(username, password, server, port)
        self.basicQ = "?f=pjson&token={}".format(self.token)

    def getToken(self, username, password, server, port, exp=60):

        query_dict = {'username':   username,
                      'password':   password,
                      'expiration': str(exp),
                      'client':     'requestip',
                      'f': 'json'}

        tokenURL = "http://{}:{}/arcgis/admin/generateToken".format(server, port)
        token = self.sendAGSReq(tokenURL, query_dict)

        if "token" not in token:
            print token['messages']
            exit()
        else:
            # Return the token, expiry and URL
            return token['token'], token['expires'], "http://{}:{}/arcgis/admin".format(server, port)

    def checkExpiredToken(self):
        # call to check if token time limit has elapsed, if so, request a new one
        # server time in epoch values
        if (self.expires) < int(time.time() * 1000):
            self.token, self.expires, self.URL = self.getToken(self.username, self.password, self.server, self.port)
            print "Obtained new token"
        else:
            print "Token is still valid"



    def sendAGSReq(self,URL, query_dict):
    #
    # Takes a URL and a dictionary and sends the request, returns the JSON

        query_string = urllib.urlencode(query_dict)

        jsonResponse = urllib.urlopen(URL, urllib.urlencode(query_dict))
        jsonOuput = json.loads(jsonResponse.read())

        return jsonOuput

    def checkMSG(self,jsonMSG):
        #
        # Takes JSON and checks if a success message was found

        try:
            if jsonMSG['status'] == "success":
                return True
            else:
                return False
        except:
            return False







if __name__=="__main__":

    server="192.168.220.64"
    port='6080'
    username='arcgis'
    password='Super123'



    adminself=ADMINself(username,password,server,port)
    url="http://192.168.220.167:6080/arcgis/rest/services/huaweiTest/MapServer/export"
    query_dict={'bbox':'787305.7985999587,817601.0343623444,833196.2452000414,840656.3066949999','layerDefs':{'0':'id<10000'},'format':'image','f':'json'}

    adminself.sendAGSReq(url,query_dict)









