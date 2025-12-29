#!/usr/bin/env python3
"""
GitHub Setup Helper for VAPT Agent
"""

import os
import subprocess
import sys

def check_git_repo():
    """Check if current directory is a Git repository"""
    try:
        result = subprocess.run(['git', 'status'], capture_output=True, text=True)
        return result.returncode == 0
    except FileNotFoundError:
        return False

def check_github_remote():
    """Check if GitHub remote is configured"""
    try:
        result = subprocess.run(['git', 'remote', '-v'], capture_output=True, text=True)
        return 'github.com' in result.stdout
    except FileNotFoundError:
        return False

def get_repo_info():
    """Get repository information"""
    try:
        # Get remote URL
        result = subprocess.run(['git', 'config', '--get', 'remote.origin.url'],
                              capture_output=True, text=True)
        if result.returncode == 0:
            url = result.stdout.strip()
            if 'github.com' in url:
                # Extract owner/repo
                if url.startswith('https://'):
                    parts = url.replace('https://github.com/', '').replace('.git', '').split('/')
                elif url.startswith('git@'):
                    parts = url.replace('git@github.com:', '').replace('.git', '').split('/')
                else:
                    parts = []
                if len(parts) >= 2:
                    return f"{parts[0]}/{parts[1]}"
    except:
        pass
    return None

def main():
    print("🔧 GitHub Setup Helper for VAPT Agent")
    print("=" * 45)

    if not check_git_repo():
        print("❌ Not a Git repository")
        print("   Initialize Git: git init")
        print("   Add remote: git remote add origin <github-url>")
        return

    print("✅ Git repository detected")

    if not check_github_remote():
        print("❌ No GitHub remote configured")
        print("   Add GitHub remote: git remote add origin https://github.com/owner/repo.git")
        return

    repo = get_repo_info()
    if repo:
        print(f"✅ GitHub repository: {repo}")
    else:
        print("❌ Could not determine repository name")

    print("\n📋 Next Steps for GitHub Actions:")
    print("1. Push this code to GitHub:")
    print("   git add .")
    print("   git commit -m 'Add VAPT Agent'")
    print("   git push -u origin main")
    print()
    print("2. Configure GitHub Secrets:")
    print("   - Go to: https://github.com/{}/settings/secrets/actions".format(repo or "owner/repo"))
    print("   - Add: OPENAI_API_KEY = your-openai-api-key")
    print()
    print("3. Enable GitHub Actions (if disabled)")
    print()
    print("4. The workflow will run automatically on next push!")
    print()
    print("🎯 Test the workflow:")
    print("   - Go to Actions tab in GitHub")
    print("   - Select 'VAPT Security Scan and Auto-Fix'")
    print("   - Click 'Run workflow'")

if __name__ == "__main__":
    main()