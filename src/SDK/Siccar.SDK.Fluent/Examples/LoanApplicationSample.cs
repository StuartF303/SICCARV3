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
using Siccar.SDK.Fluent.Builders;

namespace Siccar.SDK.Fluent.Examples
{
    /// <summary>
    /// Example: Complete loan application and approval workflow
    /// </summary>
    public static class LoanApplicationSample
    {
        public static Blueprint CreateLoanApplicationBlueprint()
        {
            return BlueprintBuilder.Create()
                .WithTitle("Loan Application Process")
                .WithDescription("Complete loan application and approval workflow with conditional routing")

                // Define participants
                .AddParticipant("applicant", p => p
                    .Named("Loan Applicant")
                    .FromOrganisation("Public")
                    .WithWallet("ws1applicant123"))

                .AddParticipant("underwriter", p => p
                    .Named("Underwriter")
                    .FromOrganisation("Big Bank Corp")
                    .WithWallet("ws1underwriter456"))

                .AddParticipant("manager", p => p
                    .Named("Loan Manager")
                    .FromOrganisation("Big Bank Corp")
                    .WithWallet("ws1manager789"))

                // Action 1: Submit Application
                .AddAction("submit-application", action => action
                    .WithTitle("Submit Loan Application")
                    .WithDescription("Applicant provides loan details")
                    .SentBy("applicant")

                    .RequiresData(schema => schema
                        .AddNumber("loanAmount", f => f
                            .WithTitle("Requested Loan Amount")
                            .WithMinimum(1000)
                            .WithMaximum(1000000)
                            .IsRequired())

                        .AddString("purpose", f => f
                            .WithTitle("Loan Purpose")
                            .WithEnum("home", "auto", "business", "personal")
                            .IsRequired())

                        .AddInteger("creditScore", f => f
                            .WithTitle("Credit Score")
                            .WithMinimum(300)
                            .WithMaximum(850)
                            .IsRequired())

                        .AddNumber("annualIncome", f => f
                            .WithTitle("Annual Income")
                            .WithMinimum(0)
                            .IsRequired())

                        .AddNumber("existingDebt", f => f
                            .WithTitle("Existing Monthly Debt")
                            .WithMinimum(0)
                            .IsRequired()))

                    // Applicant sees basic info
                    .Disclose("applicant", d => d
                        .Fields("/loanAmount", "/purpose"))

                    // Underwriter sees everything
                    .Disclose("underwriter", d => d
                        .AllFields())

                    // Track loan amount and purpose for reporting
                    .TrackData("/loanAmount", "/purpose")

                    .RouteToNext("underwriter"))

                // Action 2: Underwrite
                .AddAction("underwrite", action => action
                    .WithTitle("Underwrite Application")
                    .WithDescription("Underwriter reviews and calculates risk")
                    .SentBy("underwriter")

                    .RequiresData(schema => schema
                        .AddBoolean("approved", f => f
                            .WithTitle("Approved")
                            .IsRequired())

                        .AddNumber("interestRate", f => f
                            .WithTitle("Offered Interest Rate")
                            .WithMinimum(0)
                            .WithMaximum(30))

                        .AddString("notes", f => f
                            .WithTitle("Underwriter Notes")
                            .WithMaxLength(1000)))

                    // Calculate debt-to-income ratio
                    .Calculate("debtToIncomeRatio", calc => calc
                        .WithExpression(calc.Divide(
                            calc.Variable("existingDebt"),
                            calc.Variable("annualIncome"))))

                    // Applicant sees decision
                    .Disclose("applicant", d => d
                        .Fields("/approved", "/interestRate"))

                    // Manager sees full underwriting details
                    .Disclose("manager", d => d
                        .AllFields())

                    // Route based on loan amount
                    .RouteConditionally(c => c
                        .When(logic => logic.And(
                            logic.GreaterThan("loanAmount", 100000),
                            logic.Equals("approved", true)))
                        .ThenRoute("manager")
                        .ElseRoute("applicant")))

                // Action 3: Manager Approval (for large loans)
                .AddAction("manager-approval", action => action
                    .WithTitle("Manager Approval")
                    .WithDescription("Manager reviews large loan applications")
                    .SentBy("manager")

                    .RequiresData(schema => schema
                        .AddBoolean("finalApproval", f => f
                            .WithTitle("Final Approval")
                            .IsRequired())

                        .AddString("managerNotes", f => f
                            .WithTitle("Manager Notes")
                            .WithMaxLength(1000)))

                    .Disclose("applicant", d => d
                        .Field("/finalApproval"))

                    .Disclose("underwriter", d => d
                        .AllFields())

                    .RouteToNext("applicant"))

                .Build();
        }
    }
}
