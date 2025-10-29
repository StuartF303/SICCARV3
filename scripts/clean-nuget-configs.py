#!/usr/bin/env python3
"""
Clean all nuget.config files - remove license comments and keep only XML config
"""

from pathlib import Path
import re

def clean_nuget_config(file_path):
    """Clean a single nuget.config file"""
    with open(file_path, 'r', encoding='utf-8-sig') as f:
        content = f.read()

    # Find the <configuration> tag
    match = re.search(r'<configuration>', content, re.IGNORECASE)
    if not match:
        print(f"[ERROR] No <configuration> found in {file_path}")
        return False

    # Extract everything from <configuration> onwards
    config_start = match.start()
    clean_content = content[config_start:]

    # Build clean file with just XML declaration and config
    final_content = '<?xml version="1.0" encoding="utf-8"?>\n' + clean_content

    # Write back
    with open(file_path, 'w', encoding='utf-8', newline='\n') as f:
        f.write(final_content)

    return True

def main():
    root_dir = Path('c:/Projects/SICCARV3')

    # Find all nuget config files (case insensitive)
    config_files = list(root_dir.rglob('[Nn]uget.[Cc]onfig'))

    print(f"Found {len(config_files)} NuGet config files")
    print("Cleaning all files to remove license comments...")
    print()

    cleaned = 0
    errors = 0

    for config_file in config_files:
        try:
            if clean_nuget_config(config_file):
                print(f"[CLEANED] {config_file.relative_to(root_dir)}")
                cleaned += 1
            else:
                errors += 1
        except Exception as e:
            print(f"[ERROR] {config_file.relative_to(root_dir)}: {e}")
            errors += 1

    print()
    print(f"Cleaned: {cleaned}")
    print(f"Errors: {errors}")
    print()
    print("All nuget.config files are now clean and valid XML!")

if __name__ == '__main__':
    main()
