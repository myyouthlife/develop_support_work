__author__ = 'jiangmb'
#!/usr/bin/python
import portalpy
url = "https://jmb.portal.com/arcgis"
portal = portalpy.Portal(url)
print portal.get_version()