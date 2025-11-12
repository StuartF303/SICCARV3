# Task: Update TenantService to Use New Library

**ID:** TASK-022
**Status:** Not Started
**Priority:** High
**Estimate:** 8 hours
**Assignee:** Unassigned
**Created:** 2025-11-12

## Objective

Update TenantService to use Siccar.Cryptography v2.0 for tenant key management and cryptographic operations.

## Migration Steps

### 1. Update Dependencies
- Remove SiccarPlatformCryptography reference
- Add Siccar.Cryptography v2.0 package

### 2. Update Tenant Key Management
- Update tenant key generation to async
- Use new KeyManager for tenant key pairs
- Update tenant wallet address format (Bech32)

### 3. Update Tenant Authentication
- Update JWT signing with new crypto module
- Update signature verification
- Ensure backward compatibility for existing tenants

### 4. Update Data Encryption
- Use new SymmetricCrypto for tenant data encryption
- Migrate encryption keys if format changed
- Update decryption for backward compatibility

## Testing Requirements

- [ ] Tenant creation working
- [ ] Tenant authentication working
- [ ] Existing tenant compatibility verified
- [ ] Data encryption/decryption working
- [ ] All API tests passing

## Acceptance Criteria

- [ ] TenantService compiles with new library
- [ ] All tenant operations functional
- [ ] Backward compatibility maintained
- [ ] All tests passing

---

**Task Control**
- **Created By:** Claude Code
- **Dependencies:** TASK-001 through TASK-020, TASK-021
