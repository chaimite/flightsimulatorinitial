using FlightBooking.Core;
using NUnit.Framework;
using System.Collections.Generic;

namespace FlightBooking.Test
{
    public class ScheduleFlightTest
    {
        static FlightRoute flightRoute = new FlightRoute("London", "Paris")
        {
            BaseCost = 50,
            BasePrice = 100,
            LoyaltyPointsGained = 5,
            MinimumTakeOffPercentage = 0.7M
        };
        static ScheduledFlight scheduledFlight = new ScheduledFlight(flightRoute);
        static List<Passenger>? passengers = null;
        [SetUp]
        public void Setup()
        {
            passengers = new List<Passenger>()
            {
                new Passenger() { Name = "Steve", Age = 30, Type = PassengerType.General },
                new Passenger() { Name = "Mark", Age = 12, Type = PassengerType.General },
                new Passenger() { Name = "James", Age = 36, Type = PassengerType.General },
                new Passenger() { Name = "Jane", Age = 32, Type = PassengerType.General },
                new Passenger() { Name = "John", Age = 29, LoyaltyPoints = 1000, Type = PassengerType.LoyaltyMember, IsUsingLoyaltyPoints = true },
                new Passenger() { Name = "Sarah", Age = 45, LoyaltyPoints = 1250, Type = PassengerType.LoyaltyMember, IsUsingLoyaltyPoints = false },
                new Passenger() { Name = "Jack", Age = 60, LoyaltyPoints = 50, Type = PassengerType.LoyaltyMember, IsUsingLoyaltyPoints = false },
                new Passenger() { Name = "Trevor", Age = 47, Type = PassengerType.AirlineEmployee },
                new Passenger() { Name = "Alan", Age = 34, Type = PassengerType.General },
                new Passenger() { Name = "Suzy", Age = 21, Type = PassengerType.General }
            };

            scheduledFlight.Passengers.AddRange(passengers);

            scheduledFlight.SetAircraftForRoute(
                new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 12 });
        }
        [TearDown]
        public void Cleanup()
        {
            //scheduledFlight.SetAircraftForRoute(
            //    new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 12 });
            //scheduledFlight.BusinessRule = ScheduledFlight.BusinessRuleType.Default;
            //flightRoute.MinimumTakeOffPercentage = 0.7;
            scheduledFlight.Passengers.Clear();
            scheduledFlight.Planes.Clear();
        }

        [Test]
        public void TestExampleCase()
        {
            //Arrange
           
            //Act
            var actualResult = scheduledFlight.GetSummary();

            //Assert
            var expectedResult = @"Flight summary for London to Paris

Total passengers: 10
    General sales: 6
    Loyalty member sales: 3
    Airline employee comps: 1
    Discount sales: 0

Total expected baggage: 13

Total revenue from flight: 800
Total costs from flight: 500
Flight generating profit of: 300

Total loyalty points given away: 10
Total loyalty points redeemed: 100


THIS FLIGHT MAY PROCEED";
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void TestExampleCaseShouldNotProceed()
        {
            //Arrange
            scheduledFlight.SetAircraftForRoute(
                new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 10 });

            //Act
            var actualResult = scheduledFlight.GetSummary();

            //Assert
            var expectedResult = @"Flight summary for London to Paris

Total passengers: 10
    General sales: 6
    Loyalty member sales: 3
    Airline employee comps: 1
    Discount sales: 0

Total expected baggage: 13

Total revenue from flight: 800
Total costs from flight: 500
Flight generating profit of: 300

Total loyalty points given away: 10
Total loyalty points redeemed: 100


FLIGHT MAY NOT PROCEED";
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void TestExampleCaseShouldNotProceedRelaxedRules()
        {
            //Arrange
            scheduledFlight.SetAircraftForRoute(
                new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 10 });
            scheduledFlight.BusinessRule = ScheduledFlight.BusinessRuleType.Relaxed;


            //Act
            var actualResult = scheduledFlight.GetSummary();

            //Assert
            var expectedResult = @"Flight summary for London to Paris

Total passengers: 10
    General sales: 6
    Loyalty member sales: 3
    Airline employee comps: 1
    Discount sales: 0

Total expected baggage: 13

Total revenue from flight: 800
Total costs from flight: 500
Flight generating profit of: 300

Total loyalty points given away: 10
Total loyalty points redeemed: 100


FLIGHT MAY NOT PROCEED";
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void TestExampleCaseShouldProceedRelaxedRules()
        {
            //Arrange
            scheduledFlight.SetAircraftForRoute(
                new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 10 });
            scheduledFlight.BusinessRule = ScheduledFlight.BusinessRuleType.Relaxed;
            flightRoute.MinimumTakeOffPercentage = 0.1M;


            //Act
            var actualResult = scheduledFlight.GetSummary();

            //Assert
            var expectedResult = @"Flight summary for London to Paris

Total passengers: 10
    General sales: 6
    Loyalty member sales: 3
    Airline employee comps: 1
    Discount sales: 0

Total expected baggage: 13

Total revenue from flight: 800
Total costs from flight: 500
Flight generating profit of: 300

Total loyalty points given away: 10
Total loyalty points redeemed: 100


THIS FLIGHT MAY PROCEED";
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void TestExampleCaseShouldNotProceedAndPresentsAlernativePlanes()
        {
            //Arrange
            scheduledFlight.SetAircraftForRoute(
                new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 5 });
            scheduledFlight.Planes.Add(new Plane { Id = 55, Name = "A larger plane that fits more than 10 people", NumberOfSeats = 15 });



            //Act
            var actualResult = scheduledFlight.GetSummary();

            //Assert
            var expectedResult = @"Flight summary for London to Paris

Total passengers: 10
    General sales: 6
    Loyalty member sales: 3
    Airline employee comps: 1
    Discount sales: 0

Total expected baggage: 13

Total revenue from flight: 800
Total costs from flight: 500
Flight generating profit of: 300

Total loyalty points given away: 10
Total loyalty points redeemed: 100


FLIGHT MAY NOT PROCEED

Other more suitable aircrafts are: 
A larger plane that fits more than 10 people could handle this flight. ";
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
