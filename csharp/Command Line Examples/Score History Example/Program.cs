using Scopos.BabelFish.APIClients;
using Scopos.BabelFish.DataModel.Definitions;
using Scopos.BabelFish.DataModel.ScoreHistory;
using Scopos.BabelFish.Helpers;
using Scopos.BabelFish.Requests.ScoreHistoryAPI;
using Scopos.BabelFish.Responses.ScoreHistoryAPI;
using Scopos.BabelFish.Runtime;
using Scopos.BabelFish.Runtime.Authentication;

// You may use GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33 as a x-api-key to start working with our API.
// However, this api key is limited in its use, and should not be used in any real application.
Initializer.Initialize( "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33", false );
DefinitionAPIClient.LocalStoreDirectory = new DirectoryInfo( @"C:\temp" );

var scoreHistoryClient = new ScoreHistoryAPIClient();
var matchClient = new OrionMatchAPIClient();

Console.WriteLine();
Console.WriteLine( "##### Public ScoreHistory Request output #####." );
Console.WriteLine();

// A ScoreHistory Public Requests returns all publicly available scores for a user.
// It does not return protected (practice) scores, to do this requires the Authenticated ScoreHistory Request (example below).
var scoreHistoryPublicRequest = new GetScoreHistoryPublicRequest();
// Specify a date range
scoreHistoryPublicRequest.StartDate = new DateTime( 2024, 01, 01 );
scoreHistoryPublicRequest.EndDate = new DateTime( 2024, 12, 31 );

// Specify the user, identified by their user id. This is the same user we will authenticate with below, test_dev_7.
scoreHistoryPublicRequest.UserIds = new List<string>() { "26f32227-d428-41f6-b224-beed7b6e8850" };

// Specify the Event Style to lookup
var eventStyleDef = "v1.0:ntparc:Three-Position Sporter Air Rifle";
scoreHistoryPublicRequest.EventStyleDef = SetName.Parse( eventStyleDef );

GetScoreHistoryPublicResponse scoreHistoryPublicResponse;
do {
    //Make the request
    scoreHistoryPublicResponse = await scoreHistoryClient.GetScoreHistoryPublicAsync( scoreHistoryPublicRequest );

    if (scoreHistoryPublicResponse.HasOkStatusCode) {
        foreach (var scoreHistoryBase in scoreHistoryPublicResponse.ScoreHistoryList.Items) {
            //The response returns both ScoreHistoryEventStyleEntry and ScoreHistoryStageStyleEntry. We only want the event styles in this example.
            if (scoreHistoryBase is ScoreHistoryEventStyleEntry) {
                var scoreHistoryEventStyle = (ScoreHistoryEventStyleEntry)scoreHistoryBase;
                var cofSetName = SetName.Parse( scoreHistoryEventStyle.CourseOfFireDef );
                var cofDefinition = await DefinitionCache.GetCourseOfFireDefinitionAsync( cofSetName );
                //Print out the scores
                Console.WriteLine( $"{scoreHistoryEventStyle.MatchName}  {StringFormatting.SpanOfDates( scoreHistoryEventStyle.StartDate, scoreHistoryEventStyle.EndDate )}  {cofDefinition.CommonName}  {scoreHistoryEventStyle.ScoreFormatted}" );
            }
        }

        //Load more data if there is anymore to load.
        if (scoreHistoryPublicResponse.HasMoreItems) {
            scoreHistoryPublicRequest = (GetScoreHistoryPublicRequest)scoreHistoryPublicResponse.GetNextRequest();
        }
    }

} while (scoreHistoryPublicResponse.HasMoreItems);

/*
    Test Qualification  Thu, 19 Dec 2024  3x10 Air Rifle  281 - 8
    Test Qualification  Thu, 19 Dec 2024  3x10 Air Rifle  278 - 7
    Test 3x20 F  Thu, 07 Nov 2024  3x20 plus Final  678.7
    Test AR 3x10  Thu, 07 Nov 2024  3x10 Air Rifle  296.5
    Aggregate Finals Test  Thu, 07 Nov 2024  3x10 Plus Final  392.9
    Bristow Bombers at Baltimore Brewers  Mon, 23 Sep 2024  3x10 Air Rifle  278.8
    Annapolis Anchors at Manassas Maniacs  Mon, 23 Sep 2024  3x10 Air Rifle  290.1
    Baltimore Brewers at Annapolis Anchors  Mon, 23 Sep 2024  3x10 Air Rifle  185.8
    Annapolis Anchors at Bristow Bombers  Mon, 09 Sep 2024  3x10 Air Rifle  279.2
    Orion Scoring System Virtual Match 06 Sep 2024  Fri, 06 Sep 2024  3x10 Air Rifle  280 - 7
    Orion Scoring System Virtual Match 03 Sep 2024  Tue, 03 Sep 2024  3x10 Air Rifle  289.2
    Test 3x20 Decimal  Thu, 29 Aug 2024  3x20 Air Rifle  565.5
    Test Practice Match 3x20  Tue, 27 Aug 2024  3x20 Air Rifle  555 - 13
    Test Local Match  Mon, 26 Aug 2024  3x10 Air Rifle  274 - 8
    Test Match 27 Jun 2024  Thu, 27 Jun 2024  3x10 Air Rifle  274 - 6
    Test Match 6/12  Thu, 13 Jun 2024  3x10 Air Rifle  89 - 2
    The Results List Axiom  Wed, 12 Jun 2024  3x10 Air Rifle  174 - 4
    Test AR 3x10  Wed, 05 Jun 2024  3x10 Air Rifle  279 - 9
    Test VM  Mon, 03 Jun 2024  3x10 Air Rifle  180 - 4
    Test All of the Changes  Wed, 15 May 2024  3x10 Air Rifle  278 - 7
    Test Reduced Result List with VM  Sat, 04 May 2024  3x10 Air Rifle  278 - 8
    Test Result List Ref  Wed, 01 May 2024  3x10 Air Rifle  273 - 10
    Test Result List Ref  Wed, 01 May 2024  3x10 Air Rifle  280 - 11
    Test Rest API  Fri, 26 Apr 2024  3x10 Air Rifle  273 - 8
    Test AR 3x10  Wed, 06 Mar 2024  3x10 Air Rifle  282 - 11
    Test VM AR 3x10  Wed, 06 Mar 2024  3x10 Air Rifle  280 - 7
    Test Reduced Data Packet  Sun, 03 Mar 2024  3x10 Air Rifle  276 - 7
    Test Spectator Display  Fri, 01 Mar 2024  3x10 Air Rifle  270 - 5
    Test KPS VM  Wed, 28 Feb 2024  3x10 Air Rifle  277 - 7
    Match 08 Feb 2024  Thu, 08 Feb 2024  3x10 Air Rifle  278 - 9
    Test 3x10 PSK Plus Final  Wed, 17 Jan 2024  3x10 plus Final  376.2
    Test AR 3x20 PSK plus Final  Wed, 17 Jan 2024  3x20 plus Final  644.7
    Test 3x20 AR PSK  Tue, 16 Jan 2024  3x20  568 - 22
    Test Air Rifle 3x20 PSK  Sat, 13 Jan 2024  3x20  558 - 17
    Test Air Rifle 3x20  Fri, 12 Jan 2024  3x20 Air Rifle  558 - 13
*/

Console.WriteLine();
Console.WriteLine( "##### Authenticated ScoreHistory Request output #####." );
Console.WriteLine();

// Next example will use the Authenticated ScoreHistory Request to retrieve both public (competition) and protected (practice) scores for a user.
// This requires the user to be logged in and have the proper permissions to access their own protected scores.
var userAuthentication = new UserAuthentication(
    "test_dev_7@shooterstech.net",
    "abcd1234" );
await userAuthentication.InitializeAsync();

var scoreHistoryAuthenticatedRequest = new GetScoreHistoryAuthenticatedRequest( userAuthentication );
// Specify the same date range as before
scoreHistoryAuthenticatedRequest.StartDate = new DateTime( 2024, 01, 01 );
scoreHistoryAuthenticatedRequest.EndDate = new DateTime( 2024, 12, 31 );

// NOTE the authenticated request does not require the user id to be specified, as it will automatically retrieve the scores for the logged in user.

// In this example, also not going to specify the Event Style, so that all scores for the user will be returned, regardless of the Event Style.
// We will however filter out the Event Style in the response, to show how to do so. And also filter out the StageStyles
List<string> stageStyleList = new List<string>() {
        "v1.0:ntparc:Sporter Air Rifle Prone",
        "v1.0:ntparc:Sporter Air Rifle Standing",
        "v1.0:ntparc:Sporter Air Rifle Kneeling"
};

// We will capture one of the Result COF IDs in the response, and provide an example below on how to retreive it.
var resultCofId = string.Empty;

GetScoreHistoryAuthenticatedResponse getScoreHistoryAuthenticatedResponse;
do {
    //Make the request
    getScoreHistoryAuthenticatedResponse = await scoreHistoryClient.GetScoreHistoryAuthenticatedAsync( scoreHistoryAuthenticatedRequest );
    if (getScoreHistoryAuthenticatedResponse.HasOkStatusCode) {

        foreach (var scoreHistoryBase in getScoreHistoryAuthenticatedResponse.ScoreHistoryList.Items) {

            //The response returns both ScoreHistoryEventStyleEntry and ScoreHistoryStageStyleEntry. Lets deal with the EventStyle scores first
            if (scoreHistoryBase is ScoreHistoryEventStyleEntry) {
                var scoreHistoryEventStyle = (ScoreHistoryEventStyleEntry)scoreHistoryBase;

                // Since the request did not filter out by EventStyle, we'll do the equivalent operation here, post-request, to show how to do so.
                if (scoreHistoryEventStyle.EventStyleDef != eventStyleDef) {
                    continue;
                }

                // For the sake of this example, will also filter out Public scores, that we demoed above, and only show the protected (practice) scores.
                if (scoreHistoryEventStyle.Visibility != Scopos.BabelFish.DataModel.Common.VisibilityOption.PROTECTED) {
                    continue;
                }

                var cofSetName = SetName.Parse( scoreHistoryEventStyle.CourseOfFireDef );
                var cofDefinition = await DefinitionCache.GetCourseOfFireDefinitionAsync( cofSetName );

                // Print out the scores
                Console.WriteLine( $"{scoreHistoryEventStyle.MatchName}  {StringFormatting.SpanOfDates( scoreHistoryEventStyle.StartDate, scoreHistoryEventStyle.EndDate )}  {cofDefinition.CommonName}  {scoreHistoryEventStyle.ScoreFormatted}" );

                resultCofId = scoreHistoryEventStyle.ResultCOFID;

            } else if (scoreHistoryBase is ScoreHistoryStageStyleEntry) {
                var scoreHistoryStageStyle = (ScoreHistoryStageStyleEntry)scoreHistoryBase;

                // Since the request did not filter out by StageStyle, we'll do the equivalent operation here, post-request, to show how to do so.
                if (!stageStyleList.Contains( scoreHistoryStageStyle.StageStyleDef )) {
                    continue;
                }

                // For the sake of this example, will also filter out Public scores, that we demoed above, and only show the protected (practice) scores.
                if (scoreHistoryStageStyle.Visibility != Scopos.BabelFish.DataModel.Common.VisibilityOption.PROTECTED) {
                    continue;
                }

                // And lets also filter out scores that are zero.
                if (scoreHistoryStageStyle.Score.IsZero)
                    continue;

                var cofSetName = SetName.Parse( scoreHistoryStageStyle.CourseOfFireDef );
                var cofDefinition = await DefinitionCache.GetCourseOfFireDefinitionAsync( cofSetName );
                // Print out the scores
                Console.WriteLine( $"{scoreHistoryStageStyle.MatchName}  {StringFormatting.SpanOfDates( scoreHistoryStageStyle.StartDate, scoreHistoryStageStyle.EndDate )}  {scoreHistoryStageStyle.StageStyleDef}  {scoreHistoryStageStyle.ScoreFormatted}" );
            }
        }

        // Load more data if there is anymore to load.
        if (getScoreHistoryAuthenticatedResponse.HasMoreItems) {
            scoreHistoryAuthenticatedRequest = (GetScoreHistoryAuthenticatedRequest)getScoreHistoryAuthenticatedResponse.GetNextRequest();
        }
    }

} while (getScoreHistoryAuthenticatedResponse.HasMoreItems);

Console.WriteLine();
Console.WriteLine( "##### Authenticated Result COF Request output #####." );
Console.WriteLine();

// For fun, we'll define our own format to display each shot's scores.
// Full documentation at https://support.scopos.tech/index.html?string-formatting-score-format.html
var shotScoreFormat = "{i}{X} ({d})";

// Using the resultCofId that we obtained in the GetScoreHistoryAuthenticatedAsync(), we will retrieve the full set of data on the score, which is known as a Result COF
var resultCofAuthenticatedResponse = await matchClient.GetResultCourseOfFireDetailAuthenticatedAsync( resultCofId, userAuthentication );
if (resultCofAuthenticatedResponse.HasOkStatusCode) {
    var resultCof = resultCofAuthenticatedResponse.ResultCOF;

    // Print the scores for each Event in the Course of Fire
    foreach (var eventScore in resultCof.EventScores.Values) {
        Console.WriteLine( $"{eventScore.EventName} {eventScore.ScoreFormatted}" );
    }

    // When we go to print out the shot scores, the shots are unordered. To get the order of shots, we'll query the Course of Fire definition, which contains the order of shots.
    var cofDefinition = await DefinitionCache.GetCourseOfFireDefinitionAsync( SetName.Parse( resultCof.CourseOfFireDef ) );
    var eventTree = EventComposite.GrowEventTree( cofDefinition );

    // Get a dictionary of shots by EventName, so that we can easily retrieve the shot score for each shot in the EventTree.
    var shotsByEventName = resultCof.GetShotsByEventName();

    // Print the scores for each Shot in the Course of Fire
    // A 'Singular' is a leaf node in the EventTree, which represents a single shot.
    foreach (var shotEvent in eventTree.GetAllSingulars()) {
        // Using the shotEvent's name, retrieve the shot score from the Result COF, and print it out.
        var shot = shotsByEventName[shotEvent.EventName];
        var formattedScore = StringFormatting.FormatScore( shotScoreFormat, shot.Score );
        Console.WriteLine( $"{shot.EventName} Location: ({shot.Location.X:F2} {shot.Location.Y:F2}) Score: {formattedScore}" );
    }
}

Console.ReadKey();
