/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

using Siccar.Application;

namespace Siccar.SDK.Fluent.Builders
{
    /// <summary>
    /// Fluent builder for defining data disclosure rules
    /// </summary>
    public class DisclosureBuilder
    {
        private readonly string _participantId;
        private readonly List<string> _dataPointers;

        internal DisclosureBuilder(string participantId)
        {
            _participantId = participantId;
            _dataPointers = new List<string>();
        }

        /// <summary>
        /// Specifies multiple data fields to disclose using JSON Pointer syntax
        /// </summary>
        /// <param name="jsonPointers">JSON Pointers to data fields (e.g., "/firstName", "/address/zipCode")</param>
        /// <returns>The builder instance for chaining</returns>
        public DisclosureBuilder Fields(params string[] jsonPointers)
        {
            foreach (var pointer in jsonPointers)
            {
                // Ensure proper JSON Pointer format
                var formattedPointer = pointer.StartsWith("/") || pointer.StartsWith("#")
                    ? pointer
                    : $"/{pointer}";
                _dataPointers.Add(formattedPointer);
            }
            return this;
        }

        /// <summary>
        /// Specifies a single data field to disclose
        /// </summary>
        /// <param name="jsonPointer">JSON Pointer to a data field</param>
        /// <returns>The builder instance for chaining</returns>
        public DisclosureBuilder Field(string jsonPointer)
        {
            var formattedPointer = jsonPointer.StartsWith("/") || jsonPointer.StartsWith("#")
                ? jsonPointer
                : $"/{jsonPointer}";
            _dataPointers.Add(formattedPointer);
            return this;
        }

        /// <summary>
        /// Discloses all data fields to this participant
        /// </summary>
        /// <returns>The builder instance for chaining</returns>
        public DisclosureBuilder AllFields()
        {
            // Special marker to disclose all data
            _dataPointers.Add("/*");
            return this;
        }

        internal Disclosure Build()
        {
            return new Disclosure(_participantId, _dataPointers);
        }
    }
}
