#!/usr/bin/env python3
"""
Fix NuGet.Config files to have XML declaration as first line
"""

import os
import re
from pathlib import Path

def fix_nuget_config(file_path):
    """Fix a single nuget.config file"""
    with open(file_path, 'r', encoding='utf-8-sig') as f:
        content = f.read()

    # Check if needs fixing
    lines = content.split('\n')

    # If first line is not <?xml, needs fixing
    if not lines[0].strip().startswith('<?xml'):
        # Remove any embedded XML declaration and comment markers
        content = re.sub(r'<\?xml[^>]*\?>\s*', '', content)
        content = re.sub(r'^\s*<!--\s*\/?\*?\s*', '', content, flags=re.MULTILINE)

        # Build proper header
        header = '<?xml version="1.0" encoding="utf-8"?>\n<!--\n'

        # Add header
        content = header + content

        # Write back
        with open(file_path, 'w', encoding='utf-8', newline='\n') as f:
            f.write(content)

        return True

    # Check if comment is malformed (starts with * instead of <!--)
    if len(lines) > 1 and lines[1].strip().startswith('*'):
        # Remove any embedded XML declaration
        content = '\n'.join(lines[1:])
        content = re.sub(r'<\?xml[^>]*\?>\s*', '', content)

        # Build proper header
        header = '<?xml version="1.0" encoding="utf-8"?>\n<!--\n'

        # Add header
        content = header + content

        # Write back
        with open(file_path, 'w', encoding='utf-8', newline='\n') as f:
            f.write(content)

        return True

    return False

def main():
    root_dir = Path('c:/Projects/SICCARV3')

    # Find all nuget.config files
    config_files = list(root_dir.rglob('[Nn]uget.[Cc]onfig'))

    print(f"Found {len(config_files)} NuGet config files")
    print()

    fixed_count = 0
    for config_file in config_files:
        if fix_nuget_config(config_file):
            print(f"[FIXED] {config_file.relative_to(root_dir)}")
            fixed_count += 1
        else:
            print(f"[SKIP] {config_file.relative_to(root_dir)}")

    print()
    print(f"Fixed {fixed_count} files")

if __name__ == '__main__':
    main()
