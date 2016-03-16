__author__ = 'jiangmb'
import portalpy

portalUrl="https://jmb.portal.com/arcgis"
user="testtt"
password="teee"
portalAdmin= portalpy.Portal(portalUrl, user, password)

group_id = portalAdmin.create_group('my group', 'test tag', 'a group to share travel maps')
