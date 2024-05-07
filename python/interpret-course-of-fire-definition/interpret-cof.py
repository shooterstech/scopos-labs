import boto3, os, json
import argparse
import requests
from ValueSeries import ValueSeries

"""
This script demonstrates how to read and interpret a Course of Fire definition. 
It outputs, to the command line, human readable values from the Course of Fire definition, including the Range Scripts.

Usage:
python ./interpret-cof.py -h
Course of Fire definitions are defined at https://support.scopos.tech/index.html?definition-course-of-fire.html
"""
ap = argparse.ArgumentParser()
ap.add_argument("-c", "--cof", required = False, type=str, help = "The Course of Fire Definition SetName to try and interpret. Defaults to 'v3.0:ntparc:Three-Position Air Rifle 3x10'", default = "v3.0:ntparc:Three-Position Air Rifle 3x10")
ap.add_argument("--range-scripts", required = False, action="store_true", help = "Prints a list of the Range Script names and indexes.")
ap.add_argument("--print", required = False, action="store_true", help="Prints the Course of Fire Definition to screen.")
ap.add_argument("--singularities", required = False, action="store_true", help="Prints information about the expected singularities.")
ap.add_argument("--range-script", required = False, type=int, default=-1, help="Prints Segment Group, Segments, and Command information about the specified Range Script, identified by its index.")

args = vars(ap.parse_args())
cofSetName = args["cof"]

#This is a shared API Key that is rate limited. Visit www.scopos.tech/restapi to learn how to obtain your own.
xApiKey = "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33"

def downloadCourseOfFire() :
    global cofDefinition

    #GetCoffee is documented at https://app.swaggerhub.com/apis-docs/Shooters-Technology/api/1.5.4.0#/Scopos%20Data/GetCoffee
    definitionUrl = "https://api.scopos.tech/definition/COURSE OF FIRE/{}".format( cofSetName )

    #All Scopos' Rest API calls require the x-api-key included in the header
    headers = {'x-api-key' : xApiKey }

    #Make the call
    getDefinitionResponse = requests.get( definitionUrl, headers=headers )

    #check that the status is OK
    if (getDefinitionResponse.ok) :

        #All Scopos Rest API calls return json. Use .loads to read it into a python dictionary.
        responseJson = json.loads( getDefinitionResponse.content )
        cofDefinition = responseJson[cofSetName]

    else :
        raise Exception( "Could not download Course of Fire definition. Received error {}.".format( getDefinitionResponse.reason ) )

def printJson() :
    """
    Prints the Course of Fire Definition to screen.
    """

    print( json.dumps( cofDefinition, indent=4 ) )

    print()

def printRangeScripts() :
    """
    Prints a list of the Range Script names and indexes.
    """

    commonName = cofDefinition.get( "CommonName", "Name not found" )
    rangeScripts = cofDefinition.get("RangeScripts", [])
    print( "Course of Fire Definition '{}'' has {} Range Scripts.".format( commonName, len( rangeScripts ) ) )

    for rangeScriptIndex in range( len( rangeScripts ) ) :
        rangeScript = rangeScripts[rangeScriptIndex]
        rangeScriptName = rangeScript.get( "RangeScriptName", "Name not found" )
        designedForPaper = "designed for paper" if rangeScript.get( "DesignedForPaper", False ) else "not designed for paper"
        designedForEST = "designed for EST" if rangeScript.get( "DesignedForEST", False ) else "not designed for EST"

        print( "  '{}'' is index {}, {}, {}.".format( rangeScriptName, rangeScriptIndex, designedForEST, designedForPaper ) )

    print()

def printSingularties() :
    """
    Prints information about the defined singularity events.
    Singularities almost always represent single shots that are fired in a match for record fire. 
    """

    singulars = cofDefinition.get( "Singulars", [] )
    for singular in singulars :
        #All shots fired within an Event of EventType==STAGE, should be fired on shots with the same Stage Label.
        #Stage Labels are commonly defined by a single character. 
        stageLabel = singular["StageLabel"]
        eventNameTemplate = singular["EventName"]
        vs = ValueSeries( singular["Values"] if "Values" in singular else "" )
        print( "  Stage Label '{}' has Event singularities: ".format( stageLabel ) )
        for s in [ eventNameTemplate.format(x) for x in vs.GetAsList() ] :
          print( "    {}".format( s ) )

    print()

def getRangeScriptText( parameter, dictionary, parentDictionary, grandParentDictionary ):
    """
    Most text values in a range script follow the Value Inheritance Rules.
    Which means if (for example) a Command doesn't havve a parameter value, it will look to the 
    default segment group command. And if that doesn't have a value, then it looks to the default
    range script command. And if that doesn't have a value, then a default value is returned.
    https://support.scopos.tech/index.html?segment-and-command-value-inhe.html
    """
    value = dictionary.get(parameter, None)
    if value:
        return value

    value = parentDictionary.get( parameter, None)
    if value:
        return value

    value = grandParentDictionary.get( parameter, "" )
    return value

def getRangeScriptNumber( parameter, dictionary, parentDictionary, grandParentDictionary ):
    """
    Most number values in a range script follow the Value Inheritance Rules.
    Which means if (for example) a Command doesn't havve a parameter value, it will look to the 
    default segment group command. And if that doesn't have a value, then it looks to the default
    range script command. And if that doesn't have a value, then a default value is returned.
    https://support.scopos.tech/index.html?segment-and-command-value-inhe.html
    """
    value = dictionary.get(parameter, None)
    if value != -9999:
        return value

    value = parentDictionary.get( parameter, None)
    if value!= -9999:
        return value

    value = grandParentDictionary.get( parameter, 0 )
    return value

def getRangeScriptLight( parameter, dictionary, parentDictionary, grandParentDictionary ):
    """
    Most Light values in a range script follow the Value Inheritance Rules.
    Which means if (for example) a Command doesn't havve a parameter value, it will look to the 
    default segment group command. And if that doesn't have a value, then it looks to the default
    range script command. And if that doesn't have a value, then a default value is returned.
    https://support.scopos.tech/index.html?segment-and-command-value-inhe.html
    """
    value = dictionary.get(parameter, "NONE")
    if value != "NONE":
        return value

    value = parentDictionary.get( parameter, "NONE")
    if value != "NONE":
        return value

    value = grandParentDictionary.get( parameter, "NONE" )
    return value

def getRangeScriptTimerCommand( parameter, dictionary, parentDictionary, grandParentDictionary ):
    #Timer commands do not follow the Value Inheritance Rules
    value = dictionary.get(parameter, "NONE")
    return value

def getRangeScriptTimer( parameter, dictionary, parentDictionary, grandParentDictionary ):
    #Timer values do not follow the Value Inheritance Rules
    value = dictionary.get(parameter, "")
    return value

def getRangeScriptList( parameter, dictionary, parentDictionary, grandParentDictionary ) :
    """
    Lists are accumulative, they get all of the values from the (for example) command,
    default segment group command, and default range script command
    """
    valueList = dictionary.get( parameter, [] )

    for parentValue in parentDictionary.get( parameter, [] ) :
        if parentValue not in valueList :
            valueList.append( parentValue )

    for grandParentValue in grandParentDictionary.get( parameter, [] ) :
        if grandParentValue not in valueList :
            valueList.append( grandParentValue )

    return valueList


def printSegmentGroups( rangeScriptIndex ) :
    rangeScripts = cofDefinition.get("RangeScripts", [])
    rangeScript = rangeScripts[rangeScriptIndex]
    rangeScriptName = rangeScript.get( "RangeScriptName", "Name not found" )
    print( "Range Script '{}'".format( rangeScriptName ) )

    #Each Range Script has default values for each Command and Segment
    rsDefaultCommand = rangeScript.get("DefaultCommand", {})
    rsDefaultSegment = rangeScript.get("DefaultSegment", {})

    #Range Scripts are composed of a list of SegmentGroups.
    #Each SegmentGroup has a list of Commands and a list of Segments.
    #The sequence of Commands are controlled by the range officer.
    #The sequence of Segments are controlled (directly or indirectly) by the athlete.

    for segmentGroup in rangeScript.get("SegmentGroups", {}) :
        segmentGroupName = segmentGroup["SegmentGroupName"]
        print( "  Segment Group {}".format( segmentGroupName ) )

        #Each Segment Group also has it's own default values for Command and Segment
        sgDefaultCommand = segmentGroup.get( "DefaultCommand", {})
        sgDefaultSegment = segmentGroup.get( "DefaultSegment", {})

        for command in segmentGroup.get("Commands", []) :
            commandText = getRangeScriptText( "Command", command, sgDefaultCommand, rsDefaultCommand )
            notes = getRangeScriptText( "Notes", command, sgDefaultCommand, rsDefaultCommand )
            shotAttributes = getRangeScriptList( "ShotAttributes", command, sgDefaultCommand, rsDefaultCommand )
            timer = getRangeScriptTimer( "Timer", command, sgDefaultCommand, rsDefaultCommand )
            timerCommand = getRangeScriptTimerCommand( "TimerCommand", command, sgDefaultCommand, rsDefaultCommand )
            occuresAt = getRangeScriptTimer( "OccursAt", command, sgDefaultCommand, rsDefaultCommand )
            redLight = getRangeScriptLight( "RedLight", command, sgDefaultCommand, rsDefaultCommand )
            greenLight = getRangeScriptLight( "GreenLight", command, sgDefaultCommand, rsDefaultCommand )
            targetLight = getRangeScriptLight( "TargetLight", command, sgDefaultCommand, rsDefaultCommand )
            print( "   Command '{}'".format( commandText ) )
            if notes :
                print( "     Notes: '{}'".format( notes ) )
            if occuresAt :
                print( "     Automated command occuring at {}".format( occuresAt ) )
            if shotAttributes :
                print( "     Attributes to apply to each shot: {}".format( shotAttributes ))
            if timer :
                print( "     Set Range Timer to {}".format( timer ) )
            if timerCommand != "NONE" :
                print( "     Range Timer {} ".format( timerCommand ) )
            if redLight != "NONE" or greenLight != "NONE" or targetLight != "NONE" :
                print( "     {}, {}, {}".format( "Red X " + redLight, "Green O "  + greenLight, "Target Light " + targetLight ) )
            print()


        for segment in segmentGroup.get( "Segments", []) :
            segmentName = getRangeScriptText( "SegmentName", segment, sgDefaultSegment, sgDefaultSegment )
            stageLabel = getRangeScriptText( "StageLabel", segment, sgDefaultSegment, sgDefaultSegment )
            numberOfShots = getRangeScriptNumber( "NumberOfShots", segment, sgDefaultSegment, sgDefaultSegment )
            shotAttributes = getRangeScriptList( "ShotAttributes", segment, sgDefaultSegment, sgDefaultSegment )
            print( "   Segment Name '{}'".format( segmentName ) )
            print( "     Expecting {} number of shots on Stage Label '{}'. ".format( numberOfShots if numberOfShots >= 0 else "unlimited", stageLabel ) )
            if shotAttributes :
                print( "     Attributes to apply to each shot: {}".format( shotAttributes ))
            print()


downloadCourseOfFire()

print( args )
if args["range_scripts"] :
    printRangeScripts()

if args["range_script"] >= 0:
    printSegmentGroups( args["range_script"] )

if args["singularities"] :
    printSingularties() 

if args["print"] :
    printJson()