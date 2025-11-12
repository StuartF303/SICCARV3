# Task: Write Migration Guide

**ID:** TASK-019
**Status:** Not Started
**Priority:** Medium
**Estimate:** 6 hours
**Assignee:** Unassigned
**Created:** 2025-11-12

## Objective

Create comprehensive migration guide for transitioning from SiccarPlatformCryptography to Siccar.Cryptography v2.0.

## Migration Guide Structure

### MIGRATION.md Contents

1. **Overview**
   - Breaking changes summary
   - Benefits of migration
   - Timeline and support

2. **Namespace Changes**
   ```csharp
   // Old
   using SiccarPlatformCryptography;

   // New
   using Siccar.Cryptography;
   using Siccar.Cryptography.Core;
   using Siccar.Cryptography.Models;
   ```

3. **API Mapping**

   **Key Generation:**
   ```csharp
   // Old (synchronous)
   var (status, keySet) = CryptoModule.GenerateKeySet(WalletNetworks.ED25519, ref data);

   // New (async)
   var result = await cryptoModule.GenerateKeySetAsync(WalletNetworks.ED25519);
   if (result.IsSuccess) { var keySet = result.Value; }
   ```

   **Signing:**
   ```csharp
   // Old (synchronous)
   var (status, signature) = CryptoModule.Sign(hash, network, privKey);

   // New (async)
   var result = await cryptoModule.SignAsync(hash, network, privKey);
   if (result.IsSuccess) { var signature = result.Value; }
   ```

   **KeyManager:**
   ```csharp
   // Old
   var (status, keyRing) = KeyManager.CreateMasterKeyRing(network, password);

   // New
   var result = await keyManager.CreateMasterKeyRingAsync(network, password);
   ```

4. **Return Type Changes**
   - Old: `(Status, T?)` tuple
   - New: `CryptoResult<T>` with `IsSuccess`, `Value`, `Status`

5. **Transaction/Payload Migration**
   - Note: Transaction and Payload classes NOT included in v2.0
   - Will be separate package: `Siccar.Platform.Transactions`
   - Guidance on using core crypto for custom transaction implementations

6. **Dependency Updates**
   ```xml
   <!-- Remove old reference -->
   <PackageReference Include="SiccarPlatformCryptography" Version="1.x" />

   <!-- Add new reference -->
   <PackageReference Include="Siccar.Cryptography" Version="2.0.0" />
   ```

7. **Common Migration Patterns**
   - Converting sync to async code
   - Error handling changes
   - Disposal patterns for KeySet/KeyRing
   - Using CryptoResult pattern

8. **Testing After Migration**
   - Checklist of functionality to verify
   - Sample test cases

## Acceptance Criteria

- [ ] Complete migration guide written
- [ ] All API changes documented with examples
- [ ] Common migration patterns provided
- [ ] Breaking changes clearly highlighted
- [ ] Code examples for old and new APIs
- [ ] Troubleshooting section included

---

**Task Control**
- **Created By:** Claude Code
- **Dependencies:** TASK-001 through TASK-017
