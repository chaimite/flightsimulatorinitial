namespace FlightBooking.Core
{
    public class FlightRoute
    {
        private readonly string _origin;
        private readonly string _destination;

        public FlightRoute(string origin, string destination)
        {
            _origin = origin;
            _destination = destination;
        }

        public string Title { get { return _origin + " to " + _destination; } }
        public decimal BasePrice { get; set; }
        public decimal BaseCost { get; set; }
        public int LoyaltyPointsGained { get; set; }
        public decimal MinimumTakeOffPercentage { get; set; }        
    }
}
