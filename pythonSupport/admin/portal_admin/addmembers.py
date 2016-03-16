#!/usr/bin/env python
# Requires Python 2.7+

# Demonstrates how to add users to Portal for ArcGIS in bulk

# For Http calls
import httplib, urllib2, urllib, json
# For system tools
import sys, os
# For reading passwords without echoing
import getpass
# Other utilities
import Queue

# Defines the entry point into the script
def main(argv):

    print "This script adds users in bulk into a portal. \n"

    #Get parameters
    parameters = getParametersFromUser ()

    portalURL = parameters['portalURL']
    provider = parameters['provider']
    userName = parameters['userName']
    password = parameters['password']
    inUserFile = parameters['inUserFile']


    #Get user data from file
    usersData = getUserDataFromFile(inUserFile,provider)

    #Create users
    createUsers (userName,password, portalURL,provider, usersData)

    raw_input('Press ENTER to close the script.')

    return


# This function loads all the user data in the input text file into a Python Queue.
# This usersQueue can be later passed to the createUsers function
def getUserDataFromFile(inUserFile,provider):


    usersQ = Queue.Queue()
    keyParams = ['username', 'password', 'email', 'fullname','role','description']

    inFileHandle = open(inUserFile, 'r')
    userCount = 0
    print '...Processing input users file at: ' + inUserFile
    entryCount = 1;
    for line in inFileHandle.readlines():
        userParams = line.split('|')
        userParamDict = {}
        if provider=="webadaptor":
            if len(userParams) == 5:
                for i in range (0,5):
                    userParamDict[keyParams[0]] = userParams[0]  # login
                    userParamDict[keyParams[1]] = ""
                    userParamDict[keyParams[2]] = userParams[1]  # email address
                    userParamDict[keyParams[3]] = userParams[2]  # name
                    userParamDict[keyParams[4]] = userParams[3]  # role
                    userParamDict[keyParams[5]] = userParams[4].replace('\n','')  # description
                usersQ.put (userParamDict)
                userCount = userCount + 1
            else:
                print ' The format for entry %s is invalid.  The format for enterprise accounts should be <login>|<email address>|<name>|<role>|<description>. \n '% (entryCount)

                raise SystemExit( 'When registering enterprise accounts, the format for each entry is as follows: <login>|<email address>|<name>|<role>|<description>')
        elif provider=="arcgis":
            if len(userParams) == 6:
                for i in range (0,6):
                    userParamDict[keyParams[0]] = userParams[0]  # account
                    userParamDict[keyParams[1]] = userParams[1]  # password
                    userParamDict[keyParams[2]] = userParams[2]  # email address
                    userParamDict[keyParams[3]] = userParams[3]  # name
                    userParamDict[keyParams[4]] = userParams[4]  # role
                    userParamDict[keyParams[5]] = userParams[5].replace('\n','')  # description
                usersQ.put (userParamDict)
                userCount = userCount + 1
            else:
                print ' The format for entry %s is invalid.  The format for built-in portal accounts should be <account>|<password>|<email address>|<name>|<role>|<description>.  \n '% (entryCount)
                raise SystemExit( 'When registering built-in portal accounts, the format for each entry is as follows: <account>|<password>|<email address>|<name>|<role>|<description>')
        else:
            print '   The provider is incorrect. Script ended. \n'
            raise SystemExit( 'The value for the user type is invalid. ')
        entryCount = entryCount +1
        if not ((userParamDict[keyParams[4]].lower()== "user") or (userParamDict[keyParams[4]].lower()=="publisher") or (userParamDict[keyParams[4]].lower()== "admin")):
            raise SystemExit( 'The value for the user role %s in users text file is invalid.  Accepted values are user or publisher or admin. ' % (userParamDict[keyParams[4]]))
    inFileHandle.close()
    # Create users and report results
    print '...Total members to be added: ' + str(userCount)

    return usersQ


# This function connects to the portal and adds members to it from a collection
def createUsers(username,password, portalUrl, provider,userParamsQ):

    print '...Connecting to ' + portalUrl
    token = generateToken(username,password, portalUrl)
    print '...Adding users '
    usersLeftInQueue = True
    while usersLeftInQueue:
        try:
            userDict = userParamsQ.get(False)
            userDict['f'] = 'json'
            userDict['token'] = token
            userDict['provider'] = provider
            params = urllib.urlencode(userDict)

            request = urllib2.Request(portalUrl + '/portaladmin/security/users/createUser?',params, { 'Referer' : portalUrl })

            # POST the create request
            response = urllib2.urlopen(request).read()
            responseJSON = json.loads(response)

            # Log results
            if responseJSON.has_key('error'):
                errDict = responseJSON['error']
                if int(errDict['code'])==498:
                    message = 'Token Expired. Getting new token... Username: ' + userDict['username'] + ' will be added later'
                    token = generateToken(username,password, portalUrl)
                    userParamsQ.put(userDict)
                else:
                    message =  'Error Code: %s \n Message: %s' % (errDict['code'],
                    errDict['message'])
                print '\n' + message
            else:
                # Success
                if responseJSON.has_key('status'):
                    resultStatus = responseJSON['status']
                    print '\n' + 'User: %s account created' % (userDict['username'])

                    print 'User: %s account created' % (userDict['username'])
        except Queue.Empty:
              usersLeftInQueue = False

# This function gets a token from the portal
def generateToken(username, password, portalUrl):
    '''Retrieves a token to be used with API requests.'''
    parameters = urllib.urlencode({'username' : username,
                                   'password' : password,
                                   'client' : 'referer',
                                   'referer': portalUrl,
                                   'expiration': 60,
                                   'f' : 'json'})
    try:
        response = urllib.urlopen(portalUrl + '/sharing/rest/generateToken?',
                              parameters).read()
    except Exception as e:
        raise SystemExit( 'Unable to open the url %s/sharing/rest/generateToken' % (portalUrl))
    responseJSON =  json.loads(response.strip(' \t\n\r'))
    # Log results
    if responseJSON.has_key('error'):
        errDict = responseJSON['error']
        if int(errDict['code'])==498:
            message = 'Token Expired. Getting new token... '
            token = generateToken(username,password, portalUrl)
        else:
            message =  'Error Code: %s \n Message: %s' % (errDict['code'],
            errDict['message'])
            raise SystemExit(message)
    token = responseJSON.get('token')
    return token

# This function gets gets parameters from the user in interactive mode
def getParametersFromUser():

    parameters = {}
    # Get Location of users file
    inUserFile = raw_input ("Enter path to users text file: ")
    if not os.path.exists(inUserFile):
        print '   File does not exist. Script ended. \n'
        raise SystemExit( 'Input file: %s does not exist' % (inUserFile))
    parameters['inUserFile'] = inUserFile

    # Enteprise logins or built-in accounts?
    userInput = raw_input ("What type of users do you want to add to the portal?  Accepted values are built-in or enterprise: ")
    if userInput.lower()=="built-in":
        parameters['provider'] = 'arcgis'
        print '   Built-in accounts will be added to the portal. \n'
    elif userInput.lower()=="enterprise":
        parameters['provider'] = 'webadaptor'
        print '   Enterprise accounts will be added to the portal. \n'
    else:
        print '   The type of users is incorrect. Script ended. \n'
        raise SystemExit( 'The value entered for the user type %s is invalid.  Accepted values are built-in or enterprise. ' % (userInput))

    # Get Portal URL
    hostname = raw_input("Enter the fully qualified portal hostname (for example myportal.acme.com): ")
    parameters['portalURL'] = 'https://' + hostname + ':7443/arcgis'
    print '   Users will be added to portal at: ' + parameters['portalURL'] + '\n'

    # Get a username and password with portal administrative privileges
    parameters['userName'] = raw_input("Enter a built-in user name with portal administrative privileges:")

    parameters['password'] = raw_input("Enter password: ")
    print '\n'

    return  parameters

# Script start
if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))