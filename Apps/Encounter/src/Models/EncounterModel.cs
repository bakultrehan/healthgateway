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
namespace HealthGateway.Encounter.Models
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a patient Encounter.
    /// </summary>
    public class EncounterModel
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the EncounterDate.
        /// </summary>
        [JsonPropertyName("encounterDate")]
        public DateTime EncounterDate { get; set; }

        /// <summary>
        /// Gets or sets the Specialty Description.
        /// </summary>
        [JsonPropertyName("specialtyDescription")]
        public string SpecialtyDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Practitioner Name.
        /// </summary>
        [JsonPropertyName("practitionerName")]
        public string PractitionerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Location Name.
        /// </summary>
        [JsonPropertyName("clinic")]
        public Clinic Clinic { get; set; } = new Clinic();

        /// <summary>
        /// Creates an Encounter object from an ODR model.
        /// </summary>
        /// <param name="model">The claim result to convert.</param>
        /// <returns>The newly created Encounter object.</returns>
        public static EncounterModel FromODRClaimModel(Claim model)
        {
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
#pragma warning disable SCS0006 // Weak hashing function
            using var md5CryptoService = MD5.Create();
#pragma warning restore SCS0006 // Weak hashing function
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
            StringBuilder sourceId = new StringBuilder();
            sourceId.Append($"{model.ServiceDate:yyyyMMdd}");
            sourceId.Append($"{model.SpecialtyDesc}");
            sourceId.Append($"{model.PractitionerName}");
            sourceId.Append($"{model.LocationName}");
            sourceId.Append($"{model.LocationAddress.Province}");
            sourceId.Append($"{model.LocationAddress.City}");
            sourceId.Append($"{model.LocationAddress.PostalCode}");
            sourceId.Append($"{model.LocationAddress.AddrLine1}");
            sourceId.Append($"{model.LocationAddress.AddrLine2}");
            sourceId.Append($"{model.LocationAddress.AddrLine3}");
            sourceId.Append($"{model.LocationAddress.AddrLine4}");
            return new EncounterModel()
            {
                Id = new Guid(md5CryptoService.ComputeHash(Encoding.Default.GetBytes(sourceId.ToString()))).ToString(),
                EncounterDate = model.ServiceDate,
                SpecialtyDescription = model.SpecialtyDesc,
                PractitionerName = model.PractitionerName,
                Clinic = new Clinic()
                {
                    Name = model.LocationName,
                    Province = model.LocationAddress.Province,
                    City = model.LocationAddress.City,
                    PostalCode = model.LocationAddress.PostalCode,
                    AddressLine1 = model.LocationAddress.AddrLine1,
                    AddressLine2 = model.LocationAddress.AddrLine2,
                    AddressLine3 = model.LocationAddress.AddrLine3,
                    AddressLine4 = model.LocationAddress.AddrLine4,
                },
            };
        }

        /// <summary>
        /// Creates an Encounter list from an ODR model.
        /// </summary>
        /// <param name="models">The list of ODR models to convert.</param>
        /// <returns>A list of Encounter objects.</returns>
        public static List<EncounterModel> FromODRClaimModelList(List<Claim> models)
        {
            List<EncounterModel> objects = new List<EncounterModel>();
            HashSet<string> encounterIds = new HashSet<string>();
            foreach (Claim claimModel in models)
            {
                var encounter = EncounterModel.FromODRClaimModel(claimModel);
                if (!encounterIds.Contains(encounter.Id))
                {
                    objects.Add(encounter);
                    encounterIds.Add(encounter.Id);
                }
            }

            return objects;
        }
    }
}
