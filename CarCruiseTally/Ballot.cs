namespace CarCruiseTally
{
    /// <summary>
    /// Ballot object containing all of the data that can be entered for a ballot
    /// </summary>
    public class Ballot
    {
        /// <summary>
        /// Top 10 list on the ballot.
        /// </summary>
        public int[] Top10 { get; private set; }

        /// <summary>
        /// The car number of this ballot's owner
        /// </summary>
        public int CarNumber { get; private set; }

        /// <summary>
        /// Constructs a new <see cref="Ballot"/>
        /// </summary>
        /// <param name="top10"><see cref="Top10"/></param>
        /// <param name="carnum"><see cref="CarNumber"/></param>
        public Ballot(int[] top10, int carnum)
        {
            Top10 = top10;
            CarNumber = carnum;
        }
    }
}