__author__ = 'jiangmb'

from Get_site_information_helper import *




username="arcgis"
password="Super123"
server="192.168.220.64"
port="6080"
serviceName="poi2014.MapServer"
roleName="testB"
adminself=ADMINself(username,password,server,port)
adminself.addPermission(serviceName,roleName)
