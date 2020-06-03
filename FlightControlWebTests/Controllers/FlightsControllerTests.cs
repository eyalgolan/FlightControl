using FlightControlWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightControlWeb.Controllers.Tests
{
    [TestClass]
    public class FlightsControllerTests
    {
        /*
         * Create a Flight object for the test
         */
        private static Flight CreateTestFlight()
        {
            var testFlight = new Flight
            {
                Id = 1,
                FlightId = "IL30357629",
                IsExternal = false
            };
            return testFlight;
        }

        /*
         * Create a FlightPlan object for the test
         */
        private static FlightPlan CreateTestFlightPlan(DateTime endTime)
        {
            var testFlightPlan = new FlightPlan
            {
                Id = 1,
                FlightId = "IL30357629",
                Passengers = 247,
                CompanyName = "United Airlines",
                IsExternal = false,
                EndTime = endTime
            };
            return testFlightPlan;
        }

        /*
         * Create an InitialLocation object for the test
         */
        private static InitialLocation CreateTestInitialLocation(DateTime startTime)
        {
            var testInitialLocation = new InitialLocation
            {
                Id = 1,
                FlightPlanId = 1,
                Longitude = 33.24,
                Latitude = 19.53,
                DateTime = startTime
            };
            return testInitialLocation;
        }

        /*
         * Create a Segment object for the test
         */
        private static Segment CreateTestSegment(int id, int flightPlanId, double longitude, double latitude, int timeSpan, DateTime startTime, DateTime endTime)
        {
            var testSegment = new Segment
            {
                Id = id,
                FlightPlanId = flightPlanId,
                Longitude = longitude,
                Latitude = latitude,
                TimeSpanSeconds = timeSpan,
                StartTime = startTime,
                EndTime = endTime
            };
            return testSegment;
        }

        /*
         * Add test objects to the DB context
         */
        private static async Task AddElementsToDb(FlightContext context, Flight testFlight, FlightPlan testFlightPlan, InitialLocation testInitialLocation, Segment testSegmentFirst, Segment testSegmentSecond)
        {
            await context.FlightItems.AddAsync(testFlight);
            await context.FlightPlanItems.AddAsync(testFlightPlan);
            await context.InitialLocationItems.AddAsync(testInitialLocation);
            await context.SegmentItems.AddAsync(testSegmentFirst);
            await context.SegmentItems.AddAsync(testSegmentSecond);
            await context.SaveChangesAsync();
        }

        /*
         * Run the GetFlights method with the test objects, get the results and check if they are correct
         */
        private static async Task GetResultsAndCheck(FlightsController controller, string relativeTo, double expectedLongitude, double expectedLatitude)
        {
            // run GetFlights and get the result
            var result = (await controller.GetFlights(relativeTo)).FirstOrDefault();

            // check that result is not null and parse the result for further checks
            Assert.IsNotNull(result);
            var resultFlightId = result.FlightId;
            var resultLongitude = result.Longitude;
            var resultLatitude = result.Latitude;
            var resultPassengers = result.Passengers;
            var resultCompanyName = result.CompanyName;
            var pattern = "dd-MMM-yy h:mm:ss tt";
            var culture = new CultureInfo("en-US");
            var resultDateTime = result.CurrDateTime.ToString(pattern, culture);
            var resultIsExternal = result.IsExternal;

            // checking GetFlights returned the right results
            Assert.AreEqual("IL30357629", resultFlightId);
            Assert.AreEqual(expectedLongitude, resultLongitude);
            Assert.AreEqual(expectedLatitude, resultLatitude);
            Assert.AreEqual(247, resultPassengers);
            Assert.AreEqual("United Airlines", resultCompanyName);
            Assert.AreEqual("27-May-20 3:35:05 PM", resultDateTime);
            Assert.AreEqual(false, resultIsExternal);
        }

        /*
         * Create the DB context, populated with test objects
         */
        private async Task<FlightContext> CreateTestContext(DateTime startTime, DateTime endTime)
        {
            // context
            var options = new DbContextOptionsBuilder<FlightContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new FlightContext(options);

            // Creating elements to add to the DB
            var testFlight = CreateTestFlight();
            var testFlightPlan = CreateTestFlightPlan(endTime);
            var testInitialLocation = CreateTestInitialLocation(startTime);
            var testSegmentFirst = CreateTestSegment(1, 1, 23.240702, 34.921971, 1800, startTime, startTime.AddSeconds(1800));
            var testSegmentSecond = CreateTestSegment(2, 1, 21.346370, 39.419221, 1800, startTime.AddSeconds(1800), endTime);

            // adding the elements to the db
            await AddElementsToDb(context, testFlight, testFlightPlan, testInitialLocation, testSegmentFirst, testSegmentSecond);

            return context;
        }

        /*
         * Add a test internal flight with all of it's object and run the GetFlights method - in order to check it returns the right results
         */
        [TestMethod]
        public async Task GetFlightsInternalTest()
        {
            // determining the flight times
            var relativeTo = "2020-05-27T15:35:05Z";
            var startTime = DateTime.Parse("2020-05-27T12:05:00Z");
            var endTime = DateTime.Parse("2020-05-27T13:05:00Z");

            // context
            var context = await CreateTestContext(startTime, endTime);

            // mock http client factory
            var mockClientFactory = new Mock<IHttpClientFactory>();

            // controller
            var controller = new FlightsController(context, mockClientFactory.Object);

            // calculating the location
            var testSegmentFirst = CreateTestSegment(1, 1, 23.240702, 34.921971, 1800, startTime, startTime.AddSeconds(1800));
            var testSegmentSecond = CreateTestSegment(2, 1, 21.346370, 39.419221, 1800, startTime.AddSeconds(1800), endTime);
            var secondsInSegment = 5.0;
            var delta = secondsInSegment / testSegmentSecond.TimeSpanSeconds;
            var expectedLatitude = testSegmentFirst.Latitude + delta *
                (testSegmentSecond.Latitude - testSegmentFirst.Latitude);
            var expectedLongitude = testSegmentFirst.Longitude + delta *
                (testSegmentSecond.Longitude - testSegmentFirst.Longitude);

            // Act - running method and checking results
            await GetResultsAndCheck(controller, relativeTo, expectedLongitude, expectedLatitude);
        }
    }
}