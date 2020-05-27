using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlightControlWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FlightControlWeb.Models;
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
        public void DeleteFlightTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetFlightsTest()
        {
            var mockFlightSet = new Mock<DbSet<Flight>>();
            var mockFlightPlanSet = new Mock<DbSet<FlightPlan>>();
            var mockInitialLocationSet = new Mock<DbSet<InitialLocation>>();
            var mockSegmentSet = new Mock<DbSet<Segment>>();
            var mockExternalFlightSet = new Mock<DbSet<Flight>>();
            var mockContext = new Mock<FlightContext>();
            
            mockContext.Setup(m => m.FlightItems).Returns(mockFlightSet.Object);
            mockContext.Setup(m => m.FlightPlanItems).Returns(mockFlightPlanSet.Object);
            mockContext.Setup(m => m.InitialLocationItems).Returns(mockInitialLocationSet.Object);
            mockContext.Setup(m => m.SegmentItems).Returns(mockSegmentSet.Object);
            mockContext.Setup(m => m.ExternalFlightItems).Returns(mockExternalFlightSet.Object);

            var mockClientFactory = new Mock<IHttpClientFactory>();
            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{" +
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
                                                "}"),
                });

            var controller = new FlightsController(mockContext.Object, mockClientFactory.Object);

            Assert.Fail();
        }
    }
}