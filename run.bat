@echo off
setlocal
set ROOT=%~dp0

echo ==========================================
echo      Lancement de LetsGoBiking !
echo ==========================================
echo.

REM ------------------------------------------
REM ETAPE 0 : COMPILATION GLOBALE
REM ------------------------------------------
echo [0/4] Nettoyage et Construction de la solution complete...

REM On se place a la racine ou se trouve le .sln
cd /d "%ROOT%"

REM 1. On nettoie tout
call dotnet clean "LetsGoBiking.sln" --verbosity quiet

REM 2. On compile le projet d'un coup (Proxy, Core, Routing, etc.)
call dotnet build "LetsGoBiking.sln" --configuration Debug

REM Verification : Si la compilation echoue, on arrete tout.
if %errorlevel% neq 0 (
    echo.
    echo ERREUR CRITIQUE : La compilation a echoue. Verifiez les erreurs ci-dessus.
    pause
    exit /b
)

echo.
echo [SUCCES] Tout a ete compile correctement. Lancement des services...
echo.

REM ------------------------------------------
REM 1/4 - PROXY JCDECAUX (WCF)
REM ------------------------------------------
echo [1/4] Demarrage du Proxy JCDecaux...

REM On cherche l'exécutable généré par la solution
REM (Priorité au dossier net48 car c'est ta config)
if exist "ProxyHost\bin\Debug\net48\ProxyHost.exe" (
    set "PROXY_PATH=ProxyHost\bin\Debug\net48\ProxyHost.exe"
) else if exist "ProxyHost\bin\Debug\net472\ProxyHost.exe" (
    set "PROXY_PATH=ProxyHost\bin\Debug\net472\ProxyHost.exe"
) else (
    set "PROXY_PATH=ProxyHost\bin\Debug\ProxyHost.exe"
)

REM Important : On lance l'exe en definissant son dossier de travail (working directory)
start "JCDecaux Proxy" /D "%ROOT%ProxyHost" "%ROOT%%PROXY_PATH%"

timeout /t 4 /nobreak >nul

REM ------------------------------------------
REM 2/4 - API ROUTING (REST)
REM ------------------------------------------
echo [2/4] Demarrage du Serveur REST...
cd /d "%ROOT%RoutingService\RoutingService"

start "LetsGoBiking API" dotnet run --no-build --urls="http://localhost:5173"

timeout /t 4 /nobreak >nul

REM ------------------------------------------
REM 3/4 - SERVEUR WEB (PYTHON)
REM ------------------------------------------
echo [3/4] Demarrage du Serveur Web...
cd /d "%ROOT%"
start "LetsGoBiking WebServer" python -m http.server 8000

REM ------------------------------------------
REM 4/4 - NAVIGATEUR
REM ------------------------------------------
echo [4/4] Ouverture du navigateur...
timeout /t 2 /nobreak >nul
start http://localhost:8000/Web_Harmo_SI4/accueil.html

echo.
echo ==========================================
echo      Tout est pret ! Enjoy !
echo ==========================================
pause