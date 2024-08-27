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

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siccar.Application.Client.Services
{

    public class HeroDemoServicesLogic
    {

        private readonly DataSubmissionService DataService;

        public bool IsHero { get; set; } = false;

        public HeroDemoServicesLogic(IConfiguration configuration, DataSubmissionService dataSubmission)
        {
            IsHero = bool.Parse(configuration["IsHeroDemo"]);
            DataService = dataSubmission;
        }

        public void UpdateForm(int actionid)
        {
            if (!IsHero)
                return;

            if (actionid == 1)
            {
                DataService.UpdateFormData("passportNumber", Guid.NewGuid().ToString("N"));
            }

            if (actionid == 6)
            {
                DataService.UpdateFormData("assuranceReg", Guid.NewGuid().ToString("N"));
            }
        }

    }   

}
