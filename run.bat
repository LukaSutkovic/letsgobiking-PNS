@echo off
setlocal

REM chemin racine du repo = dossier où se trouve ce .bat
set ROOT=%~dp0

echo ==========================================
echo      Lancement de LetsGoBiking !
echo ==========================================
echo.

REM ------------------------------------------
REM 0/6 - NETTOYAGE
REM ------------------------------------------
echo [Nettoyage] Fermeture des anciens serveurs...
taskkill /F /IM dotnet.exe >nul 2>&1
taskkill /F /IM python.exe >nul 2>&1
REM NE PAS TUER java.exe -> ActiveMQ tourne en Java
REM taskkill /F /IM java.exe >nul 2>&1
taskkill /F /IM ProxyHost.exe >nul 2>&1

REM ------------------------------------------
REM 1/6 - COMPILATION C#
REM ------------------------------------------
echo [1/6] Build de la solution .NET...
cd /d "%ROOT%"
call dotnet build "LetsGoBiking.sln" --configuration Debug
if %errorlevel% neq 0 (
    echo Erreur de compilation .NET
    pause
    exit /b
)
echo.

REM ------------------------------------------
REM 2/6 - LANCEMENT PROXY JCDECAUX
REM ------------------------------------------
echo [2/6] Demarrage du Proxy JCDecaux...

if exist "ProxyHost\bin\Debug\net48\ProxyHost.exe" (
    set "PROXY_EXE=%ROOT%ProxyHost\bin\Debug\net48\ProxyHost.exe"
) else if exist "ProxyHost\bin\Debug\net472\ProxyHost.exe" (
    set "PROXY_EXE=%ROOT%ProxyHost\bin\Debug\net472\ProxyHost.exe"
) else (
    set "PROXY_EXE=%ROOT%ProxyHost\bin\Debug\ProxyHost.exe"
)

if not exist "%PROXY_EXE%" (
    echo ProxyHost.exe introuvable
    pause
    exit /b
)

start "JCDecaux Proxy" /D "%~dp0ProxyHost\bin\Debug" "%PROXY_EXE%"
timeout /t 2 >nul

REM ------------------------------------------
REM 3/6 - LANCEMENT ROUTINGSERVICE REST + SOAP
REM ------------------------------------------
echo [3/6] Demarrage du RoutingService (.NET)...

cd /d "%ROOT%RoutingService\RoutingService"
start "LetsGoBiking API" dotnet run --no-build --urls="http://localhost:5173"
timeout /t 4 >nul

REM ------------------------------------------
REM 4/6 - SERVICE NOTIFICATIONS (ActiveMQ)
REM ------------------------------------------
echo [4/6] Demarrage du service de notifications...
cd /d "%ROOT%NotifService"
start "NotifService" dotnet run --no-build
cd /d "%ROOT%"

REM ------------------------------------------
REM 5/6 - LANCEMENT HEAVY CLIENT JAVA
REM ------------------------------------------
echo [5/6] Lancement du HeavyClient Java...

cd /d "%ROOT%HeavyClientJava"
call mvn -q clean package

if exist "target\*.jar" (
    for %%F in (target\*.jar) do (
        start "HeavyClient Java" java -jar "%%F"
        goto after_java
    )
)

:after_java
cd /d "%ROOT%"

REM ------------------------------------------
REM 6/6 - SERVEUR WEB STATIQUE
REM ------------------------------------------
echo [6/6] Demarrage du serveur web local...
cd /d "%ROOT%"
start "LetsGoBiking WebServer" python -m http.server 8000

timeout /t 2 >nul
start http://localhost:8000/Web_Harmo_SI4/accueil.html

echo.
echo ==========================================
echo      Tout est pret !
echo ==========================================
pause
