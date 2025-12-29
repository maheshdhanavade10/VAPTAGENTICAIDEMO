#!/usr/bin/env bash
# Local Development Runner for VAPT Components

echo "🛠️ Local VAPT Development Tools"
echo "================================"

# Function to run setup checks
run_setup_check() {
    echo "🔍 Running setup validation..."
    python scripts/test_setup.py
}

# Function to test API key
test_api_key() {
    echo "🔑 Testing OpenAI API key..."
    python scripts/test_api_key.py
}

# Function to simulate workflow
simulate_workflow() {
    echo "🚀 Simulating GitHub Actions workflow..."
    python scripts/simulate_workflow.py
}

# Function to run basic security checks
run_security_checks() {
    echo "🛡️ Running basic security checks..."
    echo "Checking for common vulnerabilities..."

    # Check for secrets in code
    if grep -r "password\|secret\|api_key" --include="*.cs" --include="*.py" . --exclude-dir=node_modules 2>/dev/null; then
        echo "⚠️ Found potential secrets in code - review above findings"
    else
        echo "✅ No obvious secrets found in code"
    fi

    # Check for SQL injection patterns
    if grep -r "SELECT.*FROM.*+" --include="*.cs" . 2>/dev/null; then
        echo "⚠️ Found potential SQL injection patterns - review above"
    else
        echo "✅ No obvious SQL injection patterns found"
    fi
}

# Function to build .NET project
build_dotnet() {
    echo "🔨 Building .NET project..."
    cd VaptTestingDemo.API
    if dotnet build --configuration Release; then
        echo "✅ .NET build successful"
    else
        echo "❌ .NET build failed"
    fi
    cd ..
}

# Function to run Trivy locally (if installed)
run_trivy_local() {
    echo "🔍 Running Trivy vulnerability scan..."
    if command -v trivy &> /dev/null; then
        trivy fs --format table --output trivy-local-results.txt .
        echo "✅ Trivy scan completed - results in trivy-local-results.txt"
    else
        echo "❌ Trivy not installed locally"
        echo "   Install: https://aquasecurity.github.io/trivy/"
        echo "   Or use Docker: docker run aquasecurity/trivy fs ."
    fi
}

# Function to check GitHub CLI
check_github_cli() {
    echo "🐙 Checking GitHub CLI..."
    if command -v gh &> /dev/null; then
        echo "✅ GitHub CLI available"
        gh --version
    else
        echo "❌ GitHub CLI not installed"
        echo "   Install: https://cli.github.com/"
    fi
}

# Main menu
show_menu() {
    echo ""
    echo "Available commands:"
    echo "  1) Run setup validation"
    echo "  2) Test OpenAI API key"
    echo "  3) Simulate GitHub Actions workflow"
    echo "  4) Run basic security checks"
    echo "  5) Build .NET project"
    echo "  6) Run Trivy scan locally"
    echo "  7) Check GitHub CLI"
    echo "  8) Run all checks"
    echo "  q) Quit"
    echo ""
}

# Run all checks
run_all() {
    run_setup_check
    echo ""
    test_api_key
    echo ""
    run_security_checks
    echo ""
    build_dotnet
    echo ""
    run_trivy_local
    echo ""
    check_github_cli
    echo ""
    simulate_workflow
}

# Main loop
while true; do
    show_menu
    read -p "Choose an option (1-8 or q): " choice

    case $choice in
        1) run_setup_check ;;
        2) test_api_key ;;
        3) simulate_workflow ;;
        4) run_security_checks ;;
        5) build_dotnet ;;
        6) run_trivy_local ;;
        7) check_github_cli ;;
        8) run_all ;;
        q|Q) echo "Goodbye! 👋"; exit 0 ;;
        *) echo "Invalid option. Please choose 1-8 or q." ;;
    esac

    echo ""
    read -p "Press Enter to continue..."
done