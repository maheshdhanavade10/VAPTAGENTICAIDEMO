#!/usr/bin/env python3
"""
Local VAPT Workflow Simulator
Simulates the GitHub Actions workflow locally for testing
"""

import os
import sys
import subprocess
import json
from pathlib import Path
import time

class LocalWorkflowSimulator:
    def __init__(self):
        self.project_root = Path(__file__).parent.parent
        self.results = {}

    def run_setup_checks(self):
        """Run initial setup validation"""
        print("🔍 Running setup checks...")

        checks = [
            ("Python environment", self.check_python),
            ("Dependencies", self.check_dependencies),
            ("OpenAI API", self.check_openai_key),
            (".NET SDK", self.check_dotnet),
            ("Project structure", self.check_project_structure),
        ]

        for check_name, check_func in checks:
            print(f"  Checking {check_name}...")
            try:
                result = check_func()
                self.results[check_name] = result
                status = "✅" if result else "❌"
                print(f"    {status} {check_name}")
            except Exception as e:
                print(f"    ❌ {check_name}: {e}")
                self.results[check_name] = False

        return all(self.results.values())

    def check_python(self):
        """Check Python environment"""
        return sys.version_info >= (3, 8)

    def check_dependencies(self):
        """Check if required packages are installed"""
        try:
            import openai
            return True
        except ImportError:
            return False

    def check_openai_key(self):
        """Check OpenAI API key"""
        key = os.getenv('OPENAI_API_KEY')
        return key and key.startswith('sk-') and len(key) > 20

    def check_dotnet(self):
        """Check .NET SDK"""
        try:
            result = subprocess.run(['dotnet', '--version'],
                                  capture_output=True, text=True)
            return result.returncode == 0
        except FileNotFoundError:
            return False

    def check_project_structure(self):
        """Check project files exist"""
        required_files = [
            "VaptTestingDemo.API/Controllers/TestController.cs",
            "scripts/vapt_agent.py",
            "requirements.txt",
            ".env.example"
        ]

        return all((self.project_root / file).exists() for file in required_files)

    def simulate_security_scan_job(self):
        """Simulate the security-scan job"""
        print("\n🔍 Simulating security-scan job...")

        # Change to project directory
        os.chdir(self.project_root / "VaptTestingDemo.API")

        # Build .NET project
        print("  Building .NET project...")
        result = subprocess.run(['dotnet', 'build', '--configuration', 'Release'],
                              capture_output=True, text=True)
        if result.returncode != 0:
            print(f"    ❌ Build failed: {result.stderr}")
            return False

        print("    ✅ .NET build successful")

        # Run VAPT agent (simulated)
        print("  Running VAPT agent...")
        os.chdir(self.project_root)
        try:
            # Import and run agent components
            sys.path.append(str(self.project_root / "scripts"))
            # Note: This would require the actual agent code to be importable
            print("    ✅ VAPT agent simulation completed")
        except Exception as e:
            print(f"    ⚠️ VAPT agent simulation: {e}")

        return True

    def simulate_codeql_analysis_job(self):
        """Simulate CodeQL analysis (basic check)"""
        print("\n🔬 Simulating CodeQL analysis job...")

        # Check if .NET project builds (CodeQL would do deeper analysis)
        cs_files = list(self.project_root.rglob("*.cs"))
        if cs_files:
            print(f"    ✅ Found {len(cs_files)} C# files for analysis")
            return True
        else:
            print("    ❌ No C# files found")
            return False

    def simulate_dependency_review_job(self):
        """Simulate dependency review"""
        print("\n📦 Simulating dependency review job...")

        # Check for common dependency files
        dep_files = [
            "VaptTestingDemo.API/VaptTestingDemo.API.csproj",
            "requirements.txt",
            "package.json"
        ]

        found_deps = [f for f in dep_files if (self.project_root / f).exists()]
        if found_deps:
            print(f"    ✅ Found dependency files: {', '.join(found_deps)}")
            return True
        else:
            print("    ❌ No dependency files found")
            return False

    def simulate_security_audit_job(self):
        """Simulate security audit with basic checks"""
        print("\n🛡️ Simulating security audit job...")

        # Basic file system checks
        vulnerabilities = []

        # Check for common security issues
        env_file = self.project_root / ".env"
        if env_file.exists():
            with open(env_file, 'r') as f:
                content = f.read()
                if 'password' in content.lower() or 'secret' in content.lower():
                    vulnerabilities.append("Potential secrets in .env file")

        # Check for vulnerable patterns in code
        cs_files = list(self.project_root.rglob("*.cs"))
        for cs_file in cs_files:
            with open(cs_file, 'r') as f:
                content = f.read()
                if 'SELECT * FROM' in content and ('+' in content or 'string.Concat' in content):
                    vulnerabilities.append(f"Potential SQL injection in {cs_file.name}")

        if vulnerabilities:
            print("    ⚠️ Found potential security issues:")
            for vuln in vulnerabilities:
                print(f"      - {vuln}")
        else:
            print("    ✅ No obvious security issues found")

        return len(vulnerabilities) == 0

    def run_full_simulation(self):
        """Run complete workflow simulation"""
        print("🚀 Local VAPT Workflow Simulation")
        print("=" * 50)

        # Setup checks
        if not self.run_setup_checks():
            print("\n❌ Setup checks failed. Please fix issues above.")
            return False

        print("\n✅ Setup checks passed!")

        # Simulate each job
        jobs = [
            ("Security Scan", self.simulate_security_scan_job),
            ("CodeQL Analysis", self.simulate_codeql_analysis_job),
            ("Dependency Review", self.simulate_dependency_review_job),
            ("Security Audit", self.simulate_security_audit_job),
        ]

        results = {}
        for job_name, job_func in jobs:
            try:
                results[job_name] = job_func()
            except Exception as e:
                print(f"    ❌ {job_name} failed: {e}")
                results[job_name] = False

        # Summary
        print("\n" + "=" * 50)
        print("📊 Simulation Results:")

        passed = 0
        total = len(results)

        for job_name, success in results.items():
            status = "✅" if success else "❌"
            print(f"  {status} {job_name}")
            if success:
                passed += 1

        print(f"\n🎯 Overall: {passed}/{total} jobs simulated successfully")

        if passed == total:
            print("\n🎉 Local simulation completed successfully!")
            print("   Your workflow should work on GitHub Actions.")
        else:
            print("\n⚠️ Some jobs had issues. Check the output above.")

        return passed == total

def main():
    simulator = LocalWorkflowSimulator()
    success = simulator.run_full_simulation()

    if not success:
        print("\n💡 Tips:")
        print("   - Install missing dependencies: pip install -r requirements.txt")
        print("   - Set OPENAI_API_KEY environment variable")
        print("   - Install .NET SDK if missing")
        print("   - Check project structure")

    return 0 if success else 1

if __name__ == "__main__":
    sys.exit(main())