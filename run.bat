@echo off
setlocal

REM chemin racine du repo = dossier où se trouve ce .bat
set ROOT=%~dp0

echo ==========================================
echo      Lancement de LetsGoBiking ! 🚴‍♂️
echo ==========================================
echo.

REM ------------------------------------------
REM 0/3 - LANCEMENT DU PROXY JCDECAUX
REM ------------------------------------------
echo [0/3] Demarrage du Proxy JCDecaux...
cd /d "%ROOT%ProxyHost"

REM Première fois, laissez SANS --no-build pour que ça compile
start "JCDecaux Proxy" dotnet run

REM petite pause pour le laisser démarrer
timeout /t 5 /nobreak >nul

REM ------------------------------------------
REM 1/3 - API ROUTING C#
REM ------------------------------------------
echo [1/3] Demarrage du Serveur REST (RoutingService)...
cd /d "%ROOT%RoutingService\RoutingService"

REM Si ça a déjà compilé une fois, mettre --no-build
start "LetsGoBiking API" dotnet run --urls="http://localhost:5173"

timeout /t 5 /nobreak >nul

REM ------------------------------------------
REM 2/3 - SERVEUR WEB STATIQUE (PYTHON)
REM ------------------------------------------
echo [2/3] Demarrage du Serveur Web Local...
cd /d "%ROOT%"
start "LetsGoBiking WebServer" python -m http.server 8000

REM ------------------------------------------
REM 3/3 - OUVERTURE DU NAVIGATEUR
REM ------------------------------------------
echo [3/3] Ouverture du navigateur...
timeout /t 2 /nobreak >nul
start http://localhost:8000/Web_Harmo_SI4/accueil.html

echo.
echo ==========================================
echo      Tout est pret !
echo ==========================================
pause
