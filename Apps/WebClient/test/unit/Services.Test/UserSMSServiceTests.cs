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
namespace HealthGateway.WebClient.Test.Services
{
    using System;
    using DeepEqual.Syntax;
    using HealthGateway.Common.Services;
    using HealthGateway.Database.Delegates;
    using HealthGateway.Database.Models;
    using HealthGateway.WebClient.Services;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for UserSMSServiceTests.
    /// </summary>
    public class UserSMSServiceTests
    {
        private const string HdIdMock = "hdIdMock";

        /// <summary>
        /// ValidateSMS - Happy path scenario.
        /// </summary>
        [Fact]
        public void ShouldValidateSMS()
        {
            var smsValidationCode = "SMSValidationCodeMock";
            MessagingVerification expectedResult = new MessagingVerification
            {
                HdId = HdIdMock,
                VerificationAttempts = 0,
                SMSValidationCode = smsValidationCode,
                ExpireDate = DateTime.Now.AddDays(1),
            };

            Mock<IMessagingVerificationDelegate> messagingVerificationDelegate = new Mock<IMessagingVerificationDelegate>();
            messagingVerificationDelegate.Setup(s => s.GetLastForUser(It.IsAny<string>(), It.IsAny<string>())).Returns(expectedResult);

            Mock<IUserProfileDelegate> userProfileDelegate = new Mock<IUserProfileDelegate>();
            var userProfileMock = new Database.Wrapper.DBResult<UserProfile>()
            {
                Payload = new UserProfile(),
                Status = Database.Constants.DBStatusCode.Read,
            };
            userProfileDelegate.Setup(s => s.GetUserProfile(It.IsAny<string>())).Returns(userProfileMock);
            userProfileDelegate.Setup(s => s.Update(It.IsAny<UserProfile>(), It.IsAny<bool>())).Returns(new Database.Wrapper.DBResult<UserProfile>());

            IUserSMSService service = new UserSMSService(
                new Mock<ILogger<UserSMSService>>().Object,
                messagingVerificationDelegate.Object,
                userProfileDelegate.Object,
                new Mock<INotificationSettingsService>().Object);

            bool actualResult = service.ValidateSMS(HdIdMock, smsValidationCode, "bearerTokenMock");

            Assert.True(actualResult);
        }

        /// <summary>
        /// ValidateSMS - Happy path scenario.
        /// </summary>
        [Fact]
        public void ShouldValidateSMSWithInvalidInvite()
        {
            var smsValidationCode = "SMSValidationCodeMock";
            MessagingVerification expectedResult = new MessagingVerification
            {
                HdId = "invalid" + HdIdMock,
                VerificationAttempts = 0,
                SMSValidationCode = smsValidationCode,
                ExpireDate = DateTime.Now.AddDays(1),
            };

            Mock<IMessagingVerificationDelegate> messagingVerificationDelegate = new Mock<IMessagingVerificationDelegate>();
            messagingVerificationDelegate.Setup(s => s.GetLastForUser(It.IsAny<string>(), It.IsAny<string>())).Returns(expectedResult);

            Mock<IUserProfileDelegate> userProfileDelegate = new Mock<IUserProfileDelegate>();
            var userProfileMock = new Database.Wrapper.DBResult<UserProfile>()
            {
                Payload = new UserProfile(),
                Status = Database.Constants.DBStatusCode.Read,
            };
            userProfileDelegate.Setup(s => s.GetUserProfile(It.IsAny<string>())).Returns(userProfileMock);
            userProfileDelegate.Setup(s => s.Update(It.IsAny<UserProfile>(), It.IsAny<bool>())).Returns(new Database.Wrapper.DBResult<UserProfile>());

            IUserSMSService service = new UserSMSService(
                new Mock<ILogger<UserSMSService>>().Object,
                messagingVerificationDelegate.Object,
                userProfileDelegate.Object,
                new Mock<INotificationSettingsService>().Object);

            bool actualResult = service.ValidateSMS(HdIdMock, smsValidationCode, "bearerTokenMock");

            Assert.True(!actualResult);
        }

        /// <summary>
        /// RetrieveLastInvite - Happy path scenario.
        /// </summary>
        [Fact]
        public void ShouldRetrieveLastInvite()
        {
            MessagingVerification expectedResult = new MessagingVerification
            {
                HdId = HdIdMock,
            };

            Mock<IMessagingVerificationDelegate> messagingVerificationDelegate = new Mock<IMessagingVerificationDelegate>();
            messagingVerificationDelegate.Setup(s => s.GetLastForUser(It.IsAny<string>(), It.IsAny<string>())).Returns(expectedResult);

            IUserSMSService service = new UserSMSService(
                new Mock<ILogger<UserSMSService>>().Object,
                messagingVerificationDelegate.Object,
                new Mock<IUserProfileDelegate>().Object,
                new Mock<INotificationSettingsService>().Object);

            MessagingVerification? actualResult = service.RetrieveLastInvite(HdIdMock);

            Assert.True(actualResult.IsDeepEqual(expectedResult));
        }
    }
}
