// -------------------------------------------------------------------------
//  Copyright © 2019 Province of British Columbia
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// -------------------------------------------------------------------------
namespace HealthGateway.CommonTests.Utils
{
    using System.IO;
    using System.Security.Claims;
    using System.Text;
    using HealthGateway.Common.Utils;
    using Newtonsoft.Json;
    using Xunit;

    /// <summary>
    /// JsonClaimsIdentityConverterTest.
    /// </summary>
    public class JsonClaimsIdentityConverterTest
    {
        /// <summary>
        /// ShouldCanConvertClaim.
        /// </summary>
        [Fact]
        public void ShouldCanConvertClaimsIdentity()
        {
            JsonClaimsIdentityConverter jsonClaimsIdentityConverter = new JsonClaimsIdentityConverter();
            var actualResult = jsonClaimsIdentityConverter.CanConvert(typeof(ClaimsIdentity));
            Assert.True(actualResult);
        }

        /// <summary>
        /// ShouldCanConvertNotClaim.
        /// </summary>
        [Fact]
        public void ShouldCanConvertNotClaimsIdentity()
        {
            JsonClaimsIdentityConverter jsonClaimsIdentityConverter = new JsonClaimsIdentityConverter();
            var actualResult = jsonClaimsIdentityConverter.CanConvert(typeof(string));
            Assert.False(actualResult);
        }

        /// <summary>
        /// ShouldWriteJson.
        /// </summary>
        [Fact]
        public void ShouldWriteJson()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                ClaimsIdentity claimsIdentity = new ClaimsIdentity("mockAuthenticationType", "mockNameType", "mockRoleType");
                JsonSerializer jsonSerializer = new JsonSerializer();

                JsonClaimsIdentityConverter jsonClaimsIdentityConverter = new JsonClaimsIdentityConverter();
                jsonClaimsIdentityConverter.WriteJson(writer, claimsIdentity, jsonSerializer);
            }
        }
    }
}
