@echo off
setlocal

REM chemin racine du repo = dossier où se trouve ce .bat
set ROOT=%~dp0

echo ==========================================
echo      Lancement de LetsGoBiking !
echo ==========================================
echo.

REM ------------------------------------------
REM 0/3 - COMPILATION 
REM ------------------------------------------
echo [0/3] Build de la solution...
cd /d "%ROOT%"
call dotnet build "LetsGoBiking.sln" --configuration Debug
if %errorlevel% neq 0 (
    echo Erreur de compilation, on stoppe.
    pause
    exit /b
)
echo.

REM ------------------------------------------
REM 1/3 - LANCEMENT DU PROXY JCDECAUX
REM ------------------------------------------
echo [1/3] Demarrage du Proxy JCDecaux...

REM On essaye plusieurs chemins possibles
if exist "ProxyHost\bin\Debug\net48\ProxyHost.exe" (
    set "PROXY_EXE=%ROOT%ProxyHost\bin\Debug\net48\ProxyHost.exe"
) else if exist "ProxyHost\bin\Debug\net472\ProxyHost.exe" (
    set "PROXY_EXE=%ROOT%ProxyHost\bin\Debug\net472\ProxyHost.exe"
) else (
    set "PROXY_EXE=%ROOT%ProxyHost\bin\Debug\ProxyHost.exe"
)

if not exist "%PROXY_EXE%" (
    echo ProxyHost.exe introuvable ^(pas de build ?^)
    pause
    exit /b
)

REM definir le working directory sur bin\Debug
start "JCDecaux Proxy" /D "%~dp0ProxyHost\bin\Debug" "%PROXY_EXE%"

timeout /t 4 /nobreak >nul

REM ------------------------------------------
REM 2/3 - API ROUTING C#
REM ------------------------------------------
echo [2/3] Demarrage du Serveur REST (RoutingService)...
cd /d "%ROOT%RoutingService\RoutingService"
start "LetsGoBiking API" dotnet run --no-build --urls="http://localhost:5173"

timeout /t 4 /nobreak >nul

REM ------------------------------------------
REM 3/3 - SERVEUR WEB STATIQUE 
REM ------------------------------------------
echo [3/3] Demarrage du Serveur Web Local...
cd /d "%ROOT%"
start "LetsGoBiking WebServer" python -m http.server 8000

REM ------------------------------------------
REM OUVERTURE DU NAVIGATEUR
REM ------------------------------------------
echo Ouverture du navigateur...
timeout /t 2 /nobreak >nul
start http://localhost:8000/Web_Harmo_SI4/accueil.html

echo.
echo ==========================================
echo      Tout est pret !
echo ==========================================
pause
