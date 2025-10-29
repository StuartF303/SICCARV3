// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

namespace Siccar.Common.ServiceClients
{
    public class ValidatorServiceClient 
    {
        private readonly SiccarBaseClient _baseClient;

        public ValidatorServiceClient(SiccarBaseClient baseClient)
        {
            _baseClient = baseClient;
        }
    }
}
