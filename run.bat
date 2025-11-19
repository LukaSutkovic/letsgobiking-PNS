@echo off
echo ==========================================
echo      Lancement de LetsGoBiking ! 🚴‍♂️
echo ==========================================

echo.
echo [1/3] Compilation et Demarrage du Serveur REST...
cd RoutingService/RoutingService

start "LetsGoBiking API" dotnet run --no-build --urls="http://localhost:5173"
timeout /t 5 /nobreak >nul

echo.
echo [2/3] Demarrage du Serveur Web Local...
cd ../..
start "LetsGoBiking WebServer" python -m http.server 8000

echo.
echo [3/3] Ouverture du navigateur...
timeout /t 2 /nobreak >nul
:: CORRECTION ICI : On ajoute le nom du dossier dans l'URL
start http://localhost:8000/Web_Harmo_SI4/accueil.html

echo.
echo ==========================================
echo      Tout est pret ! 
echo ==========================================
pause