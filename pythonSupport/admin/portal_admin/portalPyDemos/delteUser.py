__author__ = 'jiangmb'
import portalpy
portalUrl=""
admin_user=""
admin_password=""
portal= portalpy.Portal(portalUrl, admin_user, admin_password)
portal.delete_user('amy.user', True, 'bob.user')