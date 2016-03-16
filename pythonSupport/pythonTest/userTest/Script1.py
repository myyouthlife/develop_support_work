# -*- coding: utf-8 -*-
import arcpy

def PublishMap(wrkspc,mxdName,summary,con,service,tags):
    #wrkspc = 'E:/englishFileTwo/'
    mapDoc = arcpy.mapping.MapDocument(wrkspc + mxdName)
    #con="E:\\englishFileThree\\arcgis on localhost_6080 (publisher).ags"
    #service = 'testThree'
    sddraft = wrkspc + service + '.sddraft'
    sd = wrkspc + service + '.sd'
    #tags = 'county, counties, population, density, census'
    #summary=''
    # create service definition draft
    analysis = arcpy.mapping.CreateMapSDDraft(mapDoc, sddraft, service, 'ARCGIS_SERVER', 
                                          con, True, None, summary, tags)
    # stage and upload the service if the sddraft analysis did not contain errors
    if analysis['errors'] == {}:
        # Execute StageService
        arcpy.StageService_server(sddraft, sd)
        # Execute UploadServiceDefinition
        arcpy.UploadServiceDefinition_server(sd, con)
        
wrkspc=arcpy.GetParameterAsText(0)
mxdName=arcpy.GetParameterAsText(1)
summary=arcpy.GetParameterAsText(2)
con=arcpy.GetParameterAsText(3)
service=arcpy.GetParameterAsText(4)
tags=arcpy.GetParameterAsText(5)

PublishMap(wrkspc,mxdName,summary,con,service,tags)

      
