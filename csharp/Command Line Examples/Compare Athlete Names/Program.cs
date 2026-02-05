using Scopos.BabelFish.APIClients;
using Scopos.BabelFish.DataModel.OrionMatch;
using Scopos.BabelFish.Requests.OrionMatchAPI;
using Scopos.BabelFish.Responses.OrionMatchAPI;
using Scopos.BabelFish.Runtime;
using Scopos.Compare_Athlete_Names;

//Try and read the x api key from environment variable. If it is there, use it. Else use the shared (really not recommended) x api key.
string? xApiKey = Environment.GetEnvironmentVariable( "ScoposXApiKey " );
if (xApiKey == null)
    Initializer.Initialize( "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33", false );
else
    Initializer.Initialize( xApiKey, false );

//Set the local store directory, even though we really dont' use it much in this script. 
DefinitionAPIClient.LocalStoreDirectory = new System.IO.DirectoryInfo( @"C:\temp" );

//Read in the list of names to compare against.
var listOfNames = ReadCsv( "c:\\temp\\AthleteNames.csv" );

//Create a match apii client. will use this to get a list of games from the league, as well as the result list from each of these games.
var matchClient = new OrionMatchAPIClient();

//We will be searching athlete names from the 2026 National Air Rifle New Shooter League
var leagueId = new MatchID( "1.1.2025112515273694.3" );

//Set up the first call to get league games. This returns a tokenized list, and we may have to call it multiple times to get all the games.
GetLeagueGamesPublicRequest getLeagueGameRequest = new GetLeagueGamesPublicRequest( leagueId );
GetLeagueGamesPublicResponse getLeagueGameResponse;

do {
    //Make the call to Get League Games.
    getLeagueGameResponse = await matchClient.GetLeagueGamesPublicAsync( getLeagueGameRequest );

    //Iterate over each game it returned.
    foreach (var game in getLeagueGameResponse.LeagueGames.Items) {

        //Get the Result List for this game. IN a league game the result list is always "Team - All"
        var gameResultList = await matchClient.GetResultListPublicAsync( game.GameID, "Team - All" );

        //Iterate over the teams that contributed scores.
        foreach (var team in gameResultList.ResultList.Items) {
            if (team.TeamMembers is not null) {

                //Iteratre over the team members on each team.
                foreach (var teamMember in team.TeamMembers) {

                    //Check that the team member shot, and has score that's non-zero.
                    if (teamMember.EventScores.TryGetValue( "Qualification", out EventScore score ) && !score.Score.IsZero) {
                        var partisipant = (Individual)teamMember.Participant;

                        //Using LevenshteinDistance, see if there are similiar names in the file we read in (up on line 19). 
                        foreach (var name in listOfNames) {
                            var distance = name.Distance( partisipant.GivenName, partisipant.FamilyName );

                            //Print it out if there is. 
                            if (distance <= 2) {
                                Console.WriteLine( $"{name} with a distance of {distance} from the league team {team.Participant.DisplayName} and {partisipant.DisplayName} from {partisipant.HomeTown}." );
                            }
                        }
                    }
                }
            }
        }
    }

    //Get League Game is again a tokenized list. So prepare the next request if there are more items. 
    if (getLeagueGameResponse.HasMoreItems) {
        getLeagueGameRequest = getLeagueGameResponse.GetNextRequest();
    }

} while (getLeagueGameResponse.HasMoreItems);

Console.WriteLine( "Press any key to close." );
Console.ReadKey();

/*
 * Method to read in the list of names to compare against fro a csv file
 * The CSV file has the format:
 * QUAMIR,BARHAM,Norview HS VA 1
 * BIANCA,BUENSUCESO,Pace HS FL 1
 * CONRAD,MEIS,Highland HS NM 1
 */
static List<AthleteName> ReadCsv( string filePath ) {
    var results = new List<AthleteName>();

    foreach (var line in File.ReadLines( filePath )) {
        if (string.IsNullOrWhiteSpace( line ))
            continue;

        // Split on comma
        var parts = line.Split( ',' );

        if (parts.Length < 3)
            continue; // or throw, depending on your needs

        var athlete = new AthleteName();
        athlete.FirstName = parts[0].Trim();
        athlete.LastName = parts[1].Trim();
        athlete.TeamName = parts[2].Trim();

        results.Add( athlete );
    }

    return results;
}