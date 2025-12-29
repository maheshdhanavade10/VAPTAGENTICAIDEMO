#!/usr/bin/env python3
"""
Test script for VAPT Agent setup validation
"""

import os
import sys
import subprocess
from pathlib import Path

def check_python_version():
    """Check Python version"""
    version = sys.version_info
    if version.major < 3 or (version.major == 3 and version.minor < 8):
        print("❌ Python 3.8+ required")
        return False
    print(f"✅ Python {version.major}.{version.minor}.{version.micro}")
    return True

def check_dependencies():
    """Check if required packages can be imported"""
    try:
        import openai
        print("✅ OpenAI package available")
        return True
    except ImportError:
        print("❌ OpenAI package not installed. Run: pip install -r requirements.txt")
        return False

def check_openai_key():
    """Check if OpenAI API key is set"""
    key = os.getenv('OPENAI_API_KEY')
    if not key:
        print("❌ OPENAI_API_KEY environment variable not set")
        print("   Copy .env.example to .env and set your API key")
        return False
    if len(key) < 20:
        print("❌ OPENAI_API_KEY appears to be invalid (too short)")
        return False
    print("✅ OPENAI_API_KEY is set")
    return True

def check_dotnet():
    """Check if .NET SDK is available"""
    try:
        result = subprocess.run(['dotnet', '--version'], capture_output=True, text=True)
        if result.returncode == 0:
            print(f"✅ .NET SDK {result.stdout.strip()}")
            return True
        else:
            print("❌ .NET SDK not found")
            return False
    except FileNotFoundError:
        print("❌ .NET SDK not found")
        return False

def check_git():
    """Check if Git is available"""
    try:
        result = subprocess.run(['git', '--version'], capture_output=True, text=True)
        if result.returncode == 0:
            print(f"✅ Git {result.stdout.strip()}")
            return True
        else:
            print("❌ Git not found")
            return False
    except FileNotFoundError:
        print("❌ Git not found")
        return False

def check_github_cli():
    """Check if GitHub CLI is available"""
    try:
        result = subprocess.run(['gh', '--version'], capture_output=True, text=True)
        if result.returncode == 0:
            print(f"✅ GitHub CLI {result.stdout.split()[2]}")
            return True
        else:
            print("❌ GitHub CLI not found")
            return False
    except FileNotFoundError:
        print("❌ GitHub CLI not found")
        print("   Install from: https://cli.github.com/")
        return False

def check_project_structure():
    """Check if project files exist"""
    required_files = [
        "VaptTestingDemo.API/Controllers/TestController.cs",
        "scripts/vapt_agent.py",
        "requirements.txt",
        ".github/workflows/vapt-security.yml"
    ]

    repo_root = Path(__file__).parent
    all_present = True

    for file_path in required_files:
        full_path = repo_root / file_path
        if full_path.exists():
            print(f"✅ {file_path}")
        else:
            print(f"❌ {file_path} - MISSING")
            all_present = False

    return all_present

def main():
    print("🔍 VAPT Agent Setup Validation")
    print("=" * 40)

    checks = [
        ("Python Version", check_python_version),
        ("Project Structure", check_project_structure),
        ("Dependencies", check_dependencies),
        ("OpenAI API Key", check_openai_key),
        (".NET SDK", check_dotnet),
        ("Git", check_git),
        ("GitHub CLI", check_github_cli),
    ]

    passed = 0
    total = len(checks)

    for name, check_func in checks:
        print(f"\n{name}:")
        if check_func():
            passed += 1

    print(f"\n{'=' * 40}")
    print(f"Setup Check Results: {passed}/{total} checks passed")

    if passed == total:
        print("🎉 All checks passed! You're ready to run the VAPT Agent.")
        print("\nNext steps:")
        print("1. Run: python scripts/vapt_agent.py")
        print("2. Or push to GitHub to trigger the automated workflow")
    else:
        print("❌ Some checks failed. Please fix the issues above before running the agent.")
        sys.exit(1)

if __name__ == "__main__":
    main()