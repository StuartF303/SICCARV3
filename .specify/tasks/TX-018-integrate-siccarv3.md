# Task: Integrate with SICCARV3 Services

**ID:** TX-018
**Status:** Not Started
**Priority:** High
**Estimate:** 16 hours
**Created:** 2025-11-12

## Objective

Update all SICCARV3 services to use Siccar.TransactionHandler v2.0, replacing embedded transaction classes.

## Services to Update

### 1. WalletService
**Changes:**
- Replace embedded TransactionBuilder with new library
- Update transaction creation endpoints
- Update transaction signing logic
- Update payload management
- Migrate to async/await pattern

**Estimated:** 6 hours

### 2. RegisterService
**Changes:**
- Replace transaction verification logic
- Update transaction storage/retrieval
- Update transaction formatting
- Support reading old v1-v4 transactions
- Performance testing

**Estimated:** 6 hours

### 3. TenantService
**Changes:**
- Update transaction-related operations
- Update tenant transaction history
- Ensure backward compatibility

**Estimated:** 4 hours

## Migration Steps

### Phase 1: Add Dependencies
```xml
<PackageReference Include="Siccar.Cryptography" Version="2.0.0" />
<PackageReference Include="Siccar.TransactionHandler" Version="2.0.0" />
```

### Phase 2: Update Dependency Injection
```csharp
services.AddSingleton<ICryptoModule, CryptoModule>();
services.AddSingleton<IKeyManager, KeyManager>();
services.AddTransient<ITransactionBuilder, TransactionBuilder>();
services.AddTransient<ITransactionSerializer, TransactionSerializer>();
```

### Phase 3: Update Transaction Creation
```csharp
// Old
var tx = TransactionBuilder.Build(TransactionVersion.TX_VERSION_4);
tx.SetTxRecipients(recipients);
tx.SignTx(wifKey);

// New
var result = await transactionBuilder
    .Create(TransactionVersion.V4)
    .WithRecipients(recipients)
    .SignAsync(wifKey)
    .Build();
```

### Phase 4: Update API Controllers
- Update request/response models
- Handle TransactionResult types
- Add async/await throughout
- Update error handling

## Testing Requirements

- [ ] All service APIs functional
- [ ] Transaction creation working
- [ ] Transaction verification working
- [ ] Backward compatibility verified
- [ ] Performance comparable or better
- [ ] Integration tests passing

## Acceptance Criteria

- [ ] All services updated
- [ ] All APIs functional
- [ ] No references to old embedded classes
- [ ] All tests passing
- [ ] Performance validated

---

**Dependencies:** TX-001 through TX-017, Siccar.Cryptography v2.0
