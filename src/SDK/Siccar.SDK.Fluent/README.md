# Siccar Fluent SDK

A fluent, intuitive API for building SICCAR Blueprints with strongly-typed, chainable methods.

## Overview

The Siccar Fluent SDK transforms the verbose process of creating Blueprints into an elegant, readable, and maintainable experience. Instead of manually constructing complex object graphs, you can use a fluent interface that guides you through the process with IntelliSense support.

## Installation

```bash
dotnet add package Siccar.SDK.Fluent
```

## Quick Start

```csharp
using Siccar.SDK.Fluent.Builders;

var blueprint = BlueprintBuilder.Create()
    .WithTitle("Loan Application")
    .WithDescription("Simple loan approval workflow")

    // Add participants
    .AddParticipant("applicant", p => p
        .Named("Loan Applicant")
        .WithWallet("ws1abc123..."))

    .AddParticipant("approver", p => p
        .Named("Loan Approver")
        .WithWallet("ws1def456..."))

    // Add action
    .AddAction("submit-application", action => action
        .WithTitle("Submit Application")
        .SentBy("applicant")

        .RequiresData(schema => schema
            .AddNumber("loanAmount", f => f
                .WithTitle("Loan Amount")
                .WithMinimum(1000)
                .IsRequired())
            .AddString("purpose", f => f
                .WithTitle("Purpose")
                .IsRequired()))

        .Disclose("applicant", d => d.Fields("/loanAmount", "/purpose"))
        .Disclose("approver", d => d.AllFields())
        .TrackData("/loanAmount")

        .RouteToNext("approver"))

    .Build(); // Validates and builds the blueprint
```

## Key Features

### 1. Fluent Participant Definition

```csharp
.AddParticipant("doctor", p => p
    .Named("Dr. Smith")
    .FromOrganisation("City Hospital")
    .WithWallet("ws1doctor456")
    .UseStealthAddress())
```

### 2. Rich Data Schema Building

```csharp
.RequiresData(schema => schema
    .AddString("firstName", f => f
        .WithTitle("First Name")
        .WithMinLength(1)
        .WithMaxLength(50)
        .IsRequired())

    .AddNumber("salary", f => f
        .WithTitle("Annual Salary")
        .WithMinimum(0)
        .WithMaximum(1000000))

    .AddInteger("age", f => f
        .WithMinimum(18)
        .WithMaximum(120))

    .AddBoolean("employeed", f => f
        .WithTitle("Currently Employed"))

    .AddDate("birthDate", f => f
        .WithTitle("Date of Birth")
        .IsRequired())

    .AddFile("resume", f => f
        .WithTitle("Resume")
        .WithMaxSize(5 * 1024 * 1024)
        .WithAllowedExtensions(".pdf", ".docx")))
```

### 3. Selective Data Disclosure

```csharp
// Disclose specific fields
.Disclose("participant1", d => d
    .Fields("/fieldA", "/fieldB", "/nested/fieldC"))

// Disclose all fields
.Disclose("participant2", d => d
    .AllFields())

// Make data public (unencrypted)
.MakePublic("/publicField")

// Track data for indexing (stored in metadata)
.TrackData("/trackableField")
```

### 4. Conditional Routing

```csharp
.RouteConditionally(c => c
    .When(logic => logic.GreaterThan("loanAmount", 100000))
    .ThenRoute("seniorApprover")
    .ElseRoute("juniorApprover"))

// Complex conditions
.RouteConditionally(c => c
    .When(logic => logic.And(
        logic.GreaterThan("amount", 50000),
        logic.Equals("approved", true),
        logic.LessThan("riskScore", 500)))
    .ThenRoute("manager")
    .ElseRoute("applicant"))
```

### 5. Calculations

```csharp
.Calculate("debtToIncomeRatio", calc => calc
    .WithExpression(calc.Divide(
        calc.Variable("monthlyDebt"),
        calc.Variable("monthlyIncome"))))

.Calculate("totalPrice", calc => calc
    .WithExpression(calc.Add(
        calc.Variable("basePrice"),
        calc.Multiply(
            calc.Variable("basePrice"),
            calc.Constant(0.1))))) // Add 10% tax
```

### 6. Additional Recipients

```csharp
.RouteToNext("primaryRecipient")
.AlsoSendTo("observer1", "observer2") // CC functionality
```

## Examples

### Simple Approval Workflow

```csharp
var blueprint = BlueprintBuilder.Create()
    .WithTitle("Purchase Request")
    .WithDescription("Two-party approval process")

    .AddParticipant("requester", p => p
        .Named("Employee")
        .WithWallet("ws1requester"))

    .AddParticipant("manager", p => p
        .Named("Manager")
        .WithWallet("ws1manager"))

    .AddAction("submit", action => action
        .SentBy("requester")
        .RequiresData(schema => schema
            .AddString("item")
            .AddNumber("cost"))
        .Disclose("requester", d => d.AllFields())
        .Disclose("manager", d => d.AllFields())
        .RouteToNext("manager"))

    .AddAction("approve", action => action
        .SentBy("manager")
        .RequiresData(schema => schema
            .AddBoolean("approved"))
        .Disclose("requester", d => d.AllFields())
        .Disclose("manager", d => d.AllFields())
        .RouteToNext("requester"))

    .Build();
```

### Medical Referral with Privacy

```csharp
var blueprint = BlueprintBuilder.Create()
    .WithTitle("Medical Referral")

    .AddParticipant("patient", p => p
        .Named("John Doe")
        .UseStealthAddress())

    .AddParticipant("doctor", p => p
        .Named("Dr. Smith")
        .WithWallet("ws1doctor"))

    .AddParticipant("specialist", p => p
        .Named("Dr. Jones")
        .WithWallet("ws1specialist"))

    .AddAction("consultation", action => action
        .SentBy("doctor")
        .RequiresData(schema => schema
            .AddString("symptoms")
            .AddString("diagnosis")
            .AddInteger("severity"))

        // Patient only sees symptoms and diagnosis
        .Disclose("patient", d => d
            .Fields("/symptoms", "/diagnosis"))

        // Specialist sees everything
        .Disclose("specialist", d => d.AllFields())

        // Route based on severity
        .RouteConditionally(c => c
            .When(logic => logic.GreaterThan("severity", 3))
            .ThenRoute("specialist")
            .ElseRoute("patient")))

    .Build();
```

## Advanced Features

### Form UI Definition

```csharp
.WithForm(form => form
    .WithLayout(LayoutTypes.VerticalLayout)
    .WithTitle("Application Form")
    .AddControl(ctrl => ctrl
        .OfType(ControlTypes.TextLine)
        .WithTitle("Full Name")
        .BoundTo("fullName"))
    .AddControl(ctrl => ctrl
        .OfType(ControlTypes.Numeric)
        .WithTitle("Age")
        .BoundTo("age")))
```

### Validation

```csharp
// Build with full validation
var blueprint = builder.Build();

// Build without validation (draft)
var draft = builder.BuildDraft();

// Custom inline validation
builder
    .ValidateInline(bp => {
        bp.EnsureParticipantExists("requiredParticipant");
    })
    .Build();
```

## JSON Logic Reference

The SDK supports JSON Logic for conditions and calculations:

### Comparison Operators
- `GreaterThan(variable, value)`
- `GreaterThanOrEqual(variable, value)`
- `LessThan(variable, value)`
- `LessThanOrEqual(variable, value)`
- `Equals(variable, value)`
- `NotEquals(variable, value)`

### Logical Operators
- `And(condition1, condition2, ...)`
- `Or(condition1, condition2, ...)`
- `Not(condition)`

### Arithmetic Operators (Calculations)
- `Add(operand1, operand2, ...)`
- `Subtract(left, right)`
- `Multiply(operand1, operand2, ...)`
- `Divide(numerator, denominator)`
- `Modulo(left, right)`

## Best Practices

1. **Use meaningful IDs**: Participant and action IDs should be descriptive
2. **Minimize data disclosure**: Only disclose what each participant needs
3. **Use tracking data**: Track important fields for reporting and indexing
4. **Validate early**: Use `Build()` to catch errors during development
5. **Leverage IntelliSense**: Let your IDE guide you through available options

## Migration from Manual Construction

**Before (Manual):**
```csharp
var blueprint = new Blueprint
{
    Id = Guid.NewGuid().ToString(),
    Title = "My Blueprint",
    Participants = new List<Participant>
    {
        new Participant { Id = "p1", Name = "Person 1", WalletAddress = "ws1..." }
    },
    Actions = new List<Action>
    {
        new Action
        {
            Id = 1,
            Title = "Step 1",
            Sender = "p1",
            Disclosures = new List<Disclosure>
            {
                new Disclosure("p1", new List<string> { "/field1" })
            }
        }
    }
};
```

**After (Fluent):**
```csharp
var blueprint = BlueprintBuilder.Create()
    .WithTitle("My Blueprint")
    .AddParticipant("p1", p => p
        .Named("Person 1")
        .WithWallet("ws1..."))
    .AddAction("step1", action => action
        .WithTitle("Step 1")
        .SentBy("p1")
        .Disclose("p1", d => d.Field("/field1")))
    .Build();
```

## Contributing

See the main SICCARV3 repository for contribution guidelines.

## License

This software is licensed under the Siccar Proprietary Limited Use License.
See [LICENSE](../../../LICENCE.txt) for details.

## Support

For issues and questions, please use the [GitHub issue tracker](https://github.com/siccar/SICCARV3/issues).
