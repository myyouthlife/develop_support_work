#coding:cp936
__author__ = 'jiangmb'
from Get_site_information_helper import *
import datetime


# server=raw_input("������GIS��������Ip:")
# port=raw_input("������GIS���ӵĶ˿ںţ�Ĭ��Ϊ6080������Ĭ��ֵ��ֱ�ӻس���")
# if port=='':
#     port='6080'
# username=raw_input("������վ�����Ա�û���:")
# password=raw_input("���������Ա����:")
# path=raw_input("���������ļ������ַ:")


server="localhost"
port='6080'
username='arcgis'
password='Super123'
path='d:\\'


def writeOutPut(path,contents):
    file=open(path,'w')
    file.write("������,��ʼ��ʱ��(ms)\n")
    for content in contents:
      line=content[0].encode('utf-8')+","+str(content[1]).encode('utf-8')+"\n"
      file.write(line)

    file.close()

adminself=ADMINself(username,password,server,port)
serviceList=adminself.getStartedOrStopedServiceList()
if (len(serviceList)==0):
    print "++++++++��ǰ������û�з�����Գ�ʼ��:+++++++"
    sys.exit(1)
Dict_service={}
for singleService in serviceList:
    newname=singleService.replace('.','/')
    if "//" in newname:
       newname=newname.replace('//','/')
    url="http://{}:{}/arcgis/rest/services/{}".format(server,port,newname)

    timeStart=datetime.datetime.now()
    results= adminself.sendAGSReq(url+adminself.basicQ,"")
    if results is None:
        print "++++++++"+singleService+" has an error in initialization+++++++"
    else:
        timeEnd=datetime.datetime.now()
        timeElapse=(timeEnd-timeStart).microseconds/1000
        print "++++++++"+singleService+" spent:"+str(timeElapse)+" ms in initialization++++++++"
        Dict_service[singleService]=timeElapse

#���ֵ������
results=sorted(Dict_service.items(), key=lambda Dict_service:Dict_service[1])
writeOutPut(path+"\\result.txt",results)
