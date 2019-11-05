﻿//-------------------------------------------------------------------------
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
namespace HealthGateway.Medication.Models
{
    using System;
    using System.Collections.Generic;
    using HealthGateway.Database.Models;

    /// <summary>
    /// The medications data model.
    /// </summary>
    public class MedicationResult
    {
        /// <summary>
        /// Gets or sets the Drug Identification Number for the prescribed medication.
        /// </summary>
        public string DIN { get; set; }

        /// <summary>
        /// Gets or sets the Federal Drug Data Source.
        /// </summary>
        public FederalDrugSource FederalData { get; set; }

        /// <summary>
        /// Gets or sets the Provincial Drug Data Source.
        /// </summary>
        public ProvincialDrugSource ProvincialData { get; set; }
        /// <summary>
        /// Federal Drug Source.
        /// </summary>
        public class FederalDrugSource
        {
            /// <summary>
            /// Gets or sets the drug updated date/time.
            /// </summary>
            public DateTime UpdateDateTime;

            /// <summary>
            /// Gets or sets the drug product.
            /// </summary>
            public DrugProduct DrugProduct { get; set; }

            /// <summary>
            /// Gets or sets a list of forms.
            /// </summary>
            public List<Form> Forms { get; set; }

            /// <summary>
            /// Gets or sets a list of active ingredients.
            /// </summary>
            public List<ActiveIngredient> ActiveIngredients;

            /// <summary>
            /// Gets or sets a list of companies.
            /// </summary>
            public List<Company> Companies { get; set; }
        }

        /// <summary>
        /// Provincial Drug information Source class.
        /// </summary>
        public class ProvincialDrugSource
        {
            /// <summary>
            ///  The update date/time.
            /// </summary>
            public DateTime UpdateDateTime;

            /// <summary>
            ///  Gets or sets a <see cref="PharmaCareDrug"/> instance.
            /// </summary>
            public PharmaCareDrug PharmaCareDrug { get; set; }
        }
    }
}