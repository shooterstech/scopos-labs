using Scopos.BabelFish.APIClients;
using Scopos.BabelFish.DataActors.OrionMatch;
using Scopos.BabelFish.DataModel.Athena.Shot;
using Scopos.BabelFish.DataModel.Definitions;
using Scopos.BabelFish.DataModel.OrionMatch;
using Scopos.BabelFish.Helpers;
using Scopos.BabelFish.Requests.OrionMatchAPI;
using Scopos.BabelFish.Responses.OrionMatchAPI;
using Scopos.BabelFish.Runtime;
using Scopos.BabelFish.Runtime.Authentication;

/*
 * This is an example of submitting individual shots to an Orion match. Some key points in this example:
 * - Requires an authenticated user with the Stat Officer role (set in Orion). 
 * - Makes a call to our API to retreive infomation about the match.
 * - Makes multiple calls to our API to retreive the athletes in the match, in the form of a squadding list.
 * - Usin the COURE OF FIRE definition, learns the TARGET and SCORE FORMAT COLLECTION that's in use.
 * - Using the COURSE OF FIRE definition, via the Events of EventType Stage, learns the number of expected shots in each stage, and the stage label.
 * - Makes an authenticated POST to our API submitting shot data. in this example submits all 10 shots for each stage, for each athlete.
 */

//You may use GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33 as a x-api-key to start working with our API.
//However, this api key is limited in its use, and should not be used in any real application.

Initializer.Initialize( "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33", false );
DefinitionAPIClient.LocalStoreDirectory = new System.IO.DirectoryInfo( @"C:\temp" );

var matchClient = new OrionMatchAPIClient();
//The authenticated user must be a Stat Officer in the Orion Match. 
//To learn more about user roles in Orion visit https://support.scopos.tech/index.html?participant-permissions-defini.html
var userAuthentication = new UserAuthentication(
    "test_dev_7@shooterstech.net",
    "abcd1234" );
await userAuthentication.InitializeAsync();

// Match IDs are unique to each match. To learn your match id, from Orion, click on Match -> Copy Match ID
var matchId = new MatchID( "1.1.2025121213504726.0" );

// Read information about the match from the REST API
var getMatchDetailResponse = await matchClient.GetMatchAuthenticatedAsync( matchId, userAuthentication );
var matchObj = getMatchDetailResponse.Match;

// Now pull information about the match's COURSE OF FIRE, TARGET COLLECTION, and SCORE FORMAT COLLECTION in use.
var cofDefinition = await DefinitionCache.GetCourseOfFireDefinitionAsync( SetName.Parse( matchObj.CourseOfFireDef ) );
var targetCollectionDefinition = await cofDefinition.GetTargetCollectionDefinitionAsync();
var scoreFormatDefinition = await cofDefinition.GetScoreFormatCollectionDefinitionAsync();

// A random, random number generator appears (for use to generate simulated shots).
var randomNumber = new RandomGaussianNumberGenerator();

// Retreive the Squadding for the match
// Potentially have to call GetSquaddingList multiple times, if the number of participants is too large for one call to return.
GetSquaddingListPublicRequest getSquaddingRequest = new GetSquaddingListPublicRequest( matchId, matchObj.SquaddingEvents[0].Name );
GetSquaddingListPublicResponse getSquaddingResponse;
SquaddingList? squaddingList = new SquaddingList();
do {
    getSquaddingResponse = await matchClient.GetSquaddingListPublicAsync( getSquaddingRequest );
    if (getSquaddingResponse.HasOkStatusCode) {
        if (squaddingList == null) {
            squaddingList = getSquaddingResponse.SquaddingList;
        } else {
            squaddingList.Items.AddRange( getSquaddingResponse.SquaddingList.Items );
        }
    }
} while (getSquaddingResponse.HasOkStatusCode && getSquaddingResponse.HasMoreItems);

// Learn about the relays that make up this match, and sort them.
squaddingList.GenerateRelayInformation();
CompareRelayInformation relayInfoComparer = new CompareRelayInformation( CompareRelayInformation.CompareMethod.RELAY_NAME, SortBy.ASCENDING );
squaddingList.RelayInformation.Sort( relayInfoComparer );

// Grow the Event tree. We will soo ask it about the "Stages" in the match.
var cofTree = EventComposite.GrowEventTree( cofDefinition );

//Now we are ready to start simulating scores

//For each relay in the match
foreach (var relayInformation in squaddingList.RelayInformation) {

    //For each athlete on that relay
    foreach (var athlete in squaddingList.FilterByRelayInformation( relayInformation )) {

        // Each shot reported to Orion, for each athlete, must have a unique, in order sequence number. 
        var sequence = 1;

        // For each Stage in the match.
        // The Stages, as defined  in the COURSE OF FIRE are all shot continuously, under the same time constraint, on the same TARGET, and have the same stage label.
        foreach (var stage in cofTree.GetEvents( EventtType.STAGE )) {

            // Learn the TARGET definition in use for this Stage.
            var targetDefinition = await stage.GetTargetAsync( cofDefinition, matchObj.TargetCollectionName );

            // We'll use the 10 ring as the basis of the normal distribution to simulate shots.
            var tenRingDiameter = targetDefinition.ScoringRings[0].Dimension + 2;
            Console.WriteLine( $"Generating shots for {athlete.Participant.DisplayName} {stage.EventName}, using {targetDefinition.CommonName}" );

            // Shots may be submitted to Orion one at a time or in sets. In this example we are simulating and sending all shots for one athlete for one stage at a time. 
            PostShotDataAuthenticatedRequest request = new PostShotDataAuthenticatedRequest( userAuthentication, matchId );
            // A Singular is one shot (well, its usually a shot) within an Event.
            foreach (var singular in stage.GetAllSingulars()) {
                // Simulate a random shot.
                var x = (float)randomNumber.NextGaussian( 0, tenRingDiameter );
                var y = (float)randomNumber.NextGaussian( 0, tenRingDiameter );
                var score = targetDefinition.Score( x, y, cofDefinition.DefaultScoringDiameter );
                Console.WriteLine( $"{x:F1} {y:F1} {score}" );

                var fp = ((SquaddingAssignmentFiringPoint)athlete.SquaddingAssignment).FiringPoint;
                var shot = new Shot() {
                    Score = score, // Score of the shot, that we generated above.
                    TargetName = $"Target on FP {fp}", // The unique name of the EST unit that scored the shot.
                    TargetSetName = targetDefinition.SetName, // the SetName of the TARGET definition in use.
                    TimeScored = DateTime.UtcNow, // time the shot was scored, in UTC
                    BulletDiameter = cofDefinition.DefaultExpectedDiameter, // in mm, the diamter of the bullet fired at the EST
                    ScoringDiameter = cofDefinition.DefaultScoringDiameter, // in mm, the scoring diameter
                    Location = new Location() { // Location of the shot
                        X = x,
                        Y = y
                    },
                    Sequence = sequence++, // Each shot must have a unique and in order sequence value.
                    ResultCOFID = ((Individual)athlete.Participant).ResultCOFID, //Result COF ID identifies the athletes set of scores for the match / event.
                    MatchID = matchId, // Match ID
                    StageLabel = singular.StageLabel, // Stage Label identifies the stage the shot was fired within. Must be defined in the COURSE OF FIRE
                    RangeTime = "0:00:00", // Range clock time the shot was fired
                    FiringPoint = fp, // Firing point
                    Privacy = matchObj.Visibility, // protection level of the score of this shot.
                    ScoreFormatted = StringFormatting.FormatScore( scoreFormatDefinition, matchObj.ScoreConfigName, singular.ScoreFormat, score ) //a string representing the score of teh shot.
                };
                // By setting the Meta property ESTSystem to the name of the EST System that score the shot, its will get recoonized on Rezults.
                shot.Meta = new System.Dynamic.ExpandoObject();
                ((dynamic)shot.Meta).ESTSystem = "Scopos-Labs";

                // Add the shot to the request
                request.Shots.Add( shot );
            }

            // Submit the shot to the REST API
            // * IF * the shots are accepted, the REST API will send the shots to Orion via a queue.
            // Orion does NOT have to be open to the match when your submit the shots.
            // Event when Orion is on, it may take Orion 30 to 60s to read the shots from the queue -- be patient
            var response = await matchClient.PostShotDataAuthenticatedAsync( request );

            // In this example, if you use the match id provided and run it, the shots will come back from the REST API as rejected. This is because they were submitted
            // and generated outside the start and end date of the match. Normally though, they would of been accepted. 
            Console.WriteLine( $"{response.AcceptedShots.Count} shots acepted. {response.RejectedShots.Count} shots rejected." );
            foreach (var rejectedShot in response.RejectedShots) {
                var rejectedReasons = string.Join( ", ", rejectedShot.RejectionMessages );
                Console.WriteLine( $"Shot sequence {rejectedShot.Sequence} rejected with reasones {rejectedReasons}" );
            }

            // While you could submit all the shots one after another, its more interesting if you submit them more slowly. 
            Thread.Sleep( 5000 );
        }
    }
}

// Some logging to see how many API calls we made
Console.WriteLine( $"OrionMatchAPI calls {OrionMatchAPIClient.Statistics.NumberOfApiCalls}." );
Console.WriteLine( $"DefinitionAPI calls {DefinitionAPIClient.Statistics.NumberOfApiCalls}." );

Console.WriteLine( "Press any key to close." );
Console.ReadKey();
