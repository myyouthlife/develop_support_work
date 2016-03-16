import portalpy
__author__ = 'jiangmb'

portalUrl=""
admin_user=""
admin_password=""

portal = portalpy.Portal(portalUrl, admin_user, admin_password)

resp = portal.get_group_members('67e1761068b7453693a0c68c92a62e2e')
for user in resp['users']:
   print user
