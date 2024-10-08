using System;
using FlightBooking.Core;

namespace FlightBookingProblem
{
    class Program
    {
        private static ScheduledFlight _scheduledFlight ;

        static void Main(string[] args)
        {
            SetupAirlineData();
            
            string command = "";
            do
            {
                try
                {
                    command = Console.ReadLine() ?? "";
                    var enteredText = command.ToLower();

                    if (enteredText.Contains("relaxed"))
                    {
                        _scheduledFlight.BusinessRule = ScheduledFlight.BusinessRuleType.Relaxed;
                    }
                    else if (enteredText.Contains("default"))
                    {
                        _scheduledFlight.BusinessRule = ScheduledFlight.BusinessRuleType.Default;
                    }
                    else if (enteredText.Contains("print summary"))
                    {
                        Console.WriteLine();
                        Console.WriteLine(_scheduledFlight.GetSummary());
                    }
                    else if (enteredText.Contains("add general"))
                    {
                        AddPassengerData(enteredText, _scheduledFlight, PassengerType.General);
                    }
                    else if (enteredText.Contains("add loyalty"))
                    {
                        AddPassengerData(enteredText, _scheduledFlight, PassengerType.LoyaltyMember);
                    }
                    else if (enteredText.Contains("add airline"))
                    {
                        AddPassengerData(enteredText, _scheduledFlight, PassengerType.AirlineEmployee);
                    }
                    else if(enteredText.Contains("add discounted"))
                    {
                        AddPassengerData(enteredText, _scheduledFlight, PassengerType.Discounted);
                    }
                    else if (enteredText.Contains("exit"))
                    {
                        Environment.Exit(1);
                    }
                    else
                    {
                        PrintError();
                    }
                } catch (Exception e)
                {
                    Console.WriteLine($"The data introduce is not correct. The issue was {e.Message}");
                }
            } while (command != "exit");
        }

        private static void AddPassengerData(string enteredText, ScheduledFlight scheduledFlight, PassengerType passengerType)
        {
            string[] passengerSegments = enteredText.Split(' ');
            if (passengerSegments.Length < 4)
                return;

            var newPassenger = new Passenger
            {
                Type = passengerType,
                Name = passengerSegments[2],
                Age = Convert.ToInt32(passengerSegments[3])
            };

            if (passengerType == PassengerType.General && passengerSegments.Length == 4)
            {
                scheduledFlight.AddPassenger(newPassenger);
            }
            else if (passengerType == PassengerType.LoyaltyMember && passengerSegments.Length == 6)
            {
                newPassenger.LoyaltyPoints = Convert.ToInt32(passengerSegments[4]);
                newPassenger.IsUsingLoyaltyPoints = Convert.ToBoolean(passengerSegments[5]);
                scheduledFlight.AddPassenger(newPassenger);
            }
            else if (passengerType == PassengerType.AirlineEmployee && passengerSegments.Length == 4)
            {
                scheduledFlight.AddPassenger(newPassenger);
            }      
            else if (passengerType == PassengerType.Discounted && passengerSegments.Length == 4)
            {               
                scheduledFlight.AddPassenger(newPassenger);
            }
        }

        private static void PrintError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("UNKNOWN INPUT");
            Console.ResetColor();
        }
        private static void SetupAirlineData()
        {
            FlightRoute londonToParis = new FlightRoute("London", "Paris")
            {
                BaseCost = 50, 
                BasePrice = 100, 
                LoyaltyPointsGained = 5,
                MinimumTakeOffPercentage = 0.7M
            };

            _scheduledFlight = new ScheduledFlight(londonToParis);
            _scheduledFlight.Planes.Add(new Plane { Id = 2, Name = "ATR 640", NumberOfSeats = 20 });
            _scheduledFlight.Planes.Add(new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 12 });
            _scheduledFlight.Planes.Add(new Plane { Id = 124, Name = "Bombardier Q400", NumberOfSeats = 35 });
            _scheduledFlight.SetAircraftForRoute(
                new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 12 });
        }
    }
}
