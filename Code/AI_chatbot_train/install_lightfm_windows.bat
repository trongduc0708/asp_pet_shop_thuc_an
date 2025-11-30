@echo off
REM Script to install lightfm on Windows with workarounds

echo ========================================
echo Installing lightfm for Windows
echo ========================================

echo.
echo Step 1: Installing dependencies...
pip install numpy scipy scikit-learn

echo.
echo Step 2: Trying to install lightfm from git (most reliable)...
pip install git+https://github.com/lyst/lightfm.git

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ lightfm installed successfully!
    echo.
    echo Step 3: Installing remaining packages...
    pip install fastapi==0.115.0 uvicorn[standard]==0.32.0 torch==2.6.0 pandas==2.2.3 scikit-learn==1.5.2 pydantic==2.9.2 python-multipart==0.0.12 sentence-transformers>=2.2.0 scipy>=1.11.0
    echo.
    echo ✅ All packages installed!
) else (
    echo.
    echo ❌ Failed to install lightfm from git. Trying alternative method...
    echo.
    echo Step 2b: Trying with --no-build-isolation...
    pip install lightfm --no-build-isolation
    
    if %ERRORLEVEL% EQU 0 (
        echo.
        echo ✅ lightfm installed successfully!
        echo.
        echo Step 3: Installing remaining packages...
        pip install fastapi==0.115.0 uvicorn[standard]==0.32.0 torch==2.6.0 pandas==2.2.3 scikit-learn==1.5.2 pydantic==2.9.2 python-multipart==0.0.12 sentence-transformers>=2.2.0 scipy>=1.11.0
        echo.
        echo ✅ All packages installed!
    ) else (
        echo.
        echo ❌ All methods failed. Please use Conda instead (see INSTALL_WINDOWS.md)
        exit /b 1
    )
)

echo.
echo ========================================
echo Installation complete!
echo ========================================
pause

