# -*- coding: utf-8 -*-
import arcpy
import os
import sys
import arceditor 

def AnalysisMap(wrkspc,mxdName,summary,con,service,tags,fileName):
    #wrkspc = 'E:/englishFileTwo/'
    mapDoc = arcpy.mapping.MapDocument(wrkspc + mxdName)
    #con="E:\\englishFileThree\\arcgis on localhost_6080 (publisher).ags"
    #service = 'testThree'
    sddraft = wrkspc + service + '.sddraft'
    sd = wrkspc + service + '.sd'
    #tags = 'county, counties, population, density, census'
    #summary=''
    # create service definition draft
    if os.path.exists(sddraft):
        os.remove(sddraft)
    analysis = arcpy.mapping.CreateMapSDDraft(mapDoc, sddraft, service, 'ARCGIS_SERVER', 
                                          con, True, fileName, summary, tags)
    print("The following was returned during analysis of the image service:")
    for key in analysis.keys():
        print("---{}---".format(key.upper()))
        for ((message, code), layerlist) in analysis[key].iteritems():
            print(message)
            print("applies to:{}".format("".join([layer.name for layer in layerlist])))
    # Stage and upload the service if the sddraft analysis did not contain errors
    if analysis['errors'] != {}:
        print("Service was not published because of errors found during analysis.")
        print(analysis['errors'])
	if os.path.exists(sddraft):
            os.remove(sddraft) 
                
        
wrkspc=str(sys.argv[1])
mxdName=str(sys.argv[2])
summary=str(sys.argv[3])
con=str(sys.argv[4])
service=str(sys.argv[5])
tags=str(sys.argv[6])
fileName=str(sys.argv[7])

AnalysisMap(wrkspc,mxdName,summary,con,service,tags,fileName)

      
