# Continue .NET 9 Upgrade - Phase 1

## Current Status:
- Branch: `upgrade/dotnet9-phase1`
- Progress: ~45% complete
- 45 projects updated to net9.0
- Most Microsoft packages upgraded to 9.0.0

## Steps to Complete:

### 1. Upgrade Swashbuckle
```powershell
powershell -ExecutionPolicy Bypass -File "c:\Projects\SICCARV3\scripts\upgrade-swashbuckle-net9.ps1"
```

### 2. Upgrade API Versioning
```powershell
powershell -ExecutionPolicy Bypass -File "c:\Projects\SICCARV3\scripts\upgrade-api-versioning-net9.ps1"
```

### 3. Remove Obsolete Package References
Manually remove from .csproj files:
- ActionService.csproj: Remove `IdentityServer4.AccessTokenValidation`
- BlueprintService.csproj: Remove `IdentityServer4.AccessTokenValidation`
- WalletService.csproj: Remove `IdentityServer4.AccessTokenValidation`
- RegisterService.csproj: Remove `IdentityServer4.AccessTokenValidation`
- AdminUI.Server.csproj: Remove `Microsoft.AspNetCore.Components.WebAssembly.Server`

### 4. Update Dockerfiles
```powershell
powershell -ExecutionPolicy Bypass -File "c:\Projects\SICCARV3\scripts\update-dockerfiles-net9.ps1"
```

### 5. Build and Test
```powershell
# Restore packages
dotnet restore

# Build all projects
dotnet build --no-restore

# Run tests
dotnet test --no-build
```

### 6. Commit Changes
```powershell
git add .
git commit -m "chore: upgrade to .NET 9 (Phase 1 - excluding TenantService)

Upgraded 45 projects to .NET 9, excluding TenantService and related projects which remain on .NET 8.

Changes:
- Updated TargetFramework to net9.0
- Upgraded Microsoft.AspNetCore.* packages to 9.0.0
- Upgraded Microsoft.EntityFrameworkCore.* to 9.0.0
- Upgraded Swashbuckle to 6.9.0
- Upgraded API Versioning to 8.1.0
- Updated Dockerfiles to use .NET 9 images
- Removed obsolete package references

TenantService migration (OpenIddict) deferred to Phase 2.

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>"
```

### 7. Merge to Main (Optional)
```powershell
git checkout main
git merge upgrade/dotnet9-phase1 --no-edit
git push origin main
```

## Known Issues:
1. `Microsoft.AspNetCore.Components.WebAssembly.Server` doesn't exist for .NET 9 - needs manual removal from AdminUI.Server.csproj
2. Some test projects may have NuGet config warnings - these won't block the build

## Next: Phase 2
After Phase 1 is complete and tested:
- Migrate TenantService to OpenIddict
- Update TenantService to .NET 9
- Full end-to-end testing
