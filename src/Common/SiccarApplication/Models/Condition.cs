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

using System.Collections.Generic;
using Json.Schema.Generation;
#nullable enable

namespace Siccar.Application
{
    /// <summary>
    /// A Siccar Condition is a JSON Logic Ruleset 
    /// </summary>
    public class Condition
    {
        /// <summary>
        /// The principal of what the condition effects
        /// </summary>
        [MaxLength(2048)]
        public string Prinicpal { get; set; } = string.Empty;

        /// <summary>
        /// under what conditions need to be evaluated
        /// </summary>
        [MinItems(1)]
        public IEnumerable<string> Criteria { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public Condition()
        {
            Criteria = new List<string>();
        }
        public Condition(bool defaultState)
        {
            Criteria = new List<string>();
            if (defaultState)
                ((List<string>)Criteria).Add("{\"!\": [false]}"); // NOT FALSE
            else
                ((List<string>)Criteria).Add("{\"!\": [true]}"); // NOT FALSE
        }
        public Condition(string principal, bool defaultState) : this(defaultState)
        {
            Prinicpal = principal;
        }

        public Condition(string principal, List<string> defaultState)  
        {
            Prinicpal = principal;
            Criteria = defaultState;
        }
    }
}
