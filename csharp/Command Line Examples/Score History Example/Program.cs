using Scopos.BabelFish.APIClients;
using Scopos.BabelFish.DataActors.ResultListFormatter;
using Scopos.BabelFish.DataActors.ResultListFormatter.UserProfile;
using Scopos.BabelFish.DataModel.Definitions;
using Scopos.BabelFish.DataModel.OrionMatch;
using Scopos.BabelFish.DataModel.ScoreHistory;
using Scopos.BabelFish.Helpers;
using Scopos.BabelFish.Requests.ScoreHistoryAPI;
using Scopos.BabelFish.Responses.ScoreHistoryAPI;
using Scopos.BabelFish.Runtime;

//You may use GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33 as a x-api-key to start working with our API.
//However, this api key is limited in its use, and should not be used in any real application.
Initializer.Initialize( "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33", false );
DefinitionAPIClient.LocalStoreDirectory = new DirectoryInfo( @"C:\temp" ); 

var scoreHistoryClient = new ScoreHistoryAPIClient( );

//A ScoreHistory Public Requests returns all publicly avaliable scores for a user.
var scoreHistoryRequest = new GetScoreHistoryPublicRequest();
//Specify a date range
scoreHistoryRequest.StartDate = new DateTime( 2024, 01, 01 );
scoreHistoryRequest.EndDate = new DateTime( 2024, 12, 31 );
//Specify the user
scoreHistoryRequest.UserIds = new List<string>() { "26f32227-d428-41f6-b224-beed7b6e8850" };
//Specify the Event Style to lookup
var eventStyleDef = "v1.0:ntparc:Three-Position Sporter Air Rifle";
scoreHistoryRequest.EventStyleDef = SetName.Parse( eventStyleDef );

GetScoreHistoryPublicResponse scoreHistoryResponse;
do {
    //Make the request
    scoreHistoryResponse = await scoreHistoryClient.GetScoreHistoryPublicAsync( scoreHistoryRequest );

    if (scoreHistoryResponse.HasOkStatusCode) {
        foreach (var scoreHistoryBase in scoreHistoryResponse.ScoreHistoryList.Items) {
            //The response returns both ScoreHistoryEventStyleEntry and ScoreHistoryStageStyleEntry. We only want the event styles in this example.
            if (scoreHistoryBase is ScoreHistoryEventStyleEntry) {
                var scoreHistoryEventStyle = (ScoreHistoryEventStyleEntry)scoreHistoryBase;
                var cofSetName = SetName.Parse( scoreHistoryEventStyle.CourseOfFireDef );
                var cofDefinition = await DefinitionCache.GetCourseOfFireDefinitionAsync( cofSetName );
                //Print out the scores
                Console.WriteLine( $"{scoreHistoryEventStyle.MatchName}  {StringFormatting.SpanOfDates(scoreHistoryEventStyle.StartDate, scoreHistoryEventStyle.EndDate)}  {cofDefinition.CommonName}  {scoreHistoryEventStyle.ScoreFormatted}" );
            }
        }

        //Load more data if there is anymore to load.
        if (scoreHistoryResponse.HasMoreItems) {
            scoreHistoryRequest = (GetScoreHistoryPublicRequest)scoreHistoryResponse.GetNextRequest();
        }
    }

} while (scoreHistoryResponse.HasMoreItems);

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

Console.WriteLine( "Press any key to close." );
Console.ReadKey();
