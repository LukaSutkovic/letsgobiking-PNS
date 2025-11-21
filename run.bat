@echo off
setlocal

echo ==========================================
echo      Lancement de LetsGoBiking !
echo ==========================================

REM On se place dans le dossier du script
cd /d "%~dp0"

echo.
echo [0/3] Demarrage du Proxy JCDecaux (WCF)...
cd ProxyHost\bin\Debug
start "LetsGoBiking Proxy" ProxyHost.exe
cd /d "%~dp0"

echo.
echo [1/3] Compilation et Demarrage du Serveur REST (RoutingService)...
cd RoutingService\RoutingService
start "LetsGoBiking API" dotnet run --no-build --urls="http://localhost:5173"
timeout /t 5 /nobreak >nul

echo.
echo [2/3] Demarrage du Serveur Web Local (python http.server)...
cd /d "%~dp0"
start "LetsGoBiking WebServer" python -m http.server 8000

echo.
echo [3/3] Ouverture du navigateur...
timeout /t 2 /nobreak >nul
start http://localhost:8000/Web_Harmo_SI4/accueil.html

echo.
echo ==========================================
echo      Tout est pret !
echo ==========================================
pause
endlocal