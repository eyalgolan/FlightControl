using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControlWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlightControlWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using Moq;
using Moq.Protected;

namespace FlightControlWeb.Controllers.Tests
{
    [TestClass()]
    public class FlightsControllerTests
    {
        [TestMethod()]
        public void GetFlightsTest()
        {

            // Creating elements to add to the DB

            var testFlight = new List<Flight>
            {
                new Flight()
                {
                    Id = 1,
                    FlightId = "IL30357629",
                    IsExternal = false
                }
            }.AsQueryable();

            DateTime startTime = DateTime.Parse("2020-05-27T15:05:00Z");
            DateTime endTime = DateTime.Parse("2020-05-27T16:05:00Z");
            var testFlightPlan = new List<FlightPlan>
            {
                new FlightPlan()
                {
                    Id = 1,
                    FlightId = "IL30357629",
                    Passengers = 247,
                    CompanyName = "United Airlines",
                    IsExternal = false,
                    EndTime = endTime
                }
            }.AsQueryable();

            var testInitialLocation = new List<InitialLocation>
            {
                new InitialLocation()
                {
                    Id = 1,
                    FlightPlanId = 1,
                    Longitude = 33.24,
                    Latitude = 19.53,
                    DateTime = startTime
                }
            }.AsQueryable();

            var testSegment = new List<Segment>
            {
                new Segment()
                {
                    Id = 1,
                    FlightPlanId = 1,
                    Longitude = 23.240702,
                    Latitude = 34.921971,
                    TimeSpanSeconds = 1800,
                    StartTime = startTime,
                    EndTime = startTime.AddSeconds(1800)
                },

                new Segment()
                {
                    Id = 2,
                    FlightPlanId = 1,
                    Longitude = 21.346370,
                    Latitude = 39.419221,
                    TimeSpanSeconds = 1800,
                    StartTime = startTime.AddSeconds(1800),
                    EndTime = endTime
                }
            }.AsQueryable();
            
            // Creating Mock sets for DB
            var mockFlightSet = new Mock<DbSet<Flight>>();
            var mockFlightPlanSet = new Mock<DbSet<FlightPlan>>();
            var mockInitialLocationSet = new Mock<DbSet<InitialLocation>>();
            var mockSegmentSet = new Mock<DbSet<Segment>>();
            var mockExternalFlightSet = new Mock<DbSet<Flight>>();
            var mockContext = new Mock<FlightContext>();

            // Linking mocked sets to elements created earlier
            mockFlightSet.As<IQueryable<Flight>>().Setup(m => m.Provider).Returns(testFlight.Provider);
            mockFlightPlanSet.As<IQueryable<FlightPlan>>().Setup(m => m.Provider).Returns(testFlightPlan.Provider);
            mockInitialLocationSet.As<IQueryable<InitialLocation>>().Setup(m => m.Provider).Returns(testInitialLocation.Provider);
            mockSegmentSet.As<IQueryable<Segment>>().Setup(m => m.Provider).Returns(testSegment.Provider);

            // Adding sets to context
            mockContext.As<IDataContext>().Setup(m => m.FlightItems).Returns(mockFlightSet.Object);
            mockContext.As<IDataContext>().Setup(m => m.FlightPlanItems).Returns(mockFlightPlanSet.Object);
            mockContext.As<IDataContext>().Setup(m => m.InitialLocationItems).Returns(mockInitialLocationSet.Object);
            mockContext.As<IDataContext>().Setup(m => m.SegmentItems).Returns(mockSegmentSet.Object);
            mockContext.As<IDataContext>().Setup(m => m.ExternalFlightItems).Returns(mockExternalFlightSet.Object);
            
            // creating mocked http client factory
            var mockClientFactory = new Mock<IHttpClientFactory>();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // mimics input from external server
            var input = "{" +
                        "{" +
                        "'flight_id': 'HA60377638', " +
                        "'longitude': 34.46026097, " +
                        "'latitude': 29.255859349999998, " +
                        "'passengers': 500, " +
                        "'company_name': 'El-Al', " +
                        "'date_time': '2020-05-27T15:05:00Z', " +
                        "'is_external': false" +
                        "}" +
                        "{" +
                        "'flight_id': 'NC5998837', " +
                        "'longitude': 30.632256631999997, " +
                        "'latitude': 33.13848776, " +
                        "'passengers': 257, " +
                        "'company_name': 'swiss', " +
                        "'date_time': '2020-05-27T15:05:00Z', " +
                        "'is_external': false" +
                        "}" +
                        "}";
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(input),
                });
            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // controller
            var controller = new FlightsController(mockContext.Object, mockClientFactory.Object); // todo ISSUE HERE
            
            var relativeTo = "2020-05-27T15:05:05Z";

            //Act
            Task.Run(async () =>
            {
                // running method and checking results
                var result = await controller.GetFlights(relativeTo) as ObjectResult;
                Assert.IsNotNull(result);
                //todo need to change thie to check if equal to input+what we added to db - maybe only check number of elements?
                Assert.AreEqual(result.Value, input);
                Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            }).GetAwaiter().GetResult();

        }
    }
}