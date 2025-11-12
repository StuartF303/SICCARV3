# Task: Deprecate SiccarPlatformCryptography

**ID:** TASK-024
**Status:** Not Started
**Priority:** High
**Estimate:** 4 hours
**Assignee:** Unassigned
**Created:** 2025-11-12

## Objective

Mark old SiccarPlatformCryptography library as deprecated and ensure no new code references it.

## Deprecation Steps

### 1. Add Obsolete Attributes
```csharp
[Obsolete("This class is deprecated. Use Siccar.Cryptography.Core.CryptoModule instead.", false)]
public sealed class CryptoModule : CryptoModuleBase
{
    // ...existing code...
}
```

### 2. Update Documentation
- Add deprecation notice to README
- Update all documentation references
- Create migration timeline

### 3. Code Analysis Rules
- Create .editorconfig rule to flag old library usage
- Add analyzer to detect deprecated API usage
- Configure CI/CD to fail on new deprecation warnings

### 4. Dependency Audit
- Search entire solution for SiccarPlatformCryptography references
- Create report of remaining usages
- Plan for complete removal

### 5. Communication
- Announce deprecation to development team
- Document deprecation in release notes
- Set sunset date (e.g., 6 months)

## Acceptance Criteria

- [ ] All old library classes marked [Obsolete]
- [ ] Deprecation notices in documentation
- [ ] Code analysis rules configured
- [ ] No new code references old library
- [ ] Deprecation timeline communicated
- [ ] Migration guide complete

---

**Task Control**
- **Created By:** Claude Code
- **Dependencies:** TASK-021, TASK-022, TASK-023
