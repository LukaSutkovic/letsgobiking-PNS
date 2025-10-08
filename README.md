# letsgobiking-PNS
Projet 2025-2026 - LetsGoBiking - Maveyraud &amp; Sutkovic
# Projet de Service de Routage Client-Serveur (SOAP/REST)

Ce projet est une application client-serveur de calcul d'itinéraires. Il met en œuvre un serveur SOAP en C# et un client lourd en Java, conçus pour interagir avec des API externes et fournir des trajets optimisés à l'utilisateur.

Ce projet a été développé dans le cadre du cours **MiddleWare-Soc** de **Polytech Nice-Sophia**.

## 🚀 Fonctionnalités Principales

* **Serveur SOAP (Back-end)** :
    * Expose des web services SOAP pour le calcul d'itinéraires.
    * Agit comme un **proxy** en agrégeant des données de sources multiples.
    * Intègre un système de **mise en cache** pour optimiser les réponses aux requêtes fréquentes.
    * Utilise une **file d'attente (queue)** pour gérer les requêtes complexes de manière asynchrone.
* **Client Lourd (Front-end)** :
    * Interface graphique permettant de saisir un point de départ et une destination.
    * Envoie les requêtes au serveur via le protocole SOAP.
    * Affiche et visualise l'itinéraire retourné sur une carte ou sous forme de liste d'instructions.
* **Intégration d'API Externes** :
    * Consomme l'**API JCDecaux** pour obtenir des informations sur les stations de vélos.
    * S'interface avec une **API REST externe** pour des données de cartographie et de routage.
