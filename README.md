# VAPT Agentic AI Demo

An autonomous AI-powered system for Vulnerability Assessment and Penetration Testing (VAPT) that automatically scans code, identifies security issues, generates fixes, and manages the remediation workflow through pull requests.

## 🚀 Features

- **Autonomous Security Scanning**: AI-powered analysis of code for OWASP Top 10 vulnerabilities
- **Automated Code Fixes**: Generates and applies security patches with proper documentation
- **Git Workflow Automation**: Creates branches, commits, and pull requests automatically
- **Review Integration**: Supports human review before merging security fixes
- **Multi-Scanner Support**: Integrates CodeQL, Trivy, and custom AI analysis

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Code Scanner  │ -> │   AI Analyzer   │ -> │   Fix Generator │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         v                       v                       v
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Code Modifier  │ -> │   Git Manager   │ -> │     PR Creator  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 📋 Prerequisites

- Python 3.8+ (https://python.org)
- .NET 8.0 SDK (https://dotnet.microsoft.com)
- Git (https://git-scm.com)
- GitHub CLI (`gh`) (https://cli.github.com)
- OpenAI API Key (https://platform.openai.com)

## 🛠️ Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd VAPTAgenticAIDemo
   ```

2. **Install Python dependencies**
   ```bash
   pip install -r requirements.txt
   ```

3. **Set up environment variables**
   ```bash
   export OPENAI_API_KEY="your-openai-api-key"
   ```

5. **Validate setup**
   ```bash
   python scripts/test_setup.py
   ```

6. **GitHub setup helper**
   ```bash
   python scripts/github_setup.py
   ```

7. **Build the .NET project**
   ```bash
   cd VaptTestingDemo.API
   dotnet build
   ```

## 🚀 Usage

### Manual Execution

Run the VAPT agent manually:

```bash
python scripts/vapt_agent.py
```

### Automated Workflow

The agent runs automatically via GitHub Actions on:
- **Push to protected branches**: `main` or `develop`
- **Pull request creation**: Targeting `main` branch
- **Manual trigger**: Via GitHub UI workflow dispatch

#### GitHub Setup Requirements:
1. **Repository on GitHub**: Push this code to a GitHub repository
2. **Actions enabled**: Ensure GitHub Actions is enabled (default for public repos)
3. **Secrets configured**: 
   - Go to **Settings** → **Secrets and variables** → **Actions**
   - Add `OPENAI_API_KEY` with your OpenAI API key
4. **Branch protection**: Optional, but recommended for `main`

#### What Happens Automatically:
1. **Code Checkout**: GitHub pulls the latest code
2. **Environment Setup**: Installs Python, .NET, dependencies
3. **Security Scanning**: Runs CodeQL, Trivy, and AI analysis
4. **VAPT Agent Execution**: AI scans for vulnerabilities and generates fixes
5. **PR Creation**: If vulnerabilities found, creates a security fix PR
6. **Artifact Upload**: Saves scan results and backups

#### Workflow Jobs:
- **security-scan**: Main VAPT agent execution
- **codeql-analysis**: GitHub's CodeQL security analysis  
- **dependency-review**: Checks for vulnerable dependencies
- **security-audit**: Trivy container vulnerability scanning

### Workflow Steps

1. **Security Scanning**: AI analyzes the codebase for vulnerabilities
2. **Fix Generation**: Creates secure code patches with documentation
3. **Code Application**: Applies fixes to the source code
4. **Git Operations**: Creates branch, commits changes, pushes to remote
5. **PR Creation**: Generates pull request with detailed security report
6. **Review Process**: Human review and approval
7. **Merge**: Final merge after approval

## 🔧 Configuration

### Environment Variables

- `OPENAI_API_KEY`: Your OpenAI API key for AI analysis
- `GITHUB_TOKEN`: Automatically provided by GitHub Actions

### Customization

Modify `scripts/vapt_agent.py` to:
- Add new vulnerability patterns
- Customize fix generation logic
- Integrate additional security scanners
- Modify PR templates

## 📊 Security Scanners

The system integrates multiple security analysis tools:

- **AI-Powered Analysis**: Custom GPT-4 analysis for OWASP Top 10
- **CodeQL**: GitHub's semantic code analysis engine
- **Trivy**: Comprehensive vulnerability scanner
- **Dependency Review**: Checks for vulnerable dependencies

## 🎯 Supported Vulnerabilities

- SQL Injection
- Cross-Site Scripting (XSS)
- Broken Authentication
- Insecure Deserialization
- Sensitive Data Exposure
- Broken Access Control
- Security Misconfiguration
- Server-Side Request Forgery (SSRF)
- Command Injection
- And more OWASP Top 10 categories

## 📝 API Endpoints

The demo includes intentionally vulnerable endpoints for testing:

- `GET /api/test/sql` - SQL Injection demo
- `GET /api/test/xss` - XSS vulnerability
- `GET /api/test/auth` - Broken authentication
- `GET /api/test/deserialization` - Insecure deserialization
- `GET /api/test/data` - Sensitive data exposure
- `GET /api/test/cmd` - Command injection
- `GET /api/test/admin` - Broken access control
- `GET /api/test/config` - Security misconfiguration
- `GET /api/test/ssrf` - Server-side request forgery
- `POST /api/test/login` - Weak authentication

## 🤖 AI Agent Capabilities

The VAPT Agent can:
- Analyze C# ASP.NET Core code for security issues
- Generate context-aware security fixes
- Add comprehensive documentation
- Create detailed pull request descriptions
- Maintain code functionality while improving security

## 🔒 Security Considerations

- **API Keys**: Never commit API keys to version control
- **Review Process**: Always review AI-generated fixes
- **Testing**: Run comprehensive tests after applying fixes
- **Backup**: Original vulnerable code is backed up automatically

## 📈 Monitoring and Logging

- All agent actions are logged
- Security scan results are uploaded as artifacts
- PR creation includes detailed vulnerability reports
- GitHub Security tab integration for findings

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run the VAPT agent to check for new vulnerabilities
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## ⚠️ Disclaimer

This tool is for educational and demonstration purposes. Always review AI-generated security fixes carefully before applying them to production systems. The authors are not responsible for any security issues arising from the use of this tool.
