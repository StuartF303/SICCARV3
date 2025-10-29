// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using Siccar.Application;
using Siccar.SDK.Fluent.Builders;

namespace Siccar.SDK.Fluent.Examples
{
    /// <summary>
    /// Simple examples demonstrating basic fluent SDK usage
    /// </summary>
    public static class SimpleSamples
    {
        /// <summary>
        /// Simple two-party approval workflow
        /// </summary>
        public static Blueprint CreateSimpleApprovalWorkflow()
        {
            return BlueprintBuilder.Create()
                .WithTitle("Purchase Request Approval")
                .WithDescription("Simple two-party approval process")

                .AddParticipant("requester", p => p
                    .Named("Requester")
                    .FromOrganisation("Company")
                    .WithWallet("ws1requester123"))

                .AddParticipant("approver", p => p
                    .Named("Approver")
                    .FromOrganisation("Company")
                    .WithWallet("ws1approver456"))

                .AddAction("submit-request", action => action
                    .WithTitle("Submit Request")
                    .SentBy("requester")

                    .RequiresData(schema => schema
                        .AddString("itemDescription", f => f
                            .WithTitle("Item Description")
                            .IsRequired())
                        .AddNumber("amount", f => f
                            .WithTitle("Amount")
                            .WithMinimum(0)
                            .IsRequired()))

                    .Disclose("requester", d => d.AllFields())
                    .Disclose("approver", d => d.AllFields())
                    .TrackData("/amount")

                    .RouteToNext("approver"))

                .AddAction("approve-or-reject", action => action
                    .WithTitle("Approve or Reject")
                    .SentBy("approver")

                    .RequiresData(schema => schema
                        .AddBoolean("approved", f => f
                            .WithTitle("Approved")
                            .IsRequired())
                        .AddString("comments", f => f
                            .WithTitle("Comments")))

                    .Disclose("requester", d => d.AllFields())
                    .Disclose("approver", d => d.AllFields())

                    .RouteToNext("requester"))

                .Build();
        }

        /// <summary>
        /// Medical referral with privacy controls
        /// </summary>
        public static Blueprint CreateMedicalReferralWorkflow()
        {
            return BlueprintBuilder.Create()
                .WithTitle("Medical Referral")
                .WithDescription("Patient referral with selective data disclosure")

                .AddParticipant("patient", p => p
                    .Named("Patient")
                    .FromOrganisation("Public")
                    .WithWallet("ws1patient123")
                    .UseStealthAddress())

                .AddParticipant("doctor", p => p
                    .Named("Primary Care Doctor")
                    .FromOrganisation("City Hospital")
                    .WithWallet("ws1doctor456"))

                .AddParticipant("specialist", p => p
                    .Named("Specialist")
                    .FromOrganisation("Specialist Clinic")
                    .WithWallet("ws1specialist789"))

                .AddAction("initial-consultation", action => action
                    .WithTitle("Initial Consultation")
                    .SentBy("doctor")

                    .RequiresData(schema => schema
                        .AddString("patientName", f => f.IsRequired())
                        .AddString("symptoms", f => f.IsRequired())
                        .AddString("diagnosis", f => f.IsRequired())
                        .AddInteger("severity", f => f
                            .WithMinimum(1)
                            .WithMaximum(5)))

                    // Patient sees symptoms and diagnosis
                    .Disclose("patient", d => d
                        .Fields("/symptoms", "/diagnosis"))

                    // Specialist sees everything
                    .Disclose("specialist", d => d.AllFields())

                    // Track diagnosis for audit
                    .TrackData("/diagnosis")

                    // Route to specialist if severity > 3
                    .RouteConditionally(c => c
                        .When(logic => logic.GreaterThan("severity", 3))
                        .ThenRoute("specialist")
                        .ElseRoute("patient")))

                .Build();
        }

        /// <summary>
        /// Document signing workflow
        /// </summary>
        public static Blueprint CreateDocumentSigningWorkflow()
        {
            return BlueprintBuilder.Create()
                .WithTitle("Document Signing")
                .WithDescription("Multi-party document signing workflow")

                .AddParticipant("author", p => p
                    .Named("Document Author")
                    .FromOrganisation("Company A")
                    .WithWallet("ws1author123"))

                .AddParticipant("reviewer", p => p
                    .Named("Legal Reviewer")
                    .FromOrganisation("Company B")
                    .WithWallet("ws1reviewer456"))

                .AddParticipant("signer", p => p
                    .Named("Authorized Signer")
                    .FromOrganisation("Company B")
                    .WithWallet("ws1signer789"))

                .AddAction("upload-document", action => action
                    .WithTitle("Upload Document")
                    .SentBy("author")

                    .RequiresData(schema => schema
                        .AddString("documentTitle", f => f.IsRequired())
                        .AddFile("documentFile", f => f
                            .WithTitle("Document")
                            .WithMaxSize(10 * 1024 * 1024) // 10MB
                            .WithAllowedExtensions(".pdf", ".docx")
                            .IsRequired()))

                    .Disclose("author", d => d.AllFields())
                    .Disclose("reviewer", d => d.AllFields())
                    .MakePublic("/documentTitle") // Title is public

                    .RouteToNext("reviewer"))

                .AddAction("review-document", action => action
                    .WithTitle("Review Document")
                    .SentBy("reviewer")

                    .RequiresData(schema => schema
                        .AddBoolean("approved", f => f.IsRequired())
                        .AddString("reviewNotes", f => f))

                    .Disclose("author", d => d.AllFields())
                    .Disclose("signer", d => d.AllFields())

                    .RouteConditionally(c => c
                        .When(logic => logic.Equals("approved", true))
                        .ThenRoute("signer")
                        .ElseRoute("author")))

                .AddAction("sign-document", action => action
                    .WithTitle("Sign Document")
                    .SentBy("signer")

                    .RequiresData(schema => schema
                        .AddBoolean("signed", f => f.IsRequired())
                        .AddDate("signedDate", f => f.IsRequired())
                        .AddString("signerName", f => f.IsRequired()))

                    .Disclose("author", d => d.AllFields())
                    .Disclose("reviewer", d => d.AllFields())
                    .Disclose("signer", d => d.AllFields())

                    .AlsoSendTo("author", "reviewer"))

                .Build();
        }
    }
}
