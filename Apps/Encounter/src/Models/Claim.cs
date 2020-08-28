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

    /// <summary>
    /// Represents a row in the MSP Visit Claim.
    /// </summary>
    public class Claim
    {
        /// <summary>
        /// Gets or sets the ClaimId.
        /// </summary>
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the Service Date.
        /// </summary>
        public DateTime ServiceDate { get; set; }

        /// <summary>
        /// Gets or sets the Fee Desc.
        /// </summary>
        public string FeeDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Diagnostic Code.
        /// </summary>
        public DiagnosticCode DiagnosticCode { get; set; } = new DiagnosticCode();

        /// <summary>
        /// Gets or sets the Specialty Desc.
        /// </summary>
        public string SpecialtyDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Practitioner Number.
        /// </summary>
        public string PractitionerNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Location Name.
        /// </summary>
        public string LocationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Location Address.
        /// </summary>
        public LocationAddress LocationAddress { get; set; } = new LocationAddress();
    }
}
