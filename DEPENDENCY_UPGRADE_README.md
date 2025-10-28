# Dependency Upgrade - Getting Started

## Overview

This directory contains tools and documentation for upgrading SICCARV3 dependencies to their latest stable versions.

## What's Been Prepared

✅ **Comprehensive Analysis** - All dependencies catalogued with current and target versions
✅ **Upgrade Plan** - Four-phase approach with risk assessment
✅ **Automation Scripts** - PowerShell and Bash scripts for automated upgrades
✅ **Testing Strategy** - Clear testing requirements for each phase

## Files Created

```
c:\Projects\SICCARV3\
├── DEPENDENCY_UPGRADE_PLAN.md              # Detailed upgrade plan
├── DEPENDENCY_UPGRADE_README.md            # This file
├── NuGet.Config                            # Fixed XML format issue
└── scripts/
    ├── upgrade-dependencies-phase1.ps1    # PowerShell automation
    └── upgrade-dependencies-phase1.sh     # Bash automation
```

## Quick Start

### Prerequisites

1. **.NET 8.0 SDK** - Required for building .NET 8.0 projects
   ```bash
   # Check current version
   dotnet --version

   # If not 8.0.x, download from:
   # https://dotnet.microsoft.com/download/dotnet/8.0
   ```

2. **Git** - For version control and rollback capability
   ```bash
   # Create feature branch
   git checkout -b upgrade/dependencies-phase1
   ```

3. **Backup** - Ensure you have a clean working tree
   ```bash
   git status  # Should be clean
   git stash   # If you have uncommitted changes
   ```

### Option 1: Automated Upgrade (Recommended)

#### Using PowerShell (Windows)

```powershell
# Dry run first (see what would change)
.\scripts\upgrade-dependencies-phase1.ps1 -DryRun -Verbose

# Execute actual upgrades
.\scripts\upgrade-dependencies-phase1.ps1

# Test the build
dotnet restore
dotnet build
dotnet test
```

#### Using Bash (Linux/Mac/Git Bash)

```bash
# Dry run first
./scripts/upgrade-dependencies-phase1.sh --dry-run --verbose

# Execute actual upgrades
./scripts/upgrade-dependencies-phase1.sh

# Test the build
dotnet restore
dotnet build
dotnet test
```

### Option 2: Manual Upgrade

If you prefer manual control, see the detailed commands in [DEPENDENCY_UPGRADE_PLAN.md](DEPENDENCY_UPGRADE_PLAN.md) under "Commands to Execute".

## Upgrade Phases

### ✅ Phase 1: Low-Risk Updates (READY TO EXECUTE)
**Time:** 1-2 hours | **Risk:** Low | **Status:** Scripts Ready

Updates minor versions with no breaking changes:
- Microsoft.* packages (8.0.1 → 8.0.11)
- Dapr packages (1.12.0 → 1.14.0)
- Azure, Swagger, Health Checks, etc.

**Execute with:** The provided automation scripts

### ⏳ Phase 2: Medium-Risk Updates (AFTER PHASE 1)
**Time:** 2-4 hours | **Risk:** Medium | **Status:** Manual Required

Updates with major version changes but good compatibility:
- Finbuckle.MultiTenant (6.13.0 → 7.1.0)
- Serilog packages (major versions)

**Requires:** Review migration guides before executing

### ⚠️ Phase 3: High-Risk Updates (AFTER PHASE 2)
**Time:** 4-8 hours | **Risk:** High | **Status:** Manual Required

Updates with known breaking changes:
- JsonSchema.Net (5.5.1 → 7.2.2) **BREAKING**
- JsonSchema.Net.Generation (3.5.1 → 7.2.2) **BREAKING**

**Requires:** Code changes and extensive testing

### 🔴 Phase 4: IdentityServer4 Migration (SEPARATE PROJECT)
**Time:** 1-2 weeks | **Risk:** Critical | **Status:** Planning Required

IdentityServer4 is **END OF LIFE** and must be migrated.

**Options:**
1. **Duende IdentityServer** (Recommended but requires license)
2. **OpenIddict** (Free, open source)
3. **Azure AD B2C** (If moving fully to cloud)

**See:** Separate migration document (to be created)

## Testing Checklist

After each phase, verify:

### Build Tests
- [ ] `dotnet restore` completes without errors
- [ ] `dotnet build` completes without errors
- [ ] No new warnings introduced

### Unit Tests
- [ ] All unit tests pass
- [ ] No tests skipped
- [ ] Coverage remains acceptable

### Integration Tests
- [ ] Service-to-service communication works
- [ ] Database connections work
- [ ] API endpoints respond correctly

### Functional Tests
- [ ] Blueprint creation works
- [ ] Action processing works
- [ ] Wallet operations work
- [ ] Multi-tenancy works
- [ ] Authentication/Authorization works (critical for Phase 4)

## Common Issues & Solutions

### Issue: NuGet package not found

**Cause:** Azure DevOps feed authentication
**Solution:**
```bash
# Configure Azure DevOps credentials
dotnet nuget add source https://projectbob.pkgs.visualstudio.com/SICCARV3/_packaging/siccarv3feed/nuget/v3/index.json \
  --name siccarv3 --username your-email --password YOUR-PAT
```

### Issue: Build fails with SDK version error

**Cause:** .NET SDK version mismatch
**Solution:**
```bash
# Check SDK version
dotnet --list-sdks

# Install .NET 8.0 SDK from https://dotnet.microsoft.com/download
```

### Issue: Dependency conflicts

**Cause:** Transitive dependency version conflicts
**Solution:**
```bash
# Clear NuGet caches
dotnet nuget locals all --clear

# Restore packages
dotnet restore --force
```

### Issue: Tests fail after update

**Cause:** Breaking API changes
**Solution:**
1. Check package release notes for breaking changes
2. Review DEPENDENCY_UPGRADE_PLAN.md for known issues
3. Consider rolling back specific package if blocking

## Rollback Procedure

If issues arise during any phase:

```bash
# Option 1: Rollback all changes
git reset --hard HEAD
git clean -fd

# Option 2: Rollback specific file
git checkout HEAD -- path/to/file.csproj

# Option 3: Rollback specific package
cd path/to/project
dotnet add package PackageName --version OldVersion
```

## Progress Tracking

Create issues/tasks for tracking:

- [ ] Phase 1: Low-Risk Updates
  - [ ] Execute upgrade script
  - [ ] Verify builds
  - [ ] Run unit tests
  - [ ] Run integration tests
  - [ ] Commit changes

- [ ] Phase 2: Medium-Risk Updates
  - [ ] Review Finbuckle migration guide
  - [ ] Update Finbuckle packages
  - [ ] Review Serilog changes
  - [ ] Update Serilog packages
  - [ ] Test logging output
  - [ ] Test multi-tenancy
  - [ ] Commit changes

- [ ] Phase 3: High-Risk Updates
  - [ ] Review JsonSchema.Net v7 migration guide
  - [ ] Identify code impact areas
  - [ ] Create feature branch
  - [ ] Update packages
  - [ ] Fix breaking changes
  - [ ] Test blueprint creation
  - [ ] Test action processing
  - [ ] Full regression test
  - [ ] Commit changes

- [ ] Phase 4: IdentityServer Migration
  - [ ] Create separate planning document
  - [ ] Choose replacement (Duende vs OpenIddict)
  - [ ] Plan migration strategy
  - [ ] Schedule migration window
  - [ ] Execute migration
  - [ ] Extensive testing
  - [ ] Deploy to staging
  - [ ] Production deployment

## Version Control Strategy

```bash
# Phase 1
git checkout -b upgrade/phase1-low-risk
# ... make changes ...
git add .
git commit -m "chore: upgrade Phase 1 dependencies (low-risk)"
git push origin upgrade/phase1-low-risk
# ... create PR, review, merge ...

# Phase 2
git checkout main
git pull
git checkout -b upgrade/phase2-medium-risk
# ... make changes ...
git commit -m "chore: upgrade Phase 2 dependencies (medium-risk)"

# Phase 3
git checkout main
git pull
git checkout -b upgrade/phase3-high-risk
# ... make changes ...
git commit -m "chore: upgrade Phase 3 dependencies - JsonSchema.Net v7 (breaking changes)"

# Phase 4
git checkout -b feature/identityserver-migration
# ... separate migration process ...
```

## Getting Help

- **Upgrade Plan Details:** See [DEPENDENCY_UPGRADE_PLAN.md](DEPENDENCY_UPGRADE_PLAN.md)
- **Package Release Notes:** Check NuGet.org for each package
- **Breaking Changes:** See specific package documentation
- **Issues:** Create GitHub issue with "dependency-upgrade" label

## Next Steps

1. **Review** the [DEPENDENCY_UPGRADE_PLAN.md](DEPENDENCY_UPGRADE_PLAN.md)
2. **Install** .NET 8.0 SDK (if not already installed)
3. **Create** git branch for Phase 1
4. **Run** Phase 1 automation script (dry-run first!)
5. **Test** thoroughly
6. **Commit** and create PR
7. **Proceed** to Phase 2 after Phase 1 is merged

## Notes

- Phase 1 is **safe to execute immediately**
- Phases 2-3 require more careful review
- Phase 4 (IdentityServer) is a **separate project** requiring significant planning
- Keep `main` branch stable - use feature branches
- Test thoroughly between phases
- Document any issues encountered

---

**Created:** 2025-01-XX
**Status:** Ready for Phase 1 Execution
**Owner:** Development Team
