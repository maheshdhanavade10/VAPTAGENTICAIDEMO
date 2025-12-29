@echo off
REM Local VAPT Workflow Runner for Windows
REM This simulates the GitHub Actions workflow locally

echo ========================================
echo 🚀 Local VAPT Workflow Simulation
echo ========================================
echo.

echo 🔍 Step 1: Checking system requirements...
echo.

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET SDK not found
    echo    Please install .NET 8.0 from: https://dotnet.microsoft.com/download
    goto :error
) else (
    echo ✅ .NET SDK found
    dotnet --version
)

echo.
echo 🔍 Step 2: Checking project structure...
echo.

REM Check for required files
if not exist "VaptTestingDemo.API\Controllers\TestController.cs" (
    echo ❌ TestController.cs not found
    goto :error
) else (
    echo ✅ TestController.cs found
)

if not exist "scripts\vapt_agent.py" (
    echo ❌ VAPT agent script not found
    goto :error
) else (
    echo ✅ VAPT agent script found
)

if not exist "requirements.txt" (
    echo ❌ Requirements file not found
    goto :error
) else (
    echo ✅ Requirements file found
)

echo.
echo 🔨 Step 3: Building .NET project...
echo.

cd VaptTestingDemo.API
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ❌ .NET build failed
    cd ..
    goto :error
) else (
    echo ✅ .NET build successful
)
cd ..

echo.
echo 🛡️ Step 4: Running basic security checks...
echo.

REM Basic security checks without Python
echo Checking for potential security issues...

REM Check for SQL injection patterns
findstr /R /C:"SELECT.*FROM.*+" "VaptTestingDemo.API\Controllers\*.cs" >nul 2>&1
if %errorlevel% equ 0 (
    echo ⚠️ Found potential SQL injection patterns
) else (
    echo ✅ No obvious SQL injection patterns found
)

REM Check for secrets in code
findstr /I "password\|secret\|api_key" "VaptTestingDemo.API\Controllers\*.cs" >nul 2>&1
if %errorlevel% equ 0 (
    echo ⚠️ Found potential secrets in code
) else (
    echo ✅ No obvious secrets found in code
)

echo.
echo 📦 Step 5: Checking dependencies...
echo.

REM Check .NET packages
cd VaptTestingDemo.API
dotnet list package --format json >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ .NET dependencies accessible
) else (
    echo ⚠️ Could not check .NET dependencies
)
cd ..

echo.
echo 🔬 Step 6: CodeQL analysis simulation...
echo.

REM Count C# files
for /f %%c in ('dir /s /b "VaptTestingDemo.API\*.cs" 2^>nul ^| find /c ".cs"') do set CS_COUNT=%%c
if defined CS_COUNT (
    echo ✅ Found %CS_COUNT% C# files for analysis
) else (
    echo ❌ No C# files found
)

echo.
echo 📊 Step 7: Summary...
echo.

echo ✅ Local workflow simulation completed!
echo.
echo 📋 What was simulated:
echo    • Environment checks
echo    • Project structure validation
echo    • .NET build process
echo    • Basic security scanning
echo    • Dependency analysis
echo    • Code analysis preparation
echo.
echo 💡 To run the full AI-powered VAPT agent:
echo    1. Install Python 3.8+ from https://python.org
echo    2. Install dependencies: pip install -r requirements.txt
echo    3. Set API key: set OPENAI_API_KEY=your-key-here
echo    4. Run: python scripts\vapt_agent.py
echo.
echo 🎯 Your workflow is ready for GitHub Actions deployment!

goto :end

:error
echo.
echo ❌ Simulation failed. Please fix the issues above.
echo.
exit /b 1

:end
echo.
echo Press any key to exit...
pause >nul