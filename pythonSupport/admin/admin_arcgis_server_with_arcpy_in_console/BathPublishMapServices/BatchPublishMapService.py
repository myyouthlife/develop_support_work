#-*-coding:cp936  -*-

__author__ = 'jiangmb'
import time


def GetMxFileList(filePath):
        #�ж��ļ����Ƿ����
    if not os.path.exists(filePath):
        print "++++++++ERROR:�ļ��в�����+++++++"
        sys.exit(1)
    #��ȡ�ļ����е�����mxd�ļ�
    list=[]
    for root,dirname, files in os.walk(filePath):

             for file in files:

                if os.path.splitext(file)[1]=='.mxd':
                    mxdfile=os.path.join(root,file)

                    list.append(mxdfile)

    if list==[]:
      print "++++++++INFO:�ڵ�ǰĿ¼�²�������Ч��mxd�ļ�++++++++"
      time.sleep(5)
      sys.exit(1)
    return list
def GetInfo():

    server = raw_input("������GIS Server IP:")
    userName=raw_input("������վ�����Ա�û���:")
    passWord=getpass.getpass("������վ�����Ա����:")



    print "++++++++INFO:��ʼ����server�������ļ�++++++++"
    """
    logDict={'server':'localhost',
        'userName':"arcgis",
             'passWord':"Super123"}
    dd=CreateContectionFile()
    dd.loginInfo=logDict
    dd.filePath="d:/dd.ags"
    dd.CreateContectionFile()
    """
    logDict={'server':server,
            'userName':userName,
                 'passWord':passWord}

    contionfile= os.path.split(sys.argv[0])[0]+'\\'+server+".ags"

    #���ô��������ļ��Ĳ���
    instace=CreateContectionFile()
    instace.filePath=contionfile
    instace.loginInfo=logDict
    instace.CreateContectionFile()

    if(os.path.isfile(contionfile)==False):
        print "++++++++ERROR:��������ʧ��++++++++"
        time.sleep(5)
        sys.exit(1)

    #����mxd�ļ����ļ���e
    mxdDir=raw_input('������mxd�����ļ���:')
    fileList=GetMxFileList(mxdDir)

    servic_dir=raw_input("��ָ��������������Ŀ¼��Ĭ��Ϊroot��ʹ��Ĭ��ֱֵ�ӻس�:")
    if len(servic_dir)==0:
        servic_dir==None
    clusterName=raw_input("��ָ����������Ⱥ��Ĭ��Ϊcluster����û�м�Ⱥ��������ֱ�ӻس�:")
    if len(clusterName)==0:
        clusterName='default'
    clsPublishservice=publishServices()
    clsPublishservice.publishServices(fileList,contionfile,clusterName,copy_data_to_server=False,folder=servic_dir)
if __name__=='__main__':
    GetInfo()
    # clsPublishservice=publishServices()
    # fileList=['d:\\workspace\\testCopy.mxd', 'd:\\workspace\\test.mxd']
    # contionfile=r"d:\localhost.ags"
    # clusterName='default'
    # servic_dir='test3'