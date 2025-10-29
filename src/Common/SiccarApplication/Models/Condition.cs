// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

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
