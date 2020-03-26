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
namespace HealthGateway.Medication.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HealthGateway.Common.Models;
    using HealthGateway.Medication.Models;
    using HealthGateway.Medication.Models.ODR;

    /// <summary>
    /// The Medication data service.
    /// </summary>
    public interface IMedicationStatementService
    {
        /// <summary>
        /// Gets the patient medication statemets.
        /// </summary>
        /// <param name="hdid">The hdid to retrieve records for.</param>
        /// <param name="protectiveWord">The protective word.</param>
        /// <returns>A List of MedicationStatement models.</returns>
        Task<RequestResult<List<MedicationStatement>>> GetMedicationStatements(string hdid, string? protectiveWord);

        /// <summary>
        /// Gets the patient medication history.
        /// </summary>
        /// <param name="hdid">The hdid to retrieve records for.</param>
        /// <param name="protectiveWord">The protective word.</param>
        /// <returns>A MedicationHistoryResponse models.</returns>
        Task<RequestResult<List<MedicationStatementHistory>>> GetMedicationStatementsHistory(string hdid, string? protectiveWord);
    }
}