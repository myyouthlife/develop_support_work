#coding:utf-8
import urllib
import arcpy
# import couchdb
#
# couch = couchdb.Server('http://192.168.142.141:29080/')
# db=couch.create('test')
# db = couchdb.Database("http://your.url/yourdb")
# db.resource.http.add_credentials(username, password)
parameters = urllib.urlencode({'item': "我去",
                               'text':"你大爷",
                               'overwrite': 'false',
                               'thumbnailurl': "ddd",
                               'token' : "ni",
                               'f' : 'json'})

print parameters