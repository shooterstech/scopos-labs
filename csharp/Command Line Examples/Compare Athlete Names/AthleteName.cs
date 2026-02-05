using Scopos.BabelFish.Helpers;

namespace Scopos.Compare_Athlete_Names {
    public class AthleteName {

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string TeamName { get; set; }

        public int Distance( string firstName, string lastName ) {

            return Math.Min( Common.LevenshteinDistance( firstName, this.FirstName, false ) + Common.LevenshteinDistance( lastName, this.LastName, false ),
                Common.LevenshteinDistance( firstName, this.LastName, false ) + Common.LevenshteinDistance( lastName, this.FirstName, false ) );
        }

        public override string ToString() {
            return $"{FirstName} {LastName}, {TeamName}";
        }
    }
}
