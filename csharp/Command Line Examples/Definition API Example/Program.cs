using Scopos.BabelFish.APIClients;
using Scopos.BabelFish.DataModel.Definitions;
using Scopos.BabelFish.Runtime;


//You may use GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33 as a x-api-key to start working with our API.
//However, this api key is limited in its use, and should not be used in any real application.
Initializer.Initialize( "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33", false );
DefinitionAPIClient.LocalStoreDirectory = new System.IO.DirectoryInfo( @"C:\temp" );

//A SetName uniquely ideentifies a definition
var threePositionCourseOfFireSetName = SetName.Parse( "v3.0:ntparc:Three-Position Air Rifle 3x20" );

//Retreives the COURSE OF FIRE definition.
var threePositionCourseOfFire = await DefinitionCache.GetCourseOfFireDefinitionAsync( threePositionCourseOfFireSetName );

//Print basic information about this definition
Console.WriteLine( threePositionCourseOfFire.CommonName );
Console.WriteLine( threePositionCourseOfFire.Description );
Console.WriteLine( threePositionCourseOfFire.Discipline );
Console.WriteLine( threePositionCourseOfFire.Subdiscipline );
Console.WriteLine();

/*
3x20 Air Rifle
Three-Position Air Rifle 3x20, position ordering for K-P-S
RIFLE
Three-Position Air Rifle
*/

// Build the Event Tree which is the structure of the course of fire.
// Event Trees (using EventComposite) are much more easily traversed than the raw CourseOfFire definition, and also have convenient methods to get all events of a certain type, etc.
var topLevelEvent = EventComposite.GrowEventTree( threePositionCourseOfFire );

//Print out the stages to the COF
foreach (var stage in topLevelEvent.GetEvents( EventtType.STAGE )) {
    Console.WriteLine( $"{stage.EventName} has {stage.GetAllSingulars().Count} number of shots." );

    //Load the recommended RESULT LIST FORMAT definition
    var resultListFormatSetName = SetName.Parse( stage.ResultListFormatDef );
    var resultListFormat = await DefinitionCache.GetResultListFormatDefinitionAsync( resultListFormatSetName );
    Console.WriteLine( $"The recommended RESULT LIST FORMAT is '{resultListFormat.CommonName}' which has a total of {resultListFormat.Format.Columns.Count} columns." );

    //Load the default RANKING RULE
    var rankingRuleSetName = SetName.Parse( stage.RankingRuleMapping["DefaultDef"] );
    var rankingRule = await DefinitionCache.GetRankingRuleDefinitionAsync( rankingRuleSetName );
    Console.WriteLine( $"The recommended RANKING RULE for this event is '{rankingRule.CommonName}' which defines {rankingRule.RankingRules[0].Rules.Count} rules." );
}

/*
    Kneeling has 20 number of shots.
    The recommended RESULT LIST FORMAT is 'Kneeling 20 Shots' which has a total of 5 columns.
    The recommended RANKING RULE for this event is 'Generic Decimal Kneeling' which defines 3 rules.
    Prone has 20 number of shots.
    The recommended RESULT LIST FORMAT is 'Prone 20 Shots' which has a total of 5 columns.
    The recommended RANKING RULE for this event is 'Generic Decimal Prone' which defines 3 rules.
    Standing has 20 number of shots.
    The recommended RESULT LIST FORMAT is 'Standing 20 Shots' which has a total of 5 columns.
    The recommended RANKING RULE for this event is 'Generic Decimal Standing' which defines 3 rules.
*/

Console.WriteLine();
// Print out each range command from the first avaliable RangeScript.
foreach (var sg in threePositionCourseOfFire.RangeScripts[0].SegmentGroups) {
    foreach (var command in sg.Commands) {
        Console.WriteLine( command.Command );
    }
}

/*
    Welcome to the {MatchName} Three-Position Air Rifle Match
    Relay Number {RelayNumber} you may move your rifles and equipment to the firing line.
    You may uncase and handle your rifles.
    Take your positions.
    Your 8 minute preparation and sighting time for the kneeling position starts when your green signal light appears and ends when your red light reappears.
    Sighting shots ... START
    START
    30 seconds
    Sighting shots ... STOP
    Your 20 minute for 20 shots kneeling match firing time starts when your green signal light appears and ends when your red light reappears.
    Kneeling match firing ... START
    START
    Five minutes.
    Two minutes.
    STOP - UNLOAD
    Is the line clear?
    The line is clear
    Your 5 minute changeover time for the prone position begins now.
    Take your positions.
    Your 5 minute sighting time for the prone position starts when your green signal light appears and ends when your red light reappears.
    Sighting shots ... START
    START
    30 seconds
    Sighting shots ... STOP
    Your 20 minute for 20 shots prone match firing time starts when your green signal light appears and ends when your red light reappears.
    Prone match firing ... START
    START
    Five minutes.
    Two minutes.
    STOP - UNLOAD
    Is the line clear?
    The line is clear
    Your 5 minute changeover time for the Standing position begins now.
    Take your positions.
    Your 5 minute sighting time for the standing position starts when your green signal light appears and ends when your red light reappears.
    Sighting shots ... START
    START
    30 seconds
    Sighting shots ... STOP
    Your 25 minute for 20 shots standing match firing time starts when your green signal light appears and ends when your red light reappears.
    Standing match firing ... START
    START
    Five minutes.
    Two minutes.
    STOP - UNLOAD
    Is the line clear?
    The line is clear
    Athletes, you may remove your equipment from the firing line
    You may discharge air or gas downrange.
*/

Console.WriteLine( "Press any key to close." );
Console.ReadKey();
