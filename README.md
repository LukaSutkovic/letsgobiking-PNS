# LetsGoBiking 🚴‍♂️

**Projet Middleware 2025-2026 - Polytech Nice-Sophia**
_Auteurs : Maveyraud & Sutkovic_

## 📋 Description du Projet

**LetsGoBiking** est une plateforme de routage multimodale distribuée. Elle orchestre plusieurs services pour fournir le chemin le plus rapide entre deux points en combinant intelligemment la marche à pied et les vélos en libre-service JCDecaux.

L'architecture repose sur une approche **Service-Oriented (SOA)**, intégrant des communications REST, SOAP et un système de messagerie asynchrone (MOM) pour le temps réel.

## 🌟 Fonctionnalités Clés

- **Algorithme Multimodal "Légal" :** Calcul d'itinéraires complexes (Vélo ville A -\> Marche inter-cités -\> Vélo ville B).
- **Architecture Hybride :** Le serveur principal expose simultanément une API REST (Web) et un endpoint SOAP (Client Lourd).
- **Optimisation par Proxy Cache :** Intermédiaire gérant les appels JCDecaux pour réduire la latence et respecter les quotas.
- **Notifications Temps Réel :** Diffusion d'alertes (Météo, Pollution) via un Broker ActiveMQ et WebSockets (STOMP).
- **Visualisation Cartographique :** Tracé GPS précis sur carte interactive (Leaflet).
- **Interopérabilité :** Client Web (JS) et Client Lourd (Java) consommant le même service métier.

## 🛠️ Architecture Technique

Le système est composé de 5 modules distincts :

### 1\. RoutingService (Le Cerveau)

- **Techno :** C\# .NET 8 (ASP.NET Core).
- **Rôle :** Orchestrateur métier.
- **Particularité :** Serveur Hybride.
  - Interface REST (JSON) pour le Web.
  - Interface SOAP (WSDL via SoapCore) pour le client Java.
- **Logique :** Intègre la comparaison de temps et la fusion de géométries GPS.

### 2\. Proxy Cache JCDecaux

- **Techno :** C\# .NET.
- **Rôle :** Intermédiaire avec l'API JCDecaux.
- **Fonction :** Met en cache les stations d'une ville pour éviter les appels redondants aux API externes.

### 3\. Écosystème Temps Réel (ActiveMQ)

- **Broker :** Apache ActiveMQ (Classic).
- **NotifService (Producer) :** Service C\# autonome qui publie des événements (Météo/Pollution) sur des topics.
- **Frontend (Consumer) :** Le site web s'abonne aux topics via le protocole STOMP sur WebSocket pour afficher des alertes dynamiques sur la carte.

### 4\. Client Web (Frontend)

- **Techno :** HTML5, CSS3, JavaScript (Vanilla), Web Components.
- **Rôle :** Interface utilisateur légère.
- **Features :** Autocomplétion (API Gouv), LocalStorage pour la persistance, Carte Leaflet.

### 5\. Heavy Client (Client Lourd)

- **Techno :** Java (OpenJDK 17+), Maven, JAX-WS.
- **Rôle :** Démonstrateur d'interopérabilité.
- **Fonction :** Consomme le RoutingService via le protocole SOAP (XML) en utilisant des classes proxy générées automatiquement depuis le WSDL.

---

## 🚀 Installation et Lancement

### Prérequis

- **Windows** (recommandé pour le script `.bat`).
- **.NET 8.0 SDK** (Routing & Notif Services).
- **Java JDK 17+** (Pour ActiveMQ et le Client Java).
- **Python 3** (Pour servir le frontend localement).
- Une connexion Internet (API OpenRouteService & JCDecaux).

### ⚡ Lancement Automatique (Recommandé)

Un script d'orchestration `run.bat` est fourni à la racine. Il automatise le déploiement de l'infrastructure complète.

1.  Double-cliquez sur `run.bat`.
2.  Le script va lancer séquentiellement :
    - Le Broker ActiveMQ.
    - Le Proxy Cache.
    - Le RoutingService (Port 5173).
    - Le Service de Notifications.
    - Le Client Lourd Java (Compilation + Exécution).
    - Un serveur Web Python (Port 8000).
3.  Votre navigateur s'ouvrira automatiquement sur la page d'accueil.

> **Note :** Ne fermez pas les fenêtres de terminal qui s'ouvrent, ce sont les services en cours d'exécution.

### 🛠️ Lancement Manuel (Débogage)

Si nécessaire, les services peuvent être lancés individuellement :

- **ActiveMQ :**
  ```bash
  apache-activemq-5.19.1-bin/apache-activemq-5.19.1/bin/activemq start
  ```
- **RoutingService :**
  ```bash
  dotnet run --urls="http://localhost:5173"
  ```
  _(dans le dossier du projet)_
- **Proxy :**
  Lancer l'exécutable dans `ProxyHost/bin/Debug`.
- **Client Java :**
  ```bash
  mvn exec:java
  ```
  _(dans le dossier HeavyClientJava)_
- **Frontend :**
  ```bash
  python -m http.server 8000
  ```
  _(à la racine)_

---

## 📝 Crédits API

Ce projet utilise les services tiers suivants :

- **JCDecaux Open Data** (Disponibilité vélos).
- **OpenRouteService** (Géocodage et Calcul d'itinéraires).
- **Data.gouv.fr** (Autocomplétion d'adresses).
- **OpenStreetMap** (Tuiles cartographiques).
