# SICCARV3 Dependency Upgrade Plan

**Date:** 2025-01-XX
**Status:** In Progress

## Overview

This document tracks the dependency upgrade process for the SICCARV3 solution to ensure all packages are up-to-date with the latest stable versions.

## Critical Issues Identified

### 🔴 **CRITICAL: IdentityServer4 End of Life**
- **Current:** IdentityServer4 4.1.2
- **Status:** EOL - No longer supported
- **Action Required:** Migrate to Duende IdentityServer or OpenIddict
- **Priority:** HIGH
- **Tracking:** Separate migration task required

## Package Categories & Upgrade Status

### 1. Core Framework Packages

| Package | Current | Latest | Status | Notes |
|---------|---------|--------|--------|-------|
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.1 | 8.0.11 | ⚠️ Update | Security updates |
| Microsoft.AspNetCore.Authentication.OpenIdConnect | 8.0.1 | 8.0.11 | ⚠️ Update | Security updates |
| Microsoft.EntityFrameworkCore | 8.0.1 | 8.0.11 | ⚠️ Update | Bug fixes |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.1 | 8.0.11 | ⚠️ Update | Bug fixes |
| Microsoft.AspNetCore.OData | 8.2.4 | 8.2.5 | ⚠️ Update | Minor update |

### 2. Dapr Packages

| Package | Current | Latest | Status | Notes |
|---------|---------|--------|--------|-------|
| Dapr.AspNetCore | 1.12.0 | 1.14.0 | ⚠️ Update | New features |
| Dapr.Extensions.Configuration | 1.12.0 | 1.14.0 | ⚠️ Update | New features |

### 3. Validation & Schema Packages

| Package | Current | Latest | Status | Notes |
|---------|---------|--------|--------|-------|
| FluentValidation | 11.9.0 | 11.10.0 | ⚠️ Update | Minor update |
| FluentValidation.AspNetCore | 11.3.0 | 11.3.0 | ✅ Current | - |
| JsonSchema.Net | 5.5.1 | 7.2.2 | ⚠️ Update | Major version jump - breaking changes |
| JsonSchema.Net.Generation | 3.5.1 | 7.2.2 | ⚠️ Update | Major version jump - breaking changes |
| JsonLogic | 4.0.4 | 4.1.0 | ⚠️ Update | Minor update |

### 4. Azure & Identity Packages

| Package | Current | Latest | Status | Notes |
|---------|---------|--------|--------|-------|
| Azure.Identity | 1.10.4 | 1.13.1 | ⚠️ Update | Security & performance |
| Azure.Extensions.AspNetCore.DataProtection.Blobs | 1.3.2 | 1.3.4 | ⚠️ Update | Minor update |
| Azure.Extensions.AspNetCore.DataProtection.Keys | 1.2.2 | 1.2.4 | ⚠️ Update | Minor update |
| IdentityModel.AspNetCore | 4.3.0 | 4.4.0 | ⚠️ Update | Minor update |
| IdentityServer4 | 4.1.2 | ❌ EOL | 🔴 CRITICAL | Requires migration |
| IdentityServer4.AccessTokenValidation | 3.0.1 | ❌ EOL | 🔴 CRITICAL | Requires migration |
| IdentityServer4.AspNetIdentity | 4.1.2 | ❌ EOL | 🔴 CRITICAL | Requires migration |
| IdentityServer4.EntityFramework | 4.1.2 | ❌ EOL | 🔴 CRITICAL | Requires migration |

### 5. Logging & Monitoring Packages

| Package | Current | Latest | Status | Notes |
|---------|---------|--------|--------|-------|
| Serilog.AspNetCore | 8.0.1 | 8.0.3 | ⚠️ Update | Bug fixes |
| Serilog.Expressions | 4.0.0 | 5.0.0 | ⚠️ Update | Major version |
| Serilog.Sinks.ApplicationInsights | 4.0.0 | 5.0.0 | ⚠️ Update | Major version |
| Serilog.Sinks.Seq | 6.0.0 | 8.0.0 | ⚠️ Update | Major version |
| Microsoft.ApplicationInsights.AspNetCore | 2.22.0 | 2.22.0 | ✅ Current | - |
| Microsoft.ApplicationInsights.Kubernetes | 6.1.1 | 7.0.0 | ⚠️ Update | Major version |
| Microsoft.ApplicationInsights.Profiler.AspNetCore | 2.5.3 | 2.7.0 | ⚠️ Update | Minor update |

### 6. Multi-Tenancy & Health Checks

| Package | Current | Latest | Status | Notes |
|---------|---------|--------|--------|-------|
| Finbuckle.MultiTenant | 6.13.0 | 7.1.0 | ⚠️ Update | Major version |
| Finbuckle.MultiTenant.AspNetCore | 6.13.0 | 7.1.0 | ⚠️ Update | Major version |
| AspNetCore.HealthChecks.AzureKeyVault | 8.0.0 | 8.0.1 | ⚠️ Update | Minor update |
| AspNetCore.HealthChecks.MongoDb | 8.0.0 | 8.1.0 | ⚠️ Update | Minor update |
| AspNetCore.HealthChecks.MySql | 8.0.0 | 8.0.1 | ⚠️ Update | Minor update |

### 7. API & Documentation Packages

| Package | Current | Latest | Status | Notes |
|---------|---------|--------|--------|-------|
| Swashbuckle.AspNetCore | 6.5.0 | 6.9.0 | ⚠️ Update | Multiple updates |
| Swashbuckle.AspNetCore.Annotations | 6.5.0 | 6.9.0 | ⚠️ Update | Multiple updates |
| Asp.Versioning.Mvc | 8.0.0 | 8.1.0 | ⚠️ Update | Minor update |
| Asp.Versioning.Mvc.ApiExplorer | 8.0.0 | 8.1.0 | ⚠️ Update | Minor update |

### 8. Other Dependencies

| Package | Current | Latest | Status | Notes |
|---------|---------|--------|--------|-------|
| Microsoft.Extensions.Http.Polly | 8.0.1 | 8.0.11 | ⚠️ Update | Bug fixes |
| Microsoft.SourceLink.AzureRepos.Git | 8.0.0 | 8.0.0 | ✅ Current | - |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets | 1.19.6 | 1.21.0 | ⚠️ Update | Minor update |

## Upgrade Strategy

### Phase 1: Low-Risk Updates (Safe)
**Estimated Time:** 1-2 hours
**Risk Level:** Low

Update packages with minor version changes and no breaking changes:
- Microsoft.* packages (8.0.1 → 8.0.11)
- Dapr packages (1.12.0 → 1.14.0)
- FluentValidation (11.9.0 → 11.10.0)
- Azure.Identity (1.10.4 → 1.13.1)
- Swashbuckle (6.5.0 → 6.9.0)
- Health check packages

### Phase 2: Medium-Risk Updates (Requires Testing)
**Estimated Time:** 2-4 hours
**Risk Level:** Medium

Update packages with major version changes but good backwards compatibility:
- Finbuckle.MultiTenant (6.13.0 → 7.1.0)
- Serilog packages (major version updates)
- JsonLogic (4.0.4 → 4.1.0)

### Phase 3: High-Risk Updates (Breaking Changes)
**Estimated Time:** 4-8 hours
**Risk Level:** High

Update packages with known breaking changes:
- JsonSchema.Net (5.5.1 → 7.2.2) - **BREAKING CHANGES**
- JsonSchema.Net.Generation (3.5.1 → 7.2.2) - **BREAKING CHANGES**
- Microsoft.ApplicationInsights.Kubernetes (6.1.1 → 7.0.0)

### Phase 4: Critical Migration (Separate Project)
**Estimated Time:** 1-2 weeks
**Risk Level:** Critical

IdentityServer4 Migration:
- Research: Duende IdentityServer vs OpenIddict
- Plan migration strategy
- Implement migration
- Test thoroughly
- Deploy

## Breaking Changes to Watch

### JsonSchema.Net 5.x → 7.x
- **Impact:** High - Used extensively in SiccarApplication
- **Breaking Changes:**
  - API changes in schema generation
  - Validation API updates
  - Namespace changes
- **Migration Guide:** https://docs.json-everything.net/schema/release-notes/

### Finbuckle.MultiTenant 6.x → 7.x
- **Impact:** Medium - Used in TenantService
- **Breaking Changes:**
  - Configuration API changes
  - Strategy registration updates
- **Migration Guide:** https://www.finbuckle.com/MultiTenant/Docs/v7.0.0/Introduction

### Serilog 4.x → 5.x/8.x
- **Impact:** Low-Medium - Configuration changes
- **Breaking Changes:**
  - Sink configuration updates
  - Expression syntax changes

## Testing Requirements

### After Phase 1 (Low-Risk)
- ✅ Solution builds without errors
- ✅ Unit tests pass
- ✅ Integration tests pass

### After Phase 2 (Medium-Risk)
- ✅ All Phase 1 checks
- ✅ Multi-tenancy functionality works
- ✅ Logging outputs correctly
- ✅ End-to-end tests pass

### After Phase 3 (High-Risk)
- ✅ All Phase 2 checks
- ✅ Blueprint validation works correctly
- ✅ JSON Schema generation works
- ✅ Blueprint creation and publishing works
- ✅ Action processing works
- ✅ Full regression testing

### After Phase 4 (IdentityServer Migration)
- ✅ Authentication works
- ✅ Authorization works
- ✅ Token generation and validation
- ✅ Multi-tenant authentication
- ✅ All user flows work
- ✅ Security audit

## Rollback Plan

For each phase:
1. Create git branch: `upgrade/phase-X-dependencies`
2. Commit before starting
3. If issues arise: `git reset --hard HEAD`
4. Document issues encountered
5. Reassess upgrade approach

## Commands to Execute

### Phase 1: Low-Risk Updates

```bash
# Microsoft packages
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.11
dotnet add package Microsoft.Extensions.Http.Polly --version 8.0.11
dotnet add package Microsoft.AspNetCore.OData --version 8.2.5

# Dapr packages
dotnet add package Dapr.AspNetCore --version 1.14.0
dotnet add package Dapr.Extensions.Configuration --version 1.14.0

# Validation
dotnet add package FluentValidation --version 11.10.0

# Azure
dotnet add package Azure.Identity --version 1.13.1
dotnet add package Azure.Extensions.AspNetCore.DataProtection.Blobs --version 1.3.4
dotnet add package Azure.Extensions.AspNetCore.DataProtection.Keys --version 1.2.4

# API/Docs
dotnet add package Swashbuckle.AspNetCore --version 6.9.0
dotnet add package Swashbuckle.AspNetCore.Annotations --version 6.9.0
dotnet add package Asp.Versioning.Mvc --version 8.1.0
dotnet add package Asp.Versioning.Mvc.ApiExplorer --version 8.1.0

# Health Checks
dotnet add package AspNetCore.HealthChecks.AzureKeyVault --version 8.0.1
dotnet add package AspNetCore.HealthChecks.MongoDb --version 8.1.0
dotnet add package AspNetCore.HealthChecks.MySql --version 8.0.1

# Other
dotnet add package Microsoft.VisualStudio.Azure.Containers.Tools.Targets --version 1.21.0
dotnet add package JsonLogic --version 4.1.0
dotnet add package IdentityModel.AspNetCore --version 4.4.0
```

## Progress Tracking

- [ ] Phase 1: Low-Risk Updates
- [ ] Phase 2: Medium-Risk Updates
- [ ] Phase 3: High-Risk Updates
- [ ] Phase 4: IdentityServer Migration (Separate)

## Notes

- Current .NET SDK version on system: 6.0.428 (needs upgrade to 8.0.x for proper builds)
- NuGet.Config was fixed (XML declaration issue resolved)
- Siccar.SDK.Fluent already uses latest versions

## Next Actions

1. Install .NET 8.0 SDK
2. Execute Phase 1 updates
3. Run tests
4. Document any issues
5. Proceed to Phase 2

---

**Last Updated:** 2025-01-XX
**Updated By:** Claude (AI Assistant)
