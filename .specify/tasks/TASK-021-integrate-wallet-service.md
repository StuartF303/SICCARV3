# Task: Update WalletService to Use New Library

**ID:** TASK-021
**Status:** Not Started
**Priority:** High
**Estimate:** 12 hours
**Assignee:** Unassigned
**Created:** 2025-11-12

## Objective

Update WalletService to use Siccar.Cryptography v2.0, replacing all references to SiccarPlatformCryptography.

## Migration Steps

### 1. Update Dependencies
```xml
<!-- Remove -->
<ProjectReference Include="..\Common\SiccarPlatformCryptography\..." />

<!-- Add -->
<PackageReference Include="Siccar.Cryptography" Version="2.0.0" />
```

### 2. Update Namespaces
```csharp
// Old
using SiccarPlatformCryptography;

// New
using Siccar.Cryptography;
using Siccar.Cryptography.Core;
using Siccar.Cryptography.Interfaces;
using Siccar.Cryptography.Models;
```

### 3. Update Key Generation
- Replace synchronous `GenerateKeySet` calls with async `GenerateKeySetAsync`
- Update return type handling from tuples to `CryptoResult<T>`
- Update error handling

### 4. Update Transaction Signing
- Transaction classes will need custom implementation
- Use core crypto for signing operations
- Implement transaction formatting separately

### 5. Update Wallet Management
- Use new `KeyManager` for mnemonic operations
- Use new `KeyChain` for multi-wallet management
- Update wallet address format to Bech32 (ws1 prefix)

### 6. Dependency Injection Updates
```csharp
// Register new services
services.AddSingleton<ICryptoModule, CryptoModule>();
services.AddSingleton<IKeyManager, KeyManager>();
services.AddSingleton<IHashProvider, HashProvider>();
services.AddTransient<IWalletUtilities, WalletUtilities>();
```

## Testing Requirements

- [ ] All existing wallet tests updated
- [ ] Backward compatibility for existing wallets verified
- [ ] Key recovery from old mnemonics working
- [ ] All API endpoints functional
- [ ] Performance benchmarks comparable

## Acceptance Criteria

- [ ] WalletService compiles with new library
- [ ] All wallet operations functional
- [ ] All tests passing
- [ ] No references to old SiccarPlatformCryptography
- [ ] API contracts unchanged (backward compatible)

---

**Task Control**
- **Created By:** Claude Code
- **Dependencies:** TASK-001 through TASK-020
