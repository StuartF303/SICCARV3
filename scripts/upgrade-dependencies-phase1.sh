#!/bin/bash

# SICCARV3 Dependency Upgrade Script - Phase 1 (Low-Risk)
# This script upgrades packages with minor version changes

set -e

DRY_RUN=false
VERBOSE=false

# Parse arguments
while [[ $# -gt 0 ]]; do
  case $1 in
    --dry-run)
      DRY_RUN=true
      shift
      ;;
    --verbose)
      VERBOSE=true
      shift
      ;;
    *)
      echo "Unknown option: $1"
      exit 1
      ;;
  esac
done

SOLUTION_ROOT="/c/Projects/SICCARV3"
cd "$SOLUTION_ROOT"

echo "====================================="
echo "SICCARV3 Dependency Upgrade - Phase 1"
echo "====================================="
echo ""

if [ "$DRY_RUN" = true ]; then
  echo "[DRY RUN MODE] - No changes will be made"
  echo ""
fi

# Define packages to upgrade (Name|Version|Category)
declare -a packages=(
  # Microsoft Core Packages
  "Microsoft.AspNetCore.Authentication.JwtBearer|8.0.11|Microsoft Core"
  "Microsoft.AspNetCore.Authentication.OpenIdConnect|8.0.11|Microsoft Core"
  "Microsoft.EntityFrameworkCore|8.0.11|Microsoft Core"
  "Microsoft.EntityFrameworkCore.SqlServer|8.0.11|Microsoft Core"
  "Microsoft.EntityFrameworkCore.Design|8.0.11|Microsoft Core"
  "Microsoft.Extensions.Http.Polly|8.0.11|Microsoft Core"
  "Microsoft.AspNetCore.OData|8.2.5|Microsoft Core"

  # Dapr
  "Dapr.AspNetCore|1.14.0|Dapr"
  "Dapr.Extensions.Configuration|1.14.0|Dapr"

  # Validation
  "FluentValidation|11.10.0|Validation"

  # Azure
  "Azure.Identity|1.13.1|Azure"
  "Azure.Extensions.AspNetCore.DataProtection.Blobs|1.3.4|Azure"
  "Azure.Extensions.AspNetCore.DataProtection.Keys|1.2.4|Azure"

  # API/Documentation
  "Swashbuckle.AspNetCore|6.9.0|API"
  "Swashbuckle.AspNetCore.Annotations|6.9.0|API"
  "Asp.Versioning.Mvc|8.1.0|API"
  "Asp.Versioning.Mvc.ApiExplorer|8.1.0|API"

  # Health Checks
  "AspNetCore.HealthChecks.AzureKeyVault|8.0.1|Health Checks"
  "AspNetCore.HealthChecks.MongoDb|8.1.0|Health Checks"
  "AspNetCore.HealthChecks.MySql|8.0.1|Health Checks"

  # Other
  "Microsoft.VisualStudio.Azure.Containers.Tools.Targets|1.21.0|Tools"
  "JsonLogic|4.1.0|JSON"
  "IdentityModel.AspNetCore|4.4.0|Identity"
)

# Track results
UPDATED=0
SKIPPED=0
ERRORS=0

# Find all .csproj files
echo "Searching for .csproj files..."
PROJECT_FILES=$(find src -name "*.csproj" -type f)
PROJECT_COUNT=$(echo "$PROJECT_FILES" | wc -l)

echo "Found $PROJECT_COUNT project files"
echo ""

# Process packages
CURRENT_CATEGORY=""

for package_info in "${packages[@]}"; do
  IFS='|' read -r PKG_NAME PKG_VERSION PKG_CATEGORY <<< "$package_info"

  if [ "$CURRENT_CATEGORY" != "$PKG_CATEGORY" ]; then
    CURRENT_CATEGORY="$PKG_CATEGORY"
    echo "=== $CURRENT_CATEGORY ==="
  fi

  echo "Upgrading $PKG_NAME to $PKG_VERSION..."

  UPDATED_PROJECTS=0

  while IFS= read -r project_file; do
    if [ -z "$project_file" ]; then
      continue
    fi

    # Check if project references this package
    if grep -q "<PackageReference.*Include=\"$PKG_NAME\"" "$project_file"; then

      # Extract current version
      CURRENT_VERSION=$(grep -oP "<PackageReference.*Include=\"$PKG_NAME\".*Version=\"\K[^\"]*" "$project_file" || echo "")

      if [ -n "$CURRENT_VERSION" ] && [ "$CURRENT_VERSION" != "$PKG_VERSION" ]; then
        PROJECT_NAME=$(basename "$project_file")
        echo "  - $PROJECT_NAME: $CURRENT_VERSION → $PKG_VERSION"

        if [ "$DRY_RUN" = false ]; then
          PROJECT_DIR=$(dirname "$project_file")
          cd "$PROJECT_DIR"

          if dotnet add package "$PKG_NAME" --version "$PKG_VERSION" &>/dev/null; then
            ((UPDATED_PROJECTS++))
            ((UPDATED++))
          else
            echo "    ERROR: Failed to update package"
            ((ERRORS++))
          fi

          cd "$SOLUTION_ROOT"
        else
          ((UPDATED_PROJECTS++))
        fi
      elif [ -n "$CURRENT_VERSION" ] && [ "$CURRENT_VERSION" = "$PKG_VERSION" ]; then
        if [ "$VERBOSE" = true ]; then
          PROJECT_NAME=$(basename "$project_file")
          echo "  - $PROJECT_NAME: Already at $PKG_VERSION"
        fi
        ((SKIPPED++))
      fi
    fi
  done <<< "$PROJECT_FILES"

  if [ $UPDATED_PROJECTS -gt 0 ]; then
    echo "  ✓ Updated in $UPDATED_PROJECTS project(s)"
  else
    echo "  ℹ No projects needed updating"
  fi

  echo ""
done

# Summary
echo ""
echo "====================================="
echo "SUMMARY"
echo "====================================="
echo "Packages Updated: $UPDATED"
echo "Packages Skipped (already current): $SKIPPED"
echo "Errors: $ERRORS"
echo ""

if [ "$DRY_RUN" = true ]; then
  echo "This was a DRY RUN - no actual changes were made"
  echo "Run without --dry-run to apply changes"
else
  echo "Next steps:"
  echo "1. Run: dotnet restore"
  echo "2. Run: dotnet build"
  echo "3. Run: dotnet test"
  echo "4. Review and commit changes if all tests pass"
fi

echo ""
echo "For detailed upgrade plan, see: DEPENDENCY_UPGRADE_PLAN.md"
