@echo off
REM ============================================
REM Script Setup SQL Server untuk Production
REM ============================================

echo ============================================
echo SETUP APLIKASI CHECK DIMENSI - SQL SERVER
echo ============================================
echo.

REM 1. Install SQL Server Package
echo [1/6] Installing SQL Server package...
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
if errorlevel 1 (
    echo ERROR: Failed to install SQL Server package
    pause
    exit /b 1
)
echo SUCCESS: SQL Server package installed
echo.

REM 2. Restore packages
echo [2/6] Restoring packages...
dotnet restore
if errorlevel 1 (
    echo ERROR: Failed to restore packages
    pause
    exit /b 1
)
echo SUCCESS: Packages restored
echo.

REM 3. Build project
echo [3/6] Building project...
dotnet build -c Release
if errorlevel 1 (
    echo ERROR: Build failed
    pause
    exit /b 1
)
echo SUCCESS: Build completed
echo.

REM 4. Update Program.cs notification
echo [4/6] MANUAL STEP REQUIRED!
echo Please update Program.cs:
echo   Change: options.UseSqlite(...)
echo   To:     options.UseSqlServer(...)
echo.
echo Press any key after you have updated Program.cs...
pause > nul
echo.

REM 5. Remove old migrations (optional)
echo [5/6] Do you want to remove old SQLite migrations? (Y/N)
set /p remove_migrations=
if /i "%remove_migrations%"=="Y" (
    echo Removing old migrations...
    if exist "Migrations" (
        rmdir /s /q Migrations
        echo Old migrations removed
    )
)
echo.

REM 6. Create new migrations
echo [6/6] Creating SQL Server migrations...
dotnet ef migrations add InitialSQLServer
if errorlevel 1 (
    echo ERROR: Failed to create migrations
    echo Make sure you have updated Program.cs to use SqlServer
    pause
    exit /b 1
)
echo SUCCESS: Migrations created
echo.

echo ============================================
echo NEXT STEPS:
echo ============================================
echo 1. Make sure SQL Server is running
echo 2. Run SQLServer_Setup.sql in SQL Server Management Studio
echo    OR
echo    Run: dotnet ef database update
echo.
echo 3. Update appsettings.Production.json with correct connection string
echo 4. Test the application: dotnet run --environment Production
echo.
echo 5. If everything works, publish:
echo    dotnet publish -c Release -o ./publish
echo.
echo ============================================
pause
