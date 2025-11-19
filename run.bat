@echo off
echo ==========================================
echo      Lancement de LetsGoBiking ! 🚴‍♂️
echo ==========================================

echo.
echo [1/3] Demarrage du Serveur REST (Backend)...
cd RoutingService/RoutingService
:: On lance le serveur C# sur le port 5132 (vérifie que c'est bien le tien)
start "LetsGoBiking API" dotnet run --urls="http://localhost:5173"

:: On attend que le C# démarre
timeout /t 5 /nobreak >nul

echo.
echo [2/3] Demarrage du Serveur Web Local (Frontend)...
cd ../..
cd Web_Harmo_SI4

:: C'est ici la magie : on lance un mini serveur Python sur le port 8000
:: Le "start" permet d'ouvrir une nouvelle fenêtre pour que ça tourne en fond
start "LetsGoBiking WebServer" python -m http.server 8000

echo.
echo [3/3] Ouverture du navigateur...
timeout /t 2 /nobreak >nul
:: On ouvre l'adresse locale du serveur Python sur la page accueil.html
start http://localhost:8000/accueil.html

echo.
echo ==========================================
echo      Tout est pret !
echo      Ne fermez pas les fenetres noires.
echo ==========================================
pause