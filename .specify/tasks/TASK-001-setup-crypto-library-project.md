# Task: Setup New Siccar.Cryptography Library Project

**ID:** TASK-001
**Status:** Not Started
**Priority:** Critical
**Estimate:** 4 hours
**Assignee:** Unassigned
**Created:** 2025-11-12
**Updated:** 2025-11-12

## Context

This is the foundational task for the Siccar.Cryptography library rewrite. We need to create a clean, new library project with minimal dependencies and proper project structure.

**Related Specifications:**
- [Siccar.Cryptography Rewrite Spec](../specs/siccar-cryptography-rewrite.md)
- [Project Plan](../plan.md#project-structure)

**Dependencies:**
- None (first task)

## Objective

Create the new `Siccar.Cryptography` library project with proper configuration, minimal dependencies, and organized folder structure ready for implementation.

## Implementation Details

### Changes Required

1. **Create New Project**
   - Location: `src/Common/Siccar.Cryptography/`
   - Project type: Class Library (.NET 9.0)
   - Also target .NET Standard 2.1 for broader compatibility

2. **Project Configuration**
   - Remove all unnecessary dependencies
   - Add only Sodium.Core package
   - Configure NuGet package metadata
   - Enable nullable reference types
   - Configure XML documentation generation

3. **Folder Structure**
   - Create directories: `Enums/`, `Interfaces/`, `Core/`, `Models/`, `Utilities/`, `Extensions/`
   - Add placeholder files to maintain structure

4. **Build Configuration**
   - Enable deterministic builds
   - Configure strong-name signing (if required)
   - Set up source link
   - Configure package generation

### Technical Approach

**Project File (.csproj):**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Multi-targeting -->
    <TargetFrameworks>net9.0;netstandard2.1</TargetFrameworks>

    <!-- Language and nullable -->
    <LangVersion>12</LangVersion>
    <Nullable>enable</Nullable>

    <!-- Package info -->
    <PackageId>Siccar.Cryptography</PackageId>
    <Version>2.0.0</Version>
    <Authors>Siccar Development Team</Authors>
    <Company>Wallet.Services (Scotland) Ltd</Company>
    <Product>Siccar.Cryptography</Product>
    <Description>Standalone cryptography library for the Siccar platform</Description>
    <PackageTags>cryptography;siccar;ed25519;ecdsa;rsa;encryption;signing</PackageTags>
    <RepositoryUrl>https://github.com/StuartF303/SICCARV3</RepositoryUrl>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>

    <!-- Documentation -->
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn><!-- Suppress missing XML doc warnings initially -->

    <!-- Build -->
    <Deterministic>true</Deterministic>
    <ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>

    <!-- Source link -->
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  </PropertyGroup>

  <ItemGroup>
    <!-- Minimal dependencies -->
    <PackageReference Include="Sodium.Core" Version="1.3.4" />
    <PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" PrivateAssets="All" />
  </ItemGroup>
</Project>
```

**Directory Structure to Create:**
```
src/Common/Siccar.Cryptography/
├── Siccar.Cryptography.csproj
├── Enums/
│   └── .gitkeep
├── Interfaces/
│   └── .gitkeep
├── Core/
│   └── .gitkeep
├── Models/
│   └── .gitkeep
├── Utilities/
│   └── .gitkeep
└── Extensions/
    └── .gitkeep
```

### Constitutional Compliance

- ✅ Follows .NET 9 framework standard from constitution
- ✅ Minimal dependencies principle
- ✅ Proper licensing (MIT/Siccar Proprietary)
- ✅ XML documentation enabled
- ✅ Follows project organization standards

## Testing Requirements

### Build Tests
- [ ] Project builds successfully on .NET 9.0
- [ ] Project builds successfully on .NET Standard 2.1
- [ ] No build warnings
- [ ] NuGet package generates correctly

### Configuration Tests
- [ ] XML documentation file is generated
- [ ] Source link information is embedded
- [ ] Package metadata is correct
- [ ] Multi-targeting works correctly

## Acceptance Criteria

- [ ] New `Siccar.Cryptography` project created in correct location
- [ ] Project targets both .NET 9.0 and .NET Standard 2.1
- [ ] Only Sodium.Core dependency present (plus SourceLink for dev)
- [ ] All required folders created with .gitkeep files
- [ ] Project builds without errors or warnings
- [ ] XML documentation generation enabled
- [ ] NuGet package metadata configured
- [ ] Deterministic build configured
- [ ] Project added to SICCARV3.sln solution file

## Implementation Notes

(Notes will be added during implementation)

## Review Checklist

- [ ] Code follows constitutional principles
- [ ] Project structure matches specification
- [ ] Dependencies are minimal
- [ ] Build configuration is correct
- [ ] Documentation generation works
- [ ] Multi-targeting verified

---

**Task Control**
- **Created By:** Claude Code
- **Reviewed By:** (Pending)
- **Approved By:** (Pending)
