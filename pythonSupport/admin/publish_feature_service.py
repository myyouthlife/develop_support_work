import arcpy
import os

from xml.etree import ElementTree as ET
def CreateServerConntionFile(url,uname,pwd,out_folder_path,file_name):
        ''''''
        server_url =url+'/admin'
        use_arcgis_desktop_staging_folder = False
        username = uname
        password =pwd
        arcpy.mapping.CreateGISServerConnectionFile("ADMINISTER_GIS_SERVICES",
                                            out_folder_path,
                                            file_name,
                                            server_url,
                                            "ARCGIS_SERVER",
                                            use_arcgis_desktop_staging_folder,
                                            out_folder_path,
                                            username,
                                            password,
                                            "SAVE_USERNAME")

        print "create successfully"
def publishService(mapDocPath,con,serviceName):

    mapDoc=arcpy.mapping.MapDocument(mapDocPath)

    sd=os.path.splitext(mapDocPath)[0]+".sd"
    sddraft=os.path.splitext(mapDocPath)[0]+".sddraft"
    # check the sd file exits or not
    if os.path.exists(sd):
        os.remove(sd)
    result= arcpy.mapping.CreateMapSDDraft(mapDoc, sddraft, serviceName, 'ARCGIS_SERVER', con, True, None)

    if not result['errors']=={}:
        print result['errors']

    new_sddraft=modifySddraft(result)


# analyze the service definition draft
    analysis = arcpy.mapping.AnalyzeForSD(new_sddraft)
# stage and upload the service if the sddraft analysis did not contain errors
    if analysis['errors'] == {}:
    # Execute StageService
        arcpy.StageService_server(new_sddraft, sd)
    # Execute UploadServiceDefinition
        arcpy.UploadServiceDefinition_server(sd, con)
    else:
    # if the sddraft analysis contained errors, display them
        print "failed"

def modifySddraft(sddraft):
       doc = ET.parse(sddraft)
       root_elem = doc.getroot()
       if root_elem.tag != "SVCManifest":
        raise ValueError("Root tag is incorrect. Is {} a .sddraft file?".format(sddraft))
        for config in doc.findall("./Configurations/SVCConfiguration/TypeName"):
            if config.text == "MapServer":
                config.text = "FeatureServer"
        # Turn off caching
        for prop in doc.findall("./Configurations/SVCConfiguration/Definition/" +
                                "ConfigurationProperties/PropertyArray/" +
                                "PropertySetProperty"):
            if prop.find("Key").text == 'isCached':
                prop.find("Value").text = "false"
            if prop.find("Key").text == 'maxRecordCount':
                prop.find("Value").text = maxRecords

    # Turn on feature access capabilities
        for prop in doc.findall("./Configurations/SVCConfiguration/Definition/Info/PropertyArray/PropertySetProperty"):
            if prop.find("Key").text == 'WebCapabilities':
                prop.find("Value").text = "Query,Create,Update,Delete,Uploads,Editing"

        # Add the namespaces which get stripped, back into the .SD
        root_elem.attrib["xmlns:typens"] = 'http://www.esri.com/schemas/ArcGIS/10.1'
        root_elem.attrib["xmlns:xs"] = 'http://www.w3.org/2001/XMLSchema'

        newSDdraft=sddraft
        # Write the new draft to disk
        with open(newSDdraft, 'w') as f:
            doc.write(f, 'utf-8')
        return newSDdraft

if __name__=="__main__":
    url="http://localhost:6080/arcgis"
    username="arcgis"
    pwd="Super123"
    out_folder_path=r"\\jiangmb\workspace"
    file_name="localhost.ags"
    mapDocPath=r"\\jiangmb\workspac\test.mxd"
    serviceName="autoFeatureService"
    con=os.path.join(out_folder_path,file_name)
    CreateServerConntionFile(url,username,pwd,out_folder_path,file_name)
    publishService(mapDocPath,con,serviceName)