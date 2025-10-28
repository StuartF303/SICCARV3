# Siccar Fluent SDK - Quick Start Guide

## What Was Created

The Siccar Fluent SDK is now fully implemented with the following structure:

```
Siccar.SDK.Fluent/
├── Builders/
│   ├── BlueprintBuilder.cs          ✅ Main entry point for building blueprints
│   ├── ParticipantBuilder.cs        ✅ Fluent participant definition
│   ├── ActionBuilder.cs             ✅ Fluent action configuration
│   ├── DisclosureBuilder.cs         ✅ Selective data disclosure
│   ├── DataSchemaBuilder.cs         ✅ JSON Schema creation
│   ├── FieldBuilders.cs             ✅ All field types (String, Number, Integer, etc.)
│   ├── ConditionBuilder.cs          ✅ Conditional routing with JSON Logic
│   ├── CalculationBuilder.cs        ✅ Calculations using JSON Logic
│   └── FormBuilder.cs               ✅ UI form generation
├── Extensions/
│   └── ValidationExtensions.cs      ✅ Inline validation helpers
├── Models/
│   ├── FluentBlueprintContext.cs    ✅ Internal state management
│   └── FluentValidationException.cs ✅ Custom exception type
├── Examples/
│   ├── LoanApplicationSample.cs     ✅ Complete loan workflow example
│   └── SimpleSamples.cs             ✅ Simple workflow examples
├── README.md                        ✅ Full documentation
├── QUICKSTART.md                    ✅ This file
└── Siccar.SDK.Fluent.csproj        ✅ Project file
```

## Installation

The project has been added to the SICCARV3 solution. To use it:

```bash
# From another project in the solution
dotnet add reference ../SDK/Siccar.SDK.Fluent/Siccar.SDK.Fluent.csproj

# Or via NuGet (once published)
dotnet add package Siccar.SDK.Fluent
```

## Basic Usage

### 1. Create a Simple Two-Party Workflow

```csharp
using Siccar.SDK.Fluent.Builders;

var blueprint = BlueprintBuilder.Create()
    .WithTitle("Purchase Approval")
    .WithDescription("Two-party approval process")

    // Define participants
    .AddParticipant("requester", p => p
        .Named("Employee")
        .FromOrganisation("Acme Corp")
        .WithWallet("ws1requester123"))

    .AddParticipant("approver", p => p
        .Named("Manager")
        .FromOrganisation("Acme Corp")
        .WithWallet("ws1approver456"))

    // First action: Submit request
    .AddAction("submit-request", action => action
        .WithTitle("Submit Purchase Request")
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

        .RouteToNext("approver"))

    // Second action: Approve or reject
    .AddAction("review", action => action
        .WithTitle("Review Request")
        .SentBy("approver")

        .RequiresData(schema => schema
            .AddBoolean("approved", f => f.IsRequired())
            .AddString("comments"))

        .Disclose("requester", d => d.AllFields())
        .Disclose("approver", d => d.AllFields())

        .RouteToNext("requester"))

    .Build(); // Validates and creates the blueprint
```

### 2. Selective Data Disclosure Example

```csharp
var blueprint = BlueprintBuilder.Create()
    .WithTitle("Medical Referral")

    .AddParticipant("patient", p => p
        .Named("Patient")
        .UseStealthAddress())

    .AddParticipant("doctor", p => p
        .Named("Dr. Smith")
        .WithWallet("ws1doctor"))

    .AddParticipant("specialist", p => p
        .Named("Specialist")
        .WithWallet("ws1specialist"))

    .AddAction("consultation", action => action
        .SentBy("doctor")

        .RequiresData(schema => schema
            .AddString("patientName")
            .AddString("symptoms")
            .AddString("diagnosis")
            .AddString("medicalHistory"))

        // Patient sees limited info
        .Disclose("patient", d => d
            .Fields("/symptoms", "/diagnosis"))

        // Specialist sees everything
        .Disclose("specialist", d => d
            .AllFields())

        // Track diagnosis for audit trail
        .TrackData("/diagnosis")

        .RouteToNext("specialist"))

    .Build();
```

### 3. Conditional Routing Example

```csharp
var blueprint = BlueprintBuilder.Create()
    .WithTitle("Loan Application")

    .AddParticipant("applicant", p => p.WithWallet("ws1app"))
    .AddParticipant("juniorOfficer", p => p.WithWallet("ws1junior"))
    .AddParticipant("seniorOfficer", p => p.WithWallet("ws1senior"))

    .AddAction("submit", action => action
        .SentBy("applicant")

        .RequiresData(schema => schema
            .AddNumber("loanAmount", f => f
                .WithMinimum(1000)
                .WithMaximum(1000000)
                .IsRequired()))

        .Disclose("applicant", d => d.AllFields())
        .Disclose("juniorOfficer", d => d.AllFields())
        .Disclose("seniorOfficer", d => d.AllFields())

        // Route based on loan amount
        .RouteConditionally(c => c
            .When(logic => logic.GreaterThan("loanAmount", 100000))
            .ThenRoute("seniorOfficer")
            .ElseRoute("juniorOfficer")))

    .Build();
```

### 4. Calculations Example

```csharp
.AddAction("calculate-loan", action => action
    .SentBy("applicant")

    .RequiresData(schema => schema
        .AddNumber("monthlyIncome")
        .AddNumber("monthlyDebt")
        .AddNumber("loanAmount")
        .AddNumber("interestRate")
        .AddInteger("termMonths"))

    // Calculate debt-to-income ratio
    .Calculate("debtToIncomeRatio", calc => calc
        .WithExpression(calc.Divide(
            calc.Variable("monthlyDebt"),
            calc.Variable("monthlyIncome"))))

    // Calculate monthly payment
    .Calculate("monthlyPayment", calc => calc
        .WithExpression(calc.Divide(
            calc.Variable("loanAmount"),
            calc.Variable("termMonths"))))

    .RouteToNext("underwriter"))
```

## Field Types Reference

```csharp
.RequiresData(schema => schema

    // String field
    .AddString("firstName", f => f
        .WithTitle("First Name")
        .WithMinLength(1)
        .WithMaxLength(50)
        .WithPattern("^[A-Za-z]+$")
        .WithFormat("email") // or "uri", "date-time"
        .IsRequired())

    // Number field (decimal)
    .AddNumber("salary", f => f
        .WithMinimum(0)
        .WithMaximum(1000000)
        .WithMultipleOf(0.01))

    // Integer field
    .AddInteger("age", f => f
        .WithMinimum(18)
        .WithMaximum(120))

    // Boolean field
    .AddBoolean("employed", f => f
        .WithDefault(false))

    // Date field
    .AddDate("birthDate", f => f
        .WithTitle("Date of Birth"))

    // File field
    .AddFile("resume", f => f
        .WithMaxSize(5 * 1024 * 1024) // 5MB
        .WithAllowedExtensions(".pdf", ".docx"))

    // Object field
    .AddObject("address", obj => obj
        .WithTitle("Address")
        .AddProperty("street", "string")
        .AddProperty("city", "string")
        .AddProperty("zipCode", "string"))

    // Array field
    .AddArray("skills", arr => arr
        .OfType("string")
        .WithMinItems(1)
        .WithMaxItems(10)))
```

## JSON Logic Operators

### Comparison
```csharp
logic.GreaterThan("amount", 1000)
logic.GreaterThanOrEqual("score", 700)
logic.LessThan("age", 65)
logic.LessThanOrEqual("debt", 50000)
logic.Equals("status", "approved")
logic.NotEquals("region", "excluded")
```

### Logical
```csharp
logic.And(
    logic.GreaterThan("score", 700),
    logic.LessThan("debt", 50000))

logic.Or(
    logic.Equals("status", "approved"),
    logic.Equals("status", "pending"))

logic.Not(logic.Equals("rejected", true))
```

### Arithmetic (Calculations)
```csharp
calc.Add(calc.Variable("price"), calc.Constant(100))
calc.Subtract(calc.Variable("total"), calc.Variable("discount"))
calc.Multiply(calc.Variable("quantity"), calc.Variable("price"))
calc.Divide(calc.Variable("total"), calc.Constant(12))
calc.Modulo(calc.Variable("number"), calc.Constant(10))
```

## Running the Examples

The SDK includes complete examples that you can run:

```csharp
using Siccar.SDK.Fluent.Examples;

// Simple approval workflow
var simpleBlueprint = SimpleSamples.CreateSimpleApprovalWorkflow();

// Medical referral with privacy
var medicalBlueprint = SimpleSamples.CreateMedicalReferralWorkflow();

// Document signing
var docBlueprint = SimpleSamples.CreateDocumentSigningWorkflow();

// Complex loan application
var loanBlueprint = LoanApplicationSample.CreateLoanApplicationBlueprint();
```

## Publishing to SICCAR

Once you've built your blueprint, you can publish it:

```csharp
using Siccar.Common.ServiceClients;

// Inject or create the blueprint service client
var blueprintClient = serviceProvider.GetRequiredService<IBlueprintServiceClient>();

// Build your blueprint
var blueprint = BlueprintBuilder.Create()
    // ... configure blueprint
    .Build();

// Publish to SICCAR
var response = await blueprintClient.PublishBlueprintAsync(blueprint);
```

## Next Steps

1. **Explore Examples**: Check out the files in `Examples/` folder
2. **Read Full Documentation**: See [README.md](README.md) for complete API reference
3. **Integrate**: Reference this project from your applications
4. **Extend**: Create custom builder extensions for your use cases

## Support

For issues and questions:
- GitHub Issues: https://github.com/siccar/SICCARV3/issues
- Documentation: See README.md in this folder

## License

Siccar Proprietary Limited Use License - See [LICENSE](../../../LICENCE.txt)
