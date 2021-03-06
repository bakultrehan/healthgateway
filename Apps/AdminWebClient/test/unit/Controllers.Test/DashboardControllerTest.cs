//-------------------------------------------------------------------------
// Copyright © 2019 Province of British Columbia
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-------------------------------------------------------------------------
namespace HealthGateway.Admin.Test.Services
{
    using System;
    using System.Collections.Generic;
    using DeepEqual.Syntax;
    using HealthGateway.Admin.Controllers;
    using HealthGateway.Admin.Services;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for Admin Dashboard.
    /// </summary>
    public class DashboardControllerTest
    {
        /// <summary>
        /// GetUsersWithNotesCount should return the note count.
        /// </summary>
        [Fact]
        public void ShouldGetDependentCount()
        {
            Dictionary<DateTime, int> expected = new Dictionary<DateTime, int>() { { new DateTime(), 3 } };
            Mock<IDashboardService> mockService = new Mock<IDashboardService>();
            mockService.Setup(s => s.GetDailyDependentCount(It.IsAny<int>())).Returns(expected);
            DashboardController controller = new DashboardController(
                mockService.Object);

            IActionResult actualResult = controller.GetDependentCount(-480);
            Assert.IsType<JsonResult>(actualResult);
            Assert.True(((JsonResult)actualResult).Value.IsDeepEqual(expected));
        }
    }
}
