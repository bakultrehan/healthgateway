﻿// -------------------------------------------------------------------------
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
namespace HealthGateway.Laboratory.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// An instance of a Laboratory Report.
    /// </summary>
    public class LaboratoryReport
    {
        /// <summary>
        /// Gets or sets the media type for the report data.
        /// </summary>
        [JsonPropertyName("mediaType")]
        public string MediaType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the encoding used for the report binary data.
        /// </summary>
        [JsonPropertyName("encoding")]
        public string Encoding { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the report data.
        /// </summary>
        [JsonPropertyName("data")]
        public string Report { get; set; } = string.Empty;
    }
}
