# LetsGoBiking 🚴‍♂️

**Projet Middleware 2025-2026 - Polytech Nice-Sophia** *Auteurs : Maveyraud & Sutkovic*

---

## 📋 Description du Projet

**LetsGoBiking** est une application de routage multimodale intelligente. Elle permet aux utilisateurs de trouver le chemin le plus rapide entre deux points géographiques en combinant la **marche à pied** et l'utilisation des vélos en libre-service **JCDecaux**.

L'application calcule automatiquement si l'utilisation d'un vélo est avantageuse par rapport à la marche et gère les itinéraires complexes inter-villes.

### 🌟 Fonctionnalités Principales (MVP)
- **Calcul d'itinéraire mixte :** Marche -> Vélo -> Marche.
- **Optimisation Multicontrats :** Gestion intelligente des trajets entre deux villes différentes (Logique "Légale").
- **Carte Interactive :** Visualisation du tracé GPS précis sur une carte (Leaflet/OpenStreetMap).
- **Autocomplétion :** Recherche d'adresses intuitive via l'API data.gouv.fr.
- **Architecture REST :** Serveur Backend en ASP.NET Core.
- **Client Web :** Interface moderne utilisant des Web Components et le LocalStorage.

---

## 🛠️ Architecture Technique

Le projet est divisé en deux parties principales :

1.  **RoutingService (Backend - C#)**
    - API REST développée avec **ASP.NET Core (.NET 8)**.
    - Interagit avec les API externes : **JCDecaux** (Disponibilité vélos) et **OpenRouteService** (Geocoding & Tracés).
    - Algorithme de comparaison de temps et de fusion de géométries.

2.  **Client Web (Frontend - HTML/JS)**
    - Application web sans framework (Vanilla JS).
    - Utilisation de **Web Components** pour la modularité.
    - Communication asynchrone (`fetch`) avec le RoutingService.

---

## 🚀 Comment lancer le projet ?

### Prérequis
- **.NET 8.0 SDK** (pour le serveur API).
- **Python 3** (création du serveur local pour le front-end)

### Méthode Automatique (Script) ⚡
1. Double-cliquez sur le fichier **`run.bat`** à la racine.
2. Le script va lancer :
   - Le serveur API C# (port 5173).
   - Un serveur web local Python (port 8000).
   - Votre navigateur sur `http://localhost:8000/Web_Harmo_SI4/accueil.html`.

### Méthode Manuelle (VS Code) 🛠️
Si vous préférez utiliser Visual Studio Code & VS2022 :
1. Ouvrez le dossier `RoutingService` et lancez le projet C# (F5).
2. Ouvrez le dossier `Web_Harmo_SI4`.
3. Faites un clic droit sur `accueil.html` -> **Open with Live Server** (extension VSC)

**Lancer le Serveur Backend**
Ouvrez un terminal dans le dossier `RoutingService/RoutingService` et exécutez :
```bash
dotnet run --no-build --urls="http://localhost:5173"
