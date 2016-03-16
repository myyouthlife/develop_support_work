# -*- coding: utf-8 -*-
import arcpy
def CreateAGS(outdir,out_name,server_url,username,password):
    #outdir = "E:\\englishFile"
    out_folder_path = outdir
    #out_name = 'test.ags'
    #server_url = 'http://localhost:6080/arcgis/admin'
    use_arcgis_desktop_staging_folder = False
    staging_folder_path = outdir
    #username = 'dlwy'
    #password = 'dlwy'
    arcpy.mapping.CreateGISServerConnectionFile("ADMINISTER_GIS_SERVICES",
                                                out_folder_path,
                                                out_name,
                                                server_url,
                                                "ARCGIS_SERVER",
                                                use_arcgis_desktop_staging_folder,
                                                staging_folder_path,
                                                username,
                                                password,
                                                "SAVE_USERNAME")
#参数设置模块    
outdir=arcpy.GetParameterAsText(0)
out_name=arcpy.GetParameterAsText(1)
server_url=arcpy.GetParameterAsText(2)
username=arcpy.GetParameterAsText(3)
password=arcpy.GetParameterAsText(4)
CreateAGS(outdir,out_name,server_url,username,password)
