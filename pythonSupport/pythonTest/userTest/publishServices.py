import arcpy, sys, os

# input variables
mxd_path = arcpy.GetParameterAsText(0)
service_name = arcpy.GetParameterAsText(1)
connection_file_path = arcpy.GetParameterAsText(2)
folder_name = arcpy.GetParameterAsText(3)
replace_workspace_path = arcpy.GetParameterAsText(4)

# mxd_path =r"D:\workspace\www.mxd"
# service_name =r"wo"
# connection_file_path =r"D:\workspace\www.ags"
# folder_name ="map"
# replace_workspace_path =r"D:\workspace\www.sde"
def main(argv=None):
    try:
        print "[INFO]:mxd_path" + mxd_path + "\n\n"
        print "[INFO]:service_name" + service_name + "\n\n"
        print "[INFO]:connection_file_path" + connection_file_path + "\n\n"
        print "[INFO]:folder_name" + folder_name + "\n\n"
        print "[INFO]:replace_workspace_path" + replace_workspace_path + "\n\n"
        arcpy.AddMessage("mxd_path:" + mxd_path)
        arcpy.AddMessage("service_name:" + service_name)
        arcpy.AddMessage("connection_file_path:" + connection_file_path)
        arcpy.AddMessage("folder_name:" + folder_name)
        arcpy.AddMessage("replace_workspace_path:" + replace_workspace_path)
        scratchFolder = arcpy.env.scratchFolder
        
        new_mxd_path = os.path.join(scratchFolder, service_name + '_new.mxd')
        out_sddraft = os.path.join(scratchFolder, service_name + '.sddraft')
        out_service_definition = os.path.join(scratchFolder, service_name + '.sd')
        print "[INFO]:new_mxd_path" + new_mxd_path + "\n\n"
        print "[INFO]:out_sddraft" + out_sddraft + "\n\n"
        print "[INFO]:out_service_definition" + out_service_definition + "\n\n"
        arcpy.AddMessage("new_mxd_path:" + new_mxd_path)
        arcpy.AddMessage("out_sddraft:" + out_sddraft)
        arcpy.AddMessage("out_service_definition:" + out_service_definition)
        
        map_document = arcpy.mapping.MapDocument(mxd_path)

        find_workspace_path = arcpy.mapping.ListLayers(map_document)[0].workspacePath       
        map_document.replaceWorkspaces(find_workspace_path, "FILEGDB_WORKSPACE", replace_workspace_path, "SDE_WORKSPACE", False)
        map_document.saveACopy(new_mxd_path)       
        del map_document
        
        new_map_document = arcpy.mapping.MapDocument(new_mxd_path)
        analysis = arcpy.mapping.CreateMapSDDraft(new_map_document,
                                                  out_sddraft,
                                                  service_name,
                                                  'ARCGIS_SERVER',
                                                  connection_file_path,
                                                  True,
                                                  folder_name,
                                                  None,
                                                  None)
        if analysis['errors'] == {}:
            if os.path.exists(out_service_definition):
                os.remove(out_service_definition)
            result=arcpy.StageService_server(out_sddraft, out_service_definition)


            result2= arcpy.UploadServiceDefinition_server(out_service_definition, connection_file_path)

        else:
            arcpy.AddError("[ERROR]:" + analysis['errors'] + "\n\n")
    except Exception,e:
        print "[ERROR]:" + arcpy.GetMessages() + "\n\n"
        print e
        arcpy.AddError("[ERROR]:" + arcpy.GetMessages() + "\n\n")


if __name__ == "__main__" :
    sys.exit(main(sys.argv[1:]))
