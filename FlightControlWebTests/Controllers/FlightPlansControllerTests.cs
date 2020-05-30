using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using FlightControlWeb.Controllers;
using FlightControlWeb.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace FlightControlWeb.Controllers.Tests
{
    public class FlightPlanControllerTests
    {

        public static readonly DbContextOptionsBuilder<FlightContext> fdbx = new DbContextOptionsBuilder<FlightContext>();
        public static readonly IFlightManager fm = new FlightManager();
        public static readonly IFlightPlanManager fpm = new FlightPlanManager();
        //public static readonly IHttpClientFactory cf;
        public readonly Mock<FlightContext> mockContext = new Mock<FlightContext>(fdbx.Options);
        public readonly Mock<FlightManager> mockFM = new Mock<FlightManager>(fm);
        public readonly Mock<FlightPlanManager> mockFPM = new Mock<FlightPlanManager>(fpm);
        //public readonly Mock<IHttpClientFactory> 
        dynamic mockClientFactory = new Mock<IHttpClientFactory>();
        private readonly FlightPlansController _fpc;
        public FlightPlanControllerTests()
        {
            _fpc = new FlightPlansController(mockContext.Object, fm, fpm, mockClientFactory);
        }

        [Fact]
        public async Task GetFlightPlan_ShouldReturnFlightPlan_WhenFlightPlanExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<FlightContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new FlightContext(options);
            var flightPlanId = Guid.NewGuid();
            var flightPlanDto = new FlightPlan
            {
                FlightId = flightPlanId.ToString()
            };
            mockContext.Setup(x => x.FlightPlanItems.FindAsync(flightPlanId.ToString()))
                .ReturnsAsync(flightPlanDto);
            // Act
            var flightPlan = await _fpc.GetFlightPlan(flightPlanId.ToString());
            // Assert
            Assert.Equals(flightPlanId.ToString(), flightPlan.Value);
        }
    }
}