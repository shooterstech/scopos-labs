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

// Create the request object
// By setting IncludeAwayMatches to true, the response will include Matches that Club Members (of club OrionAccount000016) participated in, but were not hosted by OrionAccount000016.
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

        Console.WriteLine();
    }
}

Console.WriteLine( "Press any key to close." );
Console.ReadKey();

/*
Items Returned: 6
Search Dates: 1/1/2025 - 1/1/2026
Sorting Matches by Match Name, Descending

Match Name:             Test USAS SB 3x20
ID:                     1.1.2025121610271329.0
Owner ID:               OrionAcct000001
Competition Dates:      12/16/2025 - 12/16/2025

Match Name:             Test Spanning Text
ID:                     1.1.2025122311175108.0
Owner ID:               OrionAcct000001
Competition Dates:      12/23/2025 - 12/24/2025

Match Name:             Test Multi-Relay Import
ID:                     1.1.2025121213504726.0
Owner ID:               OrionAcct000001
Competition Dates:      12/12/2025 - 12/12/2025

Match Name:             Test Import SB 3x20
ID:                     1.1.2025121015472732.0
Owner ID:               OrionAcct000001
Competition Dates:      12/10/2025 - 12/12/2025

Match Name:             Match 17 Dec 2025
ID:                     1.1.2025121715384014.0
Owner ID:               OrionAcct000001
Competition Dates:      12/17/2025 - 12/17/2025

Match Name:             Finals Test
ID:                     1.1.2025121510503841.0
Owner ID:               OrionAcct000001
Competition Dates:      12/15/2025 - 12/15/2025
*/