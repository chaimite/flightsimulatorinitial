using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace FlightBooking.Core
{
    public class ScheduledFlight
    {
        private readonly string VERTICAL_WHITE_SPACE = Environment.NewLine + Environment.NewLine;
        private readonly string NEW_LINE = Environment.NewLine;
        private const string INDENTATION = "    ";

        public ScheduledFlight(FlightRoute flightRoute)
        {
            FlightRoute = flightRoute;
            Passengers = new List<Passenger>();
            BusinessRule = BusinessRuleType.Default;
            Planes = new List<Plane>();
        }

        public FlightRoute FlightRoute { get; private set; }
        public Plane Aircraft { get; private set; }
        public List<Passenger> Passengers { get; private set; }

        public List<Plane> Planes { get; set; }
        public BusinessRuleType BusinessRule { get; set; }


        public enum BusinessRuleType
        {
            Relaxed,
            Default
        }

        public void AddPassenger(Passenger passenger)
        {
            Passengers.Add(passenger);
        }

        public void SetAircraftForRoute(Plane aircraft)
        {
            Aircraft = aircraft;
        }
        
        public string GetSummary()
        {
            decimal costOfFlight = 0;
            decimal profitFromFlight = 0;
            int totalLoyaltyPointsAccrued = 0;
            int totalLoyaltyPointsRedeemed = 0;
            int totalExpectedBaggage = 0;
            int seatsTaken = 0;
            int seatsTakenByAirlineEmployees = 0;

            StringBuilder result = new StringBuilder("Flight summary for " + FlightRoute.Title);

            foreach (var passenger in Passengers)
            {
                switch (passenger.Type)
                {
                    case(PassengerType.General):
                        {
                            profitFromFlight += FlightRoute.BasePrice;
                            totalExpectedBaggage++;
                            break;
                        }
                    case(PassengerType.LoyaltyMember):
                        {
                            if (passenger.IsUsingLoyaltyPoints)
                            {
                                int loyaltyPointsRedeemed = Convert.ToInt32(Math.Ceiling(FlightRoute.BasePrice));
                                passenger.LoyaltyPoints -= loyaltyPointsRedeemed;
                                totalLoyaltyPointsRedeemed += loyaltyPointsRedeemed;
                            }
                            else
                            {
                                totalLoyaltyPointsAccrued += FlightRoute.LoyaltyPointsGained;
                                profitFromFlight += FlightRoute.BasePrice;                           
                            }
                            totalExpectedBaggage += 2;
                            break;
                        }
                    case(PassengerType.AirlineEmployee):
                        {
                            seatsTakenByAirlineEmployees++;
                            totalExpectedBaggage += 1;
                            break;
                        }
                    case(PassengerType.Discounted):
                        {
                            profitFromFlight += FlightRoute.BasePrice / 2;
                            break;
                        }
                }
                costOfFlight += FlightRoute.BaseCost;
                seatsTaken++;
            }

           result.Append(VERTICAL_WHITE_SPACE);
            
           result.Append("Total passengers: " + seatsTaken);
           result.Append(NEW_LINE);
           result.Append(INDENTATION + "General sales: " + Passengers.Count(p => p.Type == PassengerType.General));
           result.Append(NEW_LINE);
           result.Append(INDENTATION + "Loyalty member sales: " + Passengers.Count(p => p.Type == PassengerType.LoyaltyMember));
           result.Append(NEW_LINE);
           result.Append(INDENTATION + "Airline employee comps: " + Passengers.Count(p => p.Type == PassengerType.AirlineEmployee));
           result.Append(NEW_LINE);
           result.Append(INDENTATION + "Discount sales: " + Passengers.Count(p => p.Type == PassengerType.Discounted));
            
           result.Append(VERTICAL_WHITE_SPACE);
           result.Append("Total expected baggage: " + totalExpectedBaggage);

           result.Append(VERTICAL_WHITE_SPACE);

           result.Append("Total revenue from flight: " + profitFromFlight);
           result.Append(NEW_LINE);
           result.Append("Total costs from flight: " + costOfFlight);
           result.Append(NEW_LINE);

           decimal profitSurplus = profitFromFlight - costOfFlight;

           result.Append((profitSurplus > 0 ? "Flight generating profit of: " : "Flight losing money of: ") + profitSurplus);

           result.Append(VERTICAL_WHITE_SPACE);

           result.Append("Total loyalty points given away: " + totalLoyaltyPointsAccrued + NEW_LINE);
           result.Append("Total loyalty points redeemed: " + totalLoyaltyPointsRedeemed + NEW_LINE);

           result.Append(VERTICAL_WHITE_SPACE);
           
            if (BusinessRule == BusinessRuleType.Relaxed )
            {
                // agreed by email that the aircraft capacity should be checked here as well
                if (IsRevenueGeneratedExceedingCost(seatsTakenByAirlineEmployees, FlightRoute.MinimumTakeOffPercentage) &&
                    IsAircraftCapacityExceeded(seatsTaken, Aircraft.NumberOfSeats))
                {
                    result.Append("THIS FLIGHT MAY PROCEED");
                    result.Append(VERTICAL_WHITE_SPACE);
                } else
                {
                    result.Append("FLIGHT MAY NOT PROCEED");
                    result.Append(GetAlternativePlanes(Planes, seatsTaken));
                }
            }
            else
            {
                if (IsFlightProfitable(profitSurplus) && 
                    IsAircraftCapacityExceeded(seatsTaken, Aircraft.NumberOfSeats) &&
                    IsMinimumTakeOffPercentageAchieved(seatsTaken, Aircraft.NumberOfSeats, FlightRoute.MinimumTakeOffPercentage))
                {
                   result.Append("THIS FLIGHT MAY PROCEED");
                }
                else
                {
                   result.Append("FLIGHT MAY NOT PROCEED");
                   result.Append(GetAlternativePlanes(Planes, seatsTaken));
                }
            }
            return result.ToString();
        }

        private string GetAlternativePlanes(List<Plane> availablePlanes, int numberOfSeatsTaken)
        {
            var stringBuilder = new StringBuilder();
            var flag = false;
            stringBuilder.Append(VERTICAL_WHITE_SPACE);
            stringBuilder.Append("Other more suitable aircrafts are: ");
            foreach (Plane plane in availablePlanes)
            {
                if (plane.NumberOfSeats >= numberOfSeatsTaken)
                {
                    flag = true;
                    stringBuilder.Append(NEW_LINE);
                    stringBuilder.Append(plane.Name + " could handle this flight. ");
                }
            }
            return  !flag? String.Empty : stringBuilder.ToString();
        }

        private bool IsAircraftCapacityExceeded(int seatsTakenInFlight, int aircraftCapacity)
        {
            return seatsTakenInFlight < aircraftCapacity;
        }

        private bool IsFlightProfitable(decimal profitSurplus)
        {
            return profitSurplus > 0;
        }

        private bool IsMinimumTakeOffPercentageAchieved (int seatsTaken, int aircraftNumberOfSeats,decimal minimumTakeOffPercentage)
        {
            return seatsTaken / (decimal)aircraftNumberOfSeats > minimumTakeOffPercentage;
        }

        private bool IsRevenueGeneratedExceedingCost(int numberOfAirlineEmployees, decimal minimumPercentageOfPassengersRequired)
        {
            return numberOfAirlineEmployees > minimumPercentageOfPassengersRequired;
        }
    }
}
