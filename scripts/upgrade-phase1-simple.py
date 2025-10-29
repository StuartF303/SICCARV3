#!/usr/bin/env python3
"""
SICCAR V3 - Phase 1 Dependency Upgrades
Simple script to upgrade low-risk packages
"""

import subprocess
import os
from pathlib import Path

# Phase 1 package upgrades (low-risk)
UPGRADES = [
    # Microsoft.AspNetCore.* packages
    ("Microsoft.AspNetCore.Authentication.JwtBearer", "8.0.11"),
    ("Microsoft.AspNetCore.Authentication.OpenIdConnect", "8.0.11"),

    # Microsoft.EntityFrameworkCore.* packages
    ("Microsoft.EntityFrameworkCore", "8.0.11"),
    ("Microsoft.EntityFrameworkCore.Design", "8.0.11"),
    ("Microsoft.EntityFrameworkCore.Tools", "8.0.11"),
    ("Pomelo.EntityFrameworkCore.MySql", "8.0.2"),

    # Microsoft.Extensions.* packages
    ("Microsoft.Extensions.Configuration", "8.0.0"),
    ("Microsoft.Extensions.Configuration.Json", "8.0.1"),
    ("Microsoft.Extensions.Hosting", "8.0.1"),
    ("Microsoft.Extensions.Http.Polly", "8.0.11"),

    # Dapr packages
    ("Dapr.AspNetCore", "1.14.0"),
    ("Dapr.Client", "1.14.0"),
    ("Dapr.Extensions.Configuration", "1.14.0"),

    # Azure packages
    ("Azure.Identity", "1.11.4"),

    # Swashbuckle
    ("Swashbuckle.AspNetCore", "6.8.1"),

    # Health checks
    ("AspNetCore.HealthChecks.Dapr", "8.0.2"),
    ("AspNetCore.HealthChecks.MySql", "8.0.1"),
    ("AspNetCore.HealthChecks.MongoDb", "8.1.0"),
    ("AspNetCore.HealthChecks.Redis", "8.0.1"),
    ("AspNetCore.HealthChecks.UI", "8.0.2"),
    ("AspNetCore.HealthChecks.UI.Client", "8.0.1"),
    ("AspNetCore.HealthChecks.UI.InMemory.Storage", "8.0.1"),

    # FluentValidation
    ("FluentValidation", "11.10.0"),
    ("FluentValidation.AspNetCore", "11.3.0"),
    ("FluentValidation.DependencyInjectionExtensions", "11.10.0"),

    # Testing packages
    ("xunit", "2.9.2"),
    ("xunit.runner.visualstudio", "2.8.2"),
    ("coverlet.collector", "6.0.2"),
    ("Microsoft.NET.Test.Sdk", "17.11.1"),
]

def find_csproj_files(root_dir):
    """Find all .csproj files in the project"""
    return list(Path(root_dir).rglob("*.csproj"))

def upgrade_package(csproj_path, package_name, version):
    """Upgrade a package in a specific project"""
    try:
        result = subprocess.run(
            ["dotnet", "add", str(csproj_path), "package", package_name, "--version", version],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            universal_newlines=True,
            timeout=60
        )

        if result.returncode == 0:
            print(f"[OK] {csproj_path.name}: {package_name} -> {version}")
            return True
        else:
            # Check if package doesn't exist in project
            if "does not have a package" in result.stderr or "No package references" in result.stderr:
                return None  # Package not in this project
            else:
                print(f"[ERROR] {csproj_path.name}: {package_name}")
                print(f"  {result.stderr.strip()[:200]}")
                return False
    except subprocess.TimeoutExpired:
        print(f"[TIMEOUT] {csproj_path.name}: {package_name}")
        return False
    except Exception as e:
        print(f"[EXCEPTION] {csproj_path.name}: {package_name} - {str(e)}")
        return False

def main():
    os.chdir(r"c:\Projects\SICCARV3")

    print("=" * 50)
    print("Phase 1 Dependency Upgrades")
    print("=" * 50)
    print()

    # Find all projects
    projects = find_csproj_files(".")
    print(f"Found {len(projects)} project files")
    print()

    # Upgrade each package
    for package_name, version in UPGRADES:
        print(f"Upgrading {package_name} to {version}...")
        success_count = 0
        error_count = 0
        skip_count = 0

        for csproj in projects:
            result = upgrade_package(csproj, package_name, version)
            if result is True:
                success_count += 1
            elif result is False:
                error_count += 1
            else:
                skip_count += 1

        print(f"  Upgraded: {success_count}, Errors: {error_count}, Skipped: {skip_count}")
        print()

    print("=" * 50)
    print("Phase 1 upgrades complete!")
    print("=" * 50)

if __name__ == "__main__":
    main()
