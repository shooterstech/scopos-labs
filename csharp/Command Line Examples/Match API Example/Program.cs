using Scopos.BabelFish.APIClients;
using Scopos.BabelFish.DataActors.ResultListFormatter;
using Scopos.BabelFish.DataActors.ResultListFormatter.UserProfile;
using Scopos.BabelFish.DataModel.OrionMatch;
using Scopos.BabelFish.Runtime;

//You may use GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33 as a x-api-key to start working with our API.
//However, this api key is limited in its use, and should not be used in any real application.
Initializer.Initialize( "GyaHV300my60rs2ylKug5aUgFnYBj6GrU6V1WE33", false );
DefinitionAPIClient.LocalStoreDirectory = new DirectoryInfo( @"C:\temp" );

var client = new OrionMatchAPIClient();

//A MatchID uniquely identifies a match
var matchId = new MatchID( "1.1.2025100109364878.1" );

//Retreives information about the match
var getMatchResponse = await client.GetMatchAsync( matchId );
if (getMatchResponse.HasOkStatusCode) {
    var match = getMatchResponse.Match;
    Console.WriteLine( match.Name );      // October 2025 - Scopos' 3 Position 3x20 Air Rifle Virtual Match
    Console.WriteLine( match.StartDate ); // 10/01/2025 00:00:00
    Console.WriteLine( match.EndDate );   // 10/31/2025 00:00:00

    //Loop through and find the primary result lists
    foreach (var resultEvent in match.ResultEvents) {
        foreach (var resultListAbbr in resultEvent.ResultLists) {
            if (resultListAbbr.Primary) {

                //Retreives the result list (note this command only reteives the start of the list).
                var getResultListResponse = await client.GetResultListAsync( matchId, resultListAbbr.ResultName );
                if (getResultListResponse.HasOkStatusCode) {

                    var resultList = getResultListResponse.ResultList;

                    //Reteive the recommended RESULT LIST FORMAT to use on this Result List
                    var resultListFormat = await resultList.GetResultListFormatDefinitionAsync();

                    //Instantiate a Result List Intermediate Formatted instance, to easily allow us to print out the results.
                    var rlif = new ResultListIntermediateFormatted( resultList, resultListFormat, new BaseUserProfileLookup() );
                    await rlif.InitializeAsync();

                    //For demo purposes, just show the top 3 participants.
                    rlif.ShowNumberOfBodyRows = 0;
                    rlif.ShowNumberOfChildRows = 0;
                    rlif.ShowRanks = 3;

                    //Pretend we are on a wide screen.
                    rlif.ResolutionWidth = 5000;

                    Console.WriteLine( $"Show results for {resultList.Name}" );

                    //Print the header row
                    //Not shown in this example, the header row and header cells are have .ClassList property, containing a list of css classes to decorate them with.
                    foreach (var headerCell in rlif.GetShownHeaderRow()) {
                        Console.Write( headerCell.Text );
                        Console.Write( "  " );
                    }
                    Console.WriteLine();

                    //Print the results, one row at a time.
                    //Not shown in this example, each row and cell are have .ClassList property, containing a list of css classes to decorate them with.
                    foreach (var row in rlif.ShownRows) {
                        foreach (var multilineRow in row) {
                            foreach (var cell in multilineRow.GetShownRow()) {
                                Console.Write( cell.Text );
                                Console.Write( "  " );
                            }
                            Console.WriteLine();
                        }
                    }
                }
                Console.WriteLine();

                /*
                    Show results for Individual - Sporter
                    Rank  Participant  Location  Kneeling  Prone  Standing  Aggregate  
                    1   TOLOSA, REEF  Aiea, HI  190.2  197.5  178.4  566.1  
                    2   Rosario, Joalis  Bartow, FL  184.4  196.7  182.3  563.4  
                    3   Algarin, Miniale  Bartow, FL  189.9  193.9  178.2  562.0  

                    Show results for Individual - Precision
                    Rank  Participant  Location  Kneeling  Prone  Standing  Aggregate  
                    1   Miller, Meredith  Green Springs, OH  207.3  210.2  201.9  619.4  
                    2   Miller, Lyla  Green Springs, OH  206.7  206.1  205.4  618.2  
                    3   Mix, Sarah  Green Springs, OH  203.8  208.6  204.9  617.3  

                    Show results for Team - Sporter
                    Rank  Participant  Location  Kneeling  Prone  Standing  Aggregate  
                    1   Summerlin Academy AJROTC  Bartow, FL  757.6  772.3  708.0  2237.7  
                    2   MSYESS  Booneville, MS  738.2  765.8  640.6  2126.0  
                    3   Bensalem MCJROTC - Sporter  Bensalem, PA  717.1  779.7  601.9  2071.8  

                    Show results for Team - Precision
                    Rank  Participant  Location  Kneeling  Prone  Standing  Aggregate  
                    1   American Legion Post 295 - Precision  Green Springs, OH  819.9  832.2  812.7  2462.4  
                    2   North Forsyth Raiders  Cumming, GA  774.2  813.1  724.8  2294.4  
                    3   Marcos de Niza AJROTC - Precision  Tempe, AZ  710.3  779.5  702.5  2192.3  
                 */
            }
        }
    }
}

Console.WriteLine( "Press any key to close." );
Console.ReadKey();