// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

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
