# -*- coding: utf-8 -*-
import arcpy
import os
import sys
import arceditor 

# def PublishMap(wrkspc,mxdName,summary,con,service,tags,fileName):
mxdName=r"test0914.mxd"
wrkspc =r'D:/workspace/'
mapDoc = arcpy.mapping.MapDocument(wrkspc + mxdName)
con="D:/workspace/www.ags"
service = 'testThree'
sddraft = wrkspc + service + '.sddraft'
sd = wrkspc + service + '.sd'
tags = 'county, counties, population, density, census'
summary=''
# create service definition draft
if os.path.exists(sddraft):
    os.remove(sddraft)
try:
    arcpy.mapping.CreateMapSDDraft(mapDoc, sddraft, service, 'ARCGIS_SERVER',con, True, 'test', summary, tags)
except Exception as err:
    print(err[0] + "\n\n")
    sys.exit("Failed to create SD draft")
analysis = arcpy.mapping.AnalyzeForSD(sddraft)
# stage and upload the service if the sddraft analysis did not contain errors
print("The following was returned during analysis of the map service:")
for key in analysis.keys():
    print("---{}---".format(key.upper()))
    for ((message, code), layerlist) in analysis[key].iteritems():
        print(message)
if analysis['errors'] == {}:
    try:
        # Execute StageService
        if os.path.exists(sd):
            os.remove(sd)
        arcpy.StageService_server(sddraft, sd)
        # Execute UploadServiceDefinition
        arcpy.UploadServiceDefinition_server(sd, con)
        print("publish success!")
    except arcpy.ExecuteError:
        print(arcpy.GetMessages() + "\n\n")
        arcpy.AddMessage(arcpy.GetMessages() + "\n\n")
else:
    print("Service was not published because of errors found during analysis.")
    print(analysis['errors'])
		
# wrkspc=arcpy.GetParameterAsText(0)
# mxdName=arcpy.GetParameterAsText(1)
# summary=arcpy.GetParameterAsText(2)
# con=arcpy.GetParameterAsText(3)
# service=arcpy.GetParameterAsText(4)
# tags=arcpy.GetParameterAsText(5)
# fileName=arcpy.GetParameterAsText(6)

# PublishMap(wrkspc,mxdName,summary,con,service,tags,fileName)

      
