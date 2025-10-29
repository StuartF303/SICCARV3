// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Common
{
    public class ODataResponse<T>
    {
        public T Value { get; set; }
    }
}
