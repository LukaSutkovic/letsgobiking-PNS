@echo off
setlocal enableextensions enabledelayedexpansion

REM ============================================================
REM CONFIGURATION DU CHEMIN RACINE
REM ============================================================
set "ROOT=%~dp0"
echo Racine detectee : "%ROOT%"

echo.
echo ============================================================
echo      LANCEMENT DE LETSGOBIKING (MODE DEBUG)
echo ============================================================
echo.

REM ------------------------------------------------------------
REM 0/7 - NETTOYAGE DES PROCESSUS
REM ------------------------------------------------------------
echo [0/7] Nettoyage en cours...
taskkill /F /IM dotnet.exe >nul 2>&1
taskkill /F /IM python.exe >nul 2>&1
taskkill /F /IM ProxyHost.exe >nul 2>&1
echo Nettoyage termine.

REM ------------------------------------------------------------
REM 1/7 - COMPILATION C#
REM ------------------------------------------------------------
echo.
echo [1/7] Build de la solution .NET...
cd /d "%ROOT%"
if not exist "LetsGoBiking.sln" (
    echo [ERREUR] Le fichier LetsGoBiking.sln est introuvable ici : %ROOT%
    pause
    exit /b
)

call dotnet build "LetsGoBiking.sln" --configuration Debug
if %errorlevel% neq 0 (
    echo [ERREUR] La compilation a echoue.
    pause
    exit /b
)

REM ------------------------------------------------------------
REM 2/7 - LANCEMENT ACTIVEMQ
REM ------------------------------------------------------------
echo.
echo [2/7] Demarrage d'ActiveMQ...

REM Adapte ce dossier si besoin. Ne mets pas de slash a la fin.
set "AMQ_DIR=apache-activemq-5.19.1-bin\apache-activemq-5.19.1\bin"
set "AMQ_FULLPATH=%ROOT%%AMQ_DIR%"

if exist "%AMQ_FULLPATH%\activemq.bat" (
    echo Lancement depuis : "%AMQ_FULLPATH%"
    start "ActiveMQ Broker" /D "%AMQ_FULLPATH%" activemq start
    echo ... ActiveMQ demarre, attente de 10s ...
    timeout /t 10 /nobreak >nul
) else (
    echo [AVERTISSEMENT] ActiveMQ introuvable ici : "%AMQ_FULLPATH%"
    echo Les notifications ne marcheront pas, mais on continue.
    pause
)

REM ------------------------------------------------------------
REM 3/7 - LANCEMENT PROXY JCDECAUX
REM ------------------------------------------------------------
echo.
echo [3/7] Recherche du Proxy...

REM On cherche l'exe le plus probable
if exist "ProxyHost\bin\Debug\net48\ProxyHost.exe" (
    set "PROXY_PATH=ProxyHost\bin\Debug\net48"
    set "PROXY_EXE=ProxyHost.exe"
) else if exist "ProxyHost\bin\Debug\ProxyHost.exe" (
    set "PROXY_PATH=ProxyHost\bin\Debug"
    set "PROXY_EXE=ProxyHost.exe"
) else (
    set "PROXY_PATH="
)

if defined PROXY_PATH (
    echo Proxy trouve dans : %PROXY_PATH%
    start "JCDecaux Proxy" /D "%ROOT%%PROXY_PATH%" %PROXY_EXE%
    timeout /t 2 >nul
) else (
    echo [ERREUR] ProxyHost.exe introuvable. Verifiez la compilation.
    pause
    exit /b
)

REM ------------------------------------------------------------
REM 4/7 - LANCEMENT ROUTINGSERVICE
REM ------------------------------------------------------------
echo.
echo [4/7] Demarrage du RoutingService...
cd /d "%ROOT%RoutingService\RoutingService"
start "LetsGoBiking API" dotnet run --no-build --urls="http://localhost:5173"
timeout /t 4 >nul

REM ------------------------------------------------------------
REM 5/7 - SERVICE NOTIFICATIONS
REM ------------------------------------------------------------
echo.
echo [5/7] Service de Notifications...
cd /d "%ROOT%NotifService"
start "NotifService" dotnet run --no-build
cd /d "%ROOT%"

REM ------------------------------------------------------------
REM 6/7 - HEAVY CLIENT JAVA
REM ------------------------------------------------------------
echo.
echo [6/7] Lancement du Client Java...
cd /d "%ROOT%HeavyClientJava"

echo Lancement direct via Maven...

start "HeavyClient Java" cmd /k "mvn clean compile exec:java"

cd /d "%ROOT%"

REM ------------------------------------------------------------
REM 7/7 - SERVEUR WEB & NAVIGATEUR
REM ------------------------------------------------------------
echo.
echo [7/7] Lancement du Web...
start "LetsGoBiking WebServer" python -m http.server 8000

timeout /t 2 >nul
echo Ouverture de l'accueil...
start http://localhost:8000/Web_Harmo_SI4/accueil.html

echo.
echo ============================================================
echo      TOUT EST LANCE ! BON COURAGE !
echo ============================================================
pause