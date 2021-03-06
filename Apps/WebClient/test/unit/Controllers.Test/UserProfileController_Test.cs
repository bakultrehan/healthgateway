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
// <auto-generated />
namespace HealthGateway.WebClient.Test.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DeepEqual.Syntax;
    using HealthGateway.Common.Constants;
    using HealthGateway.Common.Models;
    using HealthGateway.Database.Models;
    using HealthGateway.WebClient.Controllers;
    using HealthGateway.WebClient.Models;
    using HealthGateway.WebClient.Services;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class UserProfileControllerTest
    {
        private const string hdid = "1234567890123456789012345678901234567890123456789012";
        private const string token = "Fake Access Token";
        private const string userId = "1001";

        [Fact]
        public void ShouldGetUserProfile()
        {
            RequestResult<UserProfileModel> expected = GetUserProfileExpectedRequestResultMock(ResultType.Success);
            IActionResult actualResult = GetUserProfile(expected, new Dictionary<string, UserPreferenceModel>() { });

            Assert.IsType<JsonResult>(actualResult);
            Assert.True(((JsonResult)actualResult).Value.IsDeepEqual(expected));
        }

        [Fact]
        public void ShouldGetUserProfileWithoutUserPreference()
        {
            RequestResult<UserProfileModel> expected = GetUserProfileExpectedRequestResultMock(ResultType.Success);
            IActionResult actualResult = GetUserProfile(expected, null);

            Assert.IsType<JsonResult>(actualResult);
            var reqResult = (RequestResult<UserProfileModel>)((JsonResult)actualResult).Value;
            Assert.Equal(ResultType.Success, reqResult.ResultStatus);
            Assert.Empty(reqResult.ResourcePayload.Preferences);
        }

        [Fact]
        public async Task ShouldCreateUserProfile()
        {
            string hdid = "1234567890123456789012345678901234567890123456789012";
            string token = "Fake Access Token";
            string userId = "1001";

            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true
            };

            CreateUserRequest createUserRequest = new CreateUserRequest
            {
                Profile = userProfile
            };

            RequestResult<UserProfileModel> expected = new RequestResult<UserProfileModel>
            {
                ResourcePayload = UserProfileModel.CreateFromDbModel(userProfile),
                ResultStatus = Common.Constants.ResultType.Success
            };

            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);

            Mock<IUserProfileService> userProfileServiceMock = new Mock<IUserProfileService>();
            userProfileServiceMock.Setup(s => s.CreateUserProfile(createUserRequest, It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(expected);
            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            Mock<IUserSMSService> smsServiceMock = new Mock<IUserSMSService>();

            UserProfileController service = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                userProfileServiceMock.Object,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                smsServiceMock.Object
            );
            IActionResult actualResult = await service.CreateUserProfile(hdid, createUserRequest);

            Assert.IsType<JsonResult>(actualResult);
            Assert.True(((JsonResult)actualResult).Value.IsDeepEqual(expected));
        }

        [Fact]
        public async Task ShouldValidateAge()
        {
            string hdid = "1234567890123456789012345678901234567890123456789012";
            string token = "Fake Access Token";
            string userId = "1001";

            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true
            };

            PrimitiveRequestResult<bool> expected = new PrimitiveRequestResult<bool>() { ResultStatus = ResultType.Success, ResourcePayload = true };
            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);

            Mock<IUserProfileService> userProfileServiceMock = new Mock<IUserProfileService>();
            userProfileServiceMock.Setup(s => s.ValidateMinimumAge(hdid)).ReturnsAsync(expected);


            UserProfileController controller = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                userProfileServiceMock.Object,
                httpContextAccessorMock.Object,
                new Mock<IUserEmailService>().Object,
                new Mock<IUserSMSService>().Object
            );
            IActionResult actualResult = await controller.Validate(hdid);

            Assert.IsType<JsonResult>(actualResult);
            Assert.Equal(expected, ((JsonResult)actualResult).Value);
        }

        [Fact]
        public void ShouldCreateUserPreference()
        {
            var userPref = new UserPreferenceModel()
            {
                HdId = hdid,
                Preference = "actionedCovidModalAt",
                Value = "Body value",
            };
            IActionResult actualResult = CreateUserPreference(userPref);

            Assert.IsType<JsonResult>(actualResult);
            RequestResult<UserPreferenceModel> reqResult = ((JsonResult)actualResult).Value as RequestResult<UserPreferenceModel>;
            Assert.NotNull(reqResult);
            Assert.Equal(ResultType.Success, reqResult.ResultStatus);
            Assert.Equal(hdid, reqResult.ResourcePayload.HdId);
            Assert.Equal(hdid, reqResult.ResourcePayload.CreatedBy);
            Assert.Equal(hdid, reqResult.ResourcePayload.UpdatedBy);
        }

        [Fact]
        public void ShouldCreateUserPreferenceWithBadRequestResultError()
        {
            IActionResult actualResult = CreateUserPreference(null);

            Assert.IsType<BadRequestResult>(actualResult);
        }

        [Fact]
        public void ShouldUpdateUserPreference()
        {
            var userPref = new UserPreferenceModel()
            {
                HdId = hdid,
                Preference = "actionedCovidModalAt",
                Value = "Body value",
            };
            IActionResult actualResult = UpdateUserPreference(userPref);

            Assert.IsType<JsonResult>(actualResult);
            RequestResult<UserPreferenceModel> reqResult = ((JsonResult)actualResult).Value as RequestResult<UserPreferenceModel>;
            Assert.NotNull(reqResult);
            Assert.Equal(ResultType.Success, reqResult.ResultStatus);
        }

        [Fact]
        public void ShouldUpdateUserPreferenceWithBadRequestResultError()
        {
            IActionResult actualResult = UpdateUserPreference(null);

            Assert.IsType<BadRequestResult>(actualResult);
        }

        [Fact]
        public void ShouldUpdateUserPreferenceWithEmtptyReferenceNameError()
        {
            var userPref = new UserPreferenceModel()
            {
                HdId = hdid,
                Preference = null,
                Value = "Body value",
            };
            IActionResult actualResult = UpdateUserPreference(userPref);

            Assert.IsType<BadRequestResult>(actualResult);
        }

        [Fact]
        public void ShouldUpdateUserPreferenceWithForbidResultError()
        {
            var userPref = new UserPreferenceModel()
            {
                HdId = hdid + "dif.",
                Preference = "valid pref name",
                Value = "Body value",
            };
            IActionResult actualResult = UpdateUserPreference(userPref);

            Assert.IsType<ForbidResult>(actualResult);
        }

        [Fact]
        public void ShouldGetLastTermsOfService()
        {
            // Setup
            var termsOfService = new TermsOfServiceModel()
            {
                Id = Guid.NewGuid(),
                Content = "abc",
                EffectiveDate = DateTime.Today
            };
            RequestResult<TermsOfServiceModel> expectedResult = new RequestResult<TermsOfServiceModel>()
            {
                ResultStatus = ResultType.Success,
                ResourcePayload = termsOfService,
            };

            Mock<IUserProfileService> userProfileServiceMock = new Mock<IUserProfileService>();
            userProfileServiceMock.Setup(s => s.GetActiveTermsOfService()).Returns(expectedResult);

            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);
            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            Mock<IUserSMSService> smsServiceMock = new Mock<IUserSMSService>();

            UserProfileController service = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                userProfileServiceMock.Object,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                smsServiceMock.Object
            );

            var actualResult = service.GetLastTermsOfService();

            Assert.IsType<JsonResult>(actualResult);
            Assert.True(((JsonResult)actualResult).Value.IsDeepEqual(expectedResult));
        }

        [Fact]
        public void ShouldGetUserEmailInvite()
        {
            // Setup
            UserEmailInvite emailInvite = new UserEmailInvite()
            {
                Id = Guid.NewGuid(),
                HdId = hdid,
                EmailAddress = "unit.test@hgw.ca",
                EmailId = Guid.NewGuid()
            };
            MessagingVerification expectedResult = new MessagingVerification()
            {
                HdId = hdid,
                InviteKey = Guid.NewGuid(),
                EmailId = Guid.NewGuid(),
                Email = new Email()
                {
                    To = "to@hgw.ca.user"
                }
            };
            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            emailServiceMock.Setup(s => s.RetrieveLastInvite(It.IsAny<string>())).Returns(expectedResult);

            Mock<IUserProfileService> userProfileServiceMock = new Mock<IUserProfileService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);
            Mock<IUserSMSService> smsServiceMock = new Mock<IUserSMSService>();

            UserProfileController service = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                userProfileServiceMock.Object,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                smsServiceMock.Object
            );

            var actualResult = service.GetUserEmailInvite(hdid);

            Assert.IsType<JsonResult>(actualResult);

            UserEmailInvite reqResult = ((JsonResult)actualResult).Value as UserEmailInvite;
            Assert.NotNull(reqResult);
            Assert.Equal(expectedResult.Email.To, reqResult.EmailAddress);
        }

        [Fact]
        public async void ShouldUpdateUserEmail()
        {
            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            emailServiceMock.Setup(s => s.UpdateUserEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>())).Returns(true);

            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);
            UserProfileController controller = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                null,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                null
            );
            IActionResult actualResult = await controller.UpdateUserEmail(hdid, "emailadd@hgw.ca");

            Assert.True((bool)((JsonResult)actualResult).Value);
        }

        [Fact]
        public async void ShouldValidateEmail()
        {
            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            emailServiceMock.Setup(s => s.ValidateEmail(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(true);

            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);
            UserProfileController controller = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                null,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                null
            );
            IActionResult actualResult = await controller.ValidateEmail(hdid, Guid.NewGuid());
            Assert.IsType<OkResult>(actualResult);
        }

        [Fact]
        public async void ShouldValidateEmailWithEmailNotFound()
        {
            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            emailServiceMock.Setup(s => s.ValidateEmail(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>())).Returns(false);

            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);
            UserProfileController controller = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                null,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                null
            );
            IActionResult actualResult = await controller.ValidateEmail(hdid, Guid.NewGuid());
            Assert.IsType<NotFoundResult>(actualResult);
        }

        [Fact]
        public async void ShouldUpdateUserSMSNumber()
        {
            Mock<IUserSMSService> emailServiceMock = new Mock<IUserSMSService>();
            emailServiceMock.Setup(s => s.UpdateUserSMS(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>())).Returns(true);

            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);
            UserProfileController controller = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                null,
                httpContextAccessorMock.Object,
                null,
                emailServiceMock.Object
            );
            IActionResult actualResult = await controller.UpdateUserSMSNumber(hdid, "250 123 456");

            Assert.True((bool)((JsonResult)actualResult).Value);
        }

        [Fact]
        public async void ShouldValidateSms()
        {
            Mock<IUserSMSService> emailServiceMock = new Mock<IUserSMSService>();
            emailServiceMock.Setup(s => s.ValidateSMS(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);
            UserProfileController controller = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                null,
                httpContextAccessorMock.Object,
                null,
                emailServiceMock.Object
            );
            IActionResult actualResult = await controller.ValidateSMS(hdid, "205 123 4567");
            Assert.IsType<OkResult>(actualResult);
        }

        [Fact]
        public async void ShouldValidateSmsNotFoundResult()
        {
            Mock<IUserSMSService> emailServiceMock = new Mock<IUserSMSService>();
            emailServiceMock.Setup(s => s.ValidateSMS(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);
            UserProfileController controller = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                null,
                httpContextAccessorMock.Object,
                null,
                emailServiceMock.Object
            );
            IActionResult actualResult = await controller.ValidateSMS(hdid, "205 123 4567");
            Assert.IsType<NotFoundResult>(actualResult);
        }

        [Fact]
        public void ShouldGetUserSMSInvite()
        {
            // Setup
            UserEmailInvite emailInvite = new UserEmailInvite()
            {
                Id = Guid.NewGuid(),
                HdId = hdid,
                EmailAddress = "unit.test@hgw.ca",
                EmailId = Guid.NewGuid()
            };
            MessagingVerification expectedResult = new MessagingVerification()
            {
                HdId = hdid,
                InviteKey = Guid.NewGuid(),
                SMSNumber = "250 123 4567",
                ExpireDate = DateTime.Now.AddDays(1)
            };

            Mock<IUserSMSService> smsServiceMock = new Mock<IUserSMSService>();
            smsServiceMock.Setup(s => s.RetrieveLastInvite(It.IsAny<string>())).Returns(expectedResult);

            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            Mock<IUserProfileService> userProfileServiceMock = new Mock<IUserProfileService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);

            UserProfileController service = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                userProfileServiceMock.Object,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                smsServiceMock.Object
            );

            var actualResult = service.GetUserSMSInvite(hdid);

            Assert.IsType<JsonResult>(actualResult);

            UserSMSInvite reqResult = ((JsonResult)actualResult).Value as UserSMSInvite;
            Assert.NotNull(reqResult);
            Assert.Equal(expectedResult.SMSNumber, reqResult.SMSNumber);
            Assert.True(!reqResult.Expired);
        }

        private static RequestResult<UserProfileModel> GetUserProfileExpectedRequestResultMock(ResultType resultType)
        {
            UserProfile userProfile = new UserProfile
            {
                HdId = hdid,
                AcceptedTermsOfService = true
            };

            return new RequestResult<UserProfileModel>
            {
                ResourcePayload = UserProfileModel.CreateFromDbModel(userProfile),
                ResultStatus = Common.Constants.ResultType.Success
            };
        }

        private IActionResult GetUserProfile(RequestResult<UserProfileModel> expected, Dictionary<string, UserPreferenceModel> userPreferencePayloadMock)
        {
            // Setup
            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);

            Mock<IUserProfileService> userProfileServiceMock = new Mock<IUserProfileService>();
            userProfileServiceMock.Setup(s => s.GetUserProfile(hdid, It.IsAny<DateTime>())).Returns(expected);
            userProfileServiceMock.Setup(s => s.GetActiveTermsOfService()).Returns(new RequestResult<TermsOfServiceModel>());
            userProfileServiceMock.Setup(s => s.GetUserPreferences(hdid)).Returns(new RequestResult<Dictionary<string, UserPreferenceModel>>() { ResourcePayload = userPreferencePayloadMock });

            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            Mock<IUserSMSService> smsServiceMock = new Mock<IUserSMSService>();

            UserProfileController service = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                userProfileServiceMock.Object,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                smsServiceMock.Object
            );
            return service.GetUserProfile(hdid);
        }

        private IActionResult UpdateUserPreference(UserPreferenceModel userPref)
        {
            // Setup
            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);

            Mock<IUserProfileService> userProfileServiceMock = new Mock<IUserProfileService>();
            RequestResult<UserPreferenceModel> result = new RequestResult<UserPreferenceModel>()
            {
                ResourcePayload = userPref,
                ResultStatus = ResultType.Success,
            };

            userProfileServiceMock.Setup(s => s.UpdateUserPreference(userPref)).Returns(result);

            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            Mock<IUserSMSService> smsServiceMock = new Mock<IUserSMSService>();

            UserProfileController service = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                userProfileServiceMock.Object,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                smsServiceMock.Object
            );
            return service.UpdateUserPreference(hdid, userPref);
        }

        private IActionResult CreateUserPreference(UserPreferenceModel userPref)
        {
            // Setup
            Mock<IHttpContextAccessor> httpContextAccessorMock = CreateValidHttpContext(token, userId, hdid);

            Mock<IUserProfileService> userProfileServiceMock = new Mock<IUserProfileService>();
            RequestResult<UserPreferenceModel> result = new RequestResult<UserPreferenceModel>()
            {
                ResourcePayload = userPref,
                ResultStatus = ResultType.Success,
            };

            userProfileServiceMock.Setup(s => s.CreateUserPreference(userPref)).Returns(result);

            Mock<IUserEmailService> emailServiceMock = new Mock<IUserEmailService>();
            Mock<IUserSMSService> smsServiceMock = new Mock<IUserSMSService>();

            UserProfileController service = new UserProfileController(
                new Mock<ILogger<UserProfileController>>().Object,
                userProfileServiceMock.Object,
                httpContextAccessorMock.Object,
                emailServiceMock.Object,
                smsServiceMock.Object
            );
            return service.CreateUserPreference(hdid, userPref);
        }

        private Mock<IHttpContextAccessor> CreateValidHttpContext(string token, string userId, string hdid)
        {
            IHeaderDictionary headerDictionary = new HeaderDictionary();
            headerDictionary.Add("Authorization", token);
            headerDictionary.Add("referer", "http://localhost/");
            Mock<HttpRequest> httpRequestMock = new Mock<HttpRequest>();
            httpRequestMock.Setup(s => s.Headers).Returns(headerDictionary);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("hdid", hdid),
                new Claim("auth_time", "123"),
                new Claim("access_token", token)
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, "TestAuth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(s => s.User).Returns(claimsPrincipal);
            httpContextMock.Setup(s => s.Request).Returns(httpRequestMock.Object);
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(s => s.HttpContext).Returns(httpContextMock.Object);
            Mock<IAuthenticationService> authenticationMock = new Mock<IAuthenticationService>();
            var authResult = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme));
            authResult.Properties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = token }
            });
            authenticationMock
                .Setup(x => x.AuthenticateAsync(httpContextAccessorMock.Object.HttpContext, It.IsAny<string>()))
                .ReturnsAsync(authResult);

            httpContextAccessorMock
                .Setup(x => x.HttpContext.RequestServices.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationMock.Object);
            return httpContextAccessorMock;
        }
    }
}
