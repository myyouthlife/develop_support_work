import urllib
import json

__author__ = 'jiangmb'
def generateToken(username, password, portalUrl):
    '''Retrieves a token to be used with API requests.'''
    parameters = urllib.urlencode({'username' : username,
                                   'password' : password,
                                   'client' : 'referer',
                                   'referer': portalUrl,
                                   'expiration': 60,
                                   'f' : 'json'})
    response = urllib.urlopen(portalUrl + '/sharing/rest/generateToken?',
                              parameters).read()
    print response

    try:
        jsonResponse = json.loads(response)
        if 'token' in jsonResponse:
            print jsonResponse['token']
            return jsonResponse['token']

        elif 'error' in jsonResponse:
            print jsonResponse['error']['message']
            for detail in jsonResponse['error']['details']:
                print detail
    except ValueError, e:
        print 'An unspecified error occurred.'
        print e

generateToken("arcgis","Super123","https://jmb.portal.com/arcgis")