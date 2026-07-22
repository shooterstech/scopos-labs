/*
 * Uses the MatchSearch request to look up matches a club has hosted. 
 * Displays information about the match, including making a call to GetSquaddingList to show the number of participants.
 * 
 * Please Note: Match Search is deprecated as of July 2026 and with BabelFish version 2.1. It is being replaced with List Matches. 
 */

//Try and read the x api key from environment variable. If it is there, use it. Else use the shared (really not recommended) x api key.
using Scopos.BabelFish.APIClients;
using Scopos.BabelFish.DataActors.OrionMatch;
using Scopos.BabelFish.Helpers;
using Scopos.BabelFish.Requests.OrionMatchAPI;
using Scopos.BabelFish.Runtime;

string? xApiKey = Environment.GetEnvironmentVariable( "ScoposXApiKey " );
if (xApiKey == null)
    // You may use GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33 as a x-api-key to start working with our API.
    // However, this api key is limited in its use, and should not be used in any real application.
    Initializer.Initialize( "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33", false );
else
    Initializer.Initialize( xApiKey, false );

//Set the local store directory, even though we really dont' use it much in this script. 
DefinitionAPIClient.LocalStoreDirectory = new DirectoryInfo( @"C:\temp" );

//Instantiate a match api client
var client = new OrionMatchAPIClient();

//figure out our search parameters
//To have consistence in our example, retreiving matches from the year 2025.
DateTime year25 = DateTime.Parse( "2026/01/01" ); // 1 January 2025
DateTime startDate = year25.AddDays( -365 ); // need to set
DateTime endDate = year25.AddDays( 0 ); // need to set
int limit = 10;
string ownerId = "OrionAccount000016";

//Create the request object
var request = new ListMatchesPublicRequest() {
    OwnerId = ownerId,
    IncludeAwayMatches = true,
    StartDate = startDate,
    EndDate = endDate
};

// Retrieve MatchAbbrList
var matchSearchResponse = await client.ListMatchesPublicAsync( request );

// Check the status code
if (matchSearchResponse.HasOkStatusCode) {

    // Extract the list of matches
    var matchSearchList = matchSearchResponse.MatchList;
    Console.WriteLine( "Items Returned: " + matchSearchList.Items.Count() );      // amount of items returned from search based owner ID and Limit (10)
    Console.WriteLine( "Search Dates: " + startDate.ToShortDateString() + " - " + endDate.ToShortDateString() );

    Console.WriteLine( "Sorting Matches by Match Name, Descending" );
    Console.WriteLine();
    // Using the compare method for Match Abbr, as that is what the search returns.
    var comparer = new CompareMatchAbbr( CompareMatchAbbr.CompareMethod.MATCH_NAME, SortBy.DESCENDING );
    // Calling sort using the new comparer.
    matchSearchList.Items.Sort( comparer );

    // Manipulate the data in each match to show what we want.
    foreach (var matchAbbr in matchSearchList.Items) {

        Console.WriteLine( "Match Name: \t\t" + matchAbbr.MatchName );
        Console.WriteLine( "ID: \t\t\t" + matchAbbr.MatchID.ToString() );
        Console.WriteLine( "Owner ID: \t\t" + matchAbbr.OwnerId );
        Console.WriteLine( "Competition Dates: \t" + matchAbbr.StartDate.ToShortDateString() + " - " + matchAbbr.EndDate.ToShortDateString() );
        Console.WriteLine( "Match Location: \t" + matchAbbr.Location.City + ", " + matchAbbr.Location.Region + ", " + matchAbbr.Location.Country );

        // Get the match detail, which holds the name of the squadding lists.
        var matchDetailResponse = await client.GetMatchAsync( matchAbbr.MatchID );
        if (matchDetailResponse.HasOkStatusCode) {
            // Get the Name of the Squadding event from the MatchDetail
            var squaddingListName = matchDetailResponse.Match.SquaddingEvents[0].Name;

            // Retreives the result list (note this command only reteives the start of the list).
            var getSquaddingListResponse = await client.GetSquaddingListAsync( matchAbbr.MatchID, squaddingListName );
            if (getSquaddingListResponse.HasOkStatusCode) {

                // Reteive the Squadding List for match
                var squaddingList = getSquaddingListResponse.SquaddingList.Items;
                Console.WriteLine( "Number of Participants: " + squaddingList.Count() );
            }
        }
        Console.WriteLine();
    }
}

Console.WriteLine( "Press any key to close." );
Console.ReadKey();

/*
Items Returned: 10
Search Dates: 1/2/2024 - 1/1/2025
Sorting Matches by Match Name, Descending

Match Name:             Practice Match 12/8/2024
ID:                     1.2948.2024120816323626.0
Competition Dates:      12/8/2024 - 12/8/2024
Match Location:         DuBois, PA, USA
Number of Participants: 9

Match Name:             Practice Match 12/4/2024
ID:                     1.2948.2024120416451405.0
Competition Dates:      12/4/2024 - 12/4/2024
Match Location:         DuBois, PA, USA
Number of Participants: 17

Match Name:             Practice 12/21
ID:                     1.2948.2024122116515713.0
Competition Dates:      12/21/2024 - 12/21/2024
Match Location:         DuBois, PA, USA
Number of Participants: 2

Match Name:             DAHS vs Tyrone Christian Academy JV 12/12/24
ID:                     1.2948.2024121218430945.0
Competition Dates:      12/12/2024 - 12/12/2024
Match Location:         DuBois, PA, USA
Number of Participants: 6

Match Name:             DAHS vs Tyrone Christan Academy 12/12/24
ID:                     1.2948.2024121214243462.0
Competition Dates:      12/12/2024 - 12/12/2024
Match Location:         DuBois, PA, USA
Number of Participants: 15

Match Name:             DAHS vs Bishop Carroll Varsity 12/20/24
ID:                     1.2948.2024121819273482.0
Competition Dates:      12/20/2024 - 12/20/2024
Match Location:         DuBois, PA, USA
Number of Participants: 16

Match Name:             DAHS vs Bishop Carroll JV 12/20/2024
ID:                     1.2948.2024122018432887.0
Competition Dates:      12/20/2024 - 12/20/2024
Match Location:         DuBois, PA, USA
Number of Participants: 8

Match Name:             2024 4 th Quarter ACES Standing
ID:                     1.2948.2024112517152752.0
Competition Dates:      11/25/2024 - 11/25/2024
Match Location:         DuBois, PA, USA
Number of Participants: 2

Match Name:             2024 - 2025 PA State Junior Olympic SB Championship
ID:                     1.2948.2024122716400811.0
Competition Dates:      12/28/2024 - 2/2/2025
Match Location:         DuBois, PA, USA
Number of Participants: 50

Match Name:             2024 - 2025 PA State Junior Olympic Air Rifle Championship
ID:                     1.2948.2024122716495515.0
Competition Dates:      12/28/2024 - 3/31/2025
Match Location:         DuBois, PA, USA
Number of Participants: 50  
*/