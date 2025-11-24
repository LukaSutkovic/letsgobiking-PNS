@echo off
setlocal

REM chemin racine du repo = dossier où se trouve ce .bat
set ROOT=%~dp0

echo ==========================================
echo      Lancement de LetsGoBiking !
echo ==========================================
echo.

REM ------------------------------------------
<<<<<<< HEAD
REM 0/3 - NETTOYAGE (IMPORTANT POUR LE FRONT)
REM ------------------------------------------
echo [Nettoyage] Fermeture des anciens serveurs...
taskkill /F /IM dotnet.exe >nul 2>&1
taskkill /F /IM python.exe >nul 2>&1
REM On tue aussi le Proxy s'il tourne deja
taskkill /F /IM ProxyHost.exe >nul 2>&1

REM ------------------------------------------
REM 1/3 - COMPILATION 
REM ------------------------------------------
echo [1/3] Build de la solution...
=======
REM 0/3 - COMPILATION 
REM ------------------------------------------
echo [0/3] Build de la solution...
>>>>>>> e51141041042421f64460e232c51f505375363c0
cd /d "%ROOT%"
call dotnet build "LetsGoBiking.sln" --configuration Debug
if %errorlevel% neq 0 (
    echo Erreur de compilation, on stoppe.
    pause
    exit /b
)
echo.

REM ------------------------------------------
<<<<<<< HEAD
REM 2/3 - LANCEMENT DU PROXY JCDECAUX
REM ------------------------------------------
echo [2/3] Demarrage du Proxy JCDecaux...
=======
REM 1/3 - LANCEMENT DU PROXY JCDECAUX
REM ------------------------------------------
echo [1/3] Demarrage du Proxy JCDecaux...
>>>>>>> e51141041042421f64460e232c51f505375363c0

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

<<<<<<< HEAD
REM definir le working directory sur bin\Debug pour qu'il trouve ses fichiers de config
start "JCDecaux Proxy" /D "%~dp0ProxyHost\bin\Debug" "%PROXY_EXE%"

REM Petit temps de pause pour laisser le proxy demarrer
timeout /t 2 /nobreak >nul

REM ------------------------------------------
REM 3/3 - API ROUTING C#
REM ------------------------------------------
echo [3/3] Demarrage du Serveur REST (RoutingService)...
cd /d "%ROOT%RoutingService\RoutingService"
REM On lance sur le port 5173 comme demande
=======
REM definir le working directory sur bin\Debug
start "JCDecaux Proxy" /D "%~dp0ProxyHost\bin\Debug" "%PROXY_EXE%"

timeout /t 4 /nobreak >nul

REM ------------------------------------------
REM 2/3 - API ROUTING C#
REM ------------------------------------------
echo [2/3] Demarrage du Serveur REST (RoutingService)...
cd /d "%ROOT%RoutingService\RoutingService"
>>>>>>> e51141041042421f64460e232c51f505375363c0
start "LetsGoBiking API" dotnet run --no-build --urls="http://localhost:5173"

timeout /t 4 /nobreak >nul

REM ------------------------------------------
<<<<<<< HEAD
REM 4/3 - SERVEUR WEB STATIQUE 
REM ------------------------------------------
echo [4/3] Demarrage du Serveur Web Local...
=======
REM 3/3 - SERVEUR WEB STATIQUE 
REM ------------------------------------------
echo [3/3] Demarrage du Serveur Web Local...
>>>>>>> e51141041042421f64460e232c51f505375363c0
cd /d "%ROOT%"
REM Le serveur Python servira les fichiers frais
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
<<<<<<< HEAD
echo      ASTUCE : Fais CTRL+F5 sur la page web
echo      pour vider le cache du navigateur.
=======
>>>>>>> e51141041042421f64460e232c51f505375363c0
echo ==========================================
pause
