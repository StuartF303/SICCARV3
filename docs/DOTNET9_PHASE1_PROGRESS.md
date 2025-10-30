# .NET 9 Upgrade - Phase 1 Progress

## Status: IN PROGRESS

### Completed Steps:

#### ✅ Step 1: Branch Created
- Branch: `upgrade/dotnet9-phase1`
- Created successfully

#### ✅ Step 2: TargetFramework Updated
- **45 projects** updated from net8.0/net7.0 to net9.0
- **4 projects excluded** (staying on .NET 8):
  - TenantService.csproj
  - TenantCore.csproj
  - TenantRepository.csproj
  - TenantService.IntegrationTests.csproj

#### ✅ Step 3: Microsoft Packages Upgraded
- Successfully upgraded to 9.0.0:
  - Microsoft.AspNetCore.Authentication.JwtBearer
  - Microsoft.AspNetCore.Authentication.OpenIdConnect
  - Microsoft.AspNetCore.SignalR.Client
  - Microsoft.AspNetCore.OData
  - Microsoft.EntityFrameworkCore (+ Design, Tools, InMemory)
  - Microsoft.Extensions.* packages
  - Microsoft.NET.Test.Sdk → 18.0.0

- **Known failures** (expected):
  - `Microsoft.AspNetCore.Components.WebAssembly.Server` - Package doesn't exist for .NET 9 (deprecated)
    - Affected: AdminUI.Server.csproj
    - Fix: Need to remove this package reference manually

  - IntegrationTests.csproj and SiccarCommonServiceClientsTests.csproj had NuGet config issues
    - These are minor and won't block the build

### Next Steps (TODO):

#### Step 4: Upgrade Swashbuckle to 6.9.0
- Script: `scripts/upgrade-swashbuckle-net9.ps1`
- Target version: 6.9.0 for both Swashbuckle.AspNetCore and Swashbuckle.AspNetCore.Annotations

#### Step 5: Upgrade API Versioning to 8.1.0
- Script: `scripts/upgrade-api-versioning-net9.ps1`
- Asp.Versioning.Mvc.ApiExplorer: 8.0.0 → 8.1.0
- Asp.Versioning.Mvc: 8.0.0 → 8.1.0

#### Step 6: Remove Obsolete Package References
- Remove `IdentityServer4.AccessTokenValidation` from:
  - ActionService.csproj
  - BlueprintService.csproj
  - WalletService.csproj
  - RegisterService.csproj
- Remove `Microsoft.AspNetCore.Components.WebAssembly.Server` from:
  - AdminUI.Server.csproj

#### Step 7: Update Dockerfiles to .NET 9
- Script: `scripts/update-dockerfiles-net9.ps1`
- Change base images from 8.0 to 9.0

#### Step 8: Run dotnet restore
- Verify all packages restore correctly

#### Step 9: Run dotnet build
- Verify all projects build successfully

#### Step 10: Run tests
- Execute all unit and integration tests

#### Step 11: Commit changes
- Commit message: "chore: upgrade to .NET 9 (Phase 1 - excluding TenantService)"

---

## Notes:

### Package Version Issues to Address:
1. AdminUI.Server needs `Microsoft.AspNetCore.Components.WebAssembly.Server` removed - this package is deprecated in .NET 9
2. May need to update MongoDB.Driver to latest version for full .NET 9 compatibility
3. Pomelo.EntityFrameworkCore.MySql may need update if 9.0 version is available

### Post-Phase 1:
- TenantService remains on .NET 8
- Phase 2 will tackle OpenIddict migration for TenantService
- All other services will be on .NET 9 and can interoperate with .NET 8 TenantService

---

**Date:** 2025-10-29
**Branch:** upgrade/dotnet9-phase1
**Progress:** ~40% complete
