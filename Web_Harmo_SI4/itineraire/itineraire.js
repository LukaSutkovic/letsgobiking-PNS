document.addEventListener("DOMContentLoaded", async () => {
  // --- 1. Sélection des éléments ---
  const reminderDiv = document.querySelector("#itinerary-reminder");
  const stepsPanel = document.querySelector("#steps-panel");
  const startRerouteComponent = document.querySelector("#start-point-reroute");
  const endRerouteComponent = document.querySelector("#end-point-reroute");

  // --- NOUVEAU : Initialisation de la carte ---
  // On crée la carte dans la div id="mapid"
  // On la centre sur Lyon ([45.76, 4.83]) avec un zoom de 13 pour tester
  const map = L.map("mapid").setView([45.76, 4.83], 13);

  // On ajoute les "tuiles" (le fond de carte) OpenStreetMap
  L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    attribution:
      '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
  }).addTo(map);

  // --- 2. CONFIGURATION API (IMPORTANT) ---
  // Remplace 5132 par ton port Swagger si ce n'est pas celui-là
  const API_BASE_URL = "http://localhost:5000/api/routing";

  // --- 3. Récupération du LocalStorage ---
  const itineraryDataString = localStorage.getItem("itineraryData");
  let itinerary = { start: null, end: null };

  if (itineraryDataString) {
    itinerary = JSON.parse(itineraryDataString);

    if (itinerary.start && itinerary.end) {
      // A. Mise à jour de la barre latérale (Ton code)
      reminderDiv.innerHTML = `
                <p><strong>Départ :</strong> ${itinerary.start.properties.label}</p>
                <p><strong>Arrivée :</strong> ${itinerary.end.properties.label}</p>
            `;

      // B. APPEL AU SERVEUR C# (Le nouveau code)
      try {
        stepsPanel.innerHTML = `... (ton HTML de chargement) ...`;

        const labelDepart = itinerary.start.properties.label;
        const labelArrivee = itinerary.end.properties.label;
        const url = `${API_BASE_URL}?depart=${encodeURIComponent(
          labelDepart
        )}&arrivee=${encodeURIComponent(labelArrivee)}`;

        console.log("Appel API C# :", url);

        const response = await fetch(url);
        if (!response.ok) throw new Error("Erreur serveur");

        // IMPORTANT : On lit maintenant du JSON, plus du texte !
        const data = await response.json();

        // 1. Afficher le texte (data.description)
        // Note : Le C# renvoie les propriétés avec une majuscule (Description),
        // mais le JSON les met souvent en minuscule (description) ou majuscule selon la config.
        // Par sécurité, vérifie la console si ça ne s'affiche pas.
        const descriptionText = data.description || data.Description;

        stepsPanel.innerHTML = `
                    <h3>Détail des étapes</h3>
                    <div style="white-space: pre-wrap; ...">
                        ${descriptionText}
                    </div>
                `;

        // 2. Dessiner sur la carte (data.geometry)
        const geometry = data.geometry || data.Geometry;

        if (geometry && geometry.length > 0) {
          // ATTENTION : GeoJSON est [Long, Lat], mais Leaflet veut [Lat, Long].
          // On doit inverser chaque point.
          const latLngs = geometry.map((coord) => [coord[1], coord[0]]);

          // On crée la ligne bleue (polyline)
          const polyline = L.polyline(latLngs, {
            color: "blue",
            weight: 5,
          }).addTo(map);

          // On zoome la carte pour voir tout le trajet
          map.fitBounds(polyline.getBounds());
        }
      } catch (error) {
        console.error(error);
        stepsPanel.innerHTML = `
                    <h3>Oups !</h3>
                    <p style="color: red;">Impossible de récupérer l'itinéraire.</p>
                    <p>Vérifiez que votre serveur C# est bien lancé sur le port <strong>5132</strong> (ou modifiez le port dans le fichier js).</p>
                `;
      }
    } else {
      reminderDiv.innerHTML = `<p>Les données de l'itinéraire sont incomplètes.</p>`;
    }
  } else {
    reminderDiv.innerHTML = `<p>Aucun itinéraire défini. <a href="../index.html">Retour à l'accueil</a></p>`;
  }

  // --- 4. Logique de changement de trajet (Améliorée) ---

  // On initialise avec les valeurs existantes pour ne pas perdre l'autre point si on en change qu'un seul
  let newItinerary = {
    start: itinerary.start,
    end: itinerary.end,
  };

  startRerouteComponent.addEventListener("addressSelected", (event) => {
    console.log("Nouveau départ sélectionné !");
    newItinerary.start = event.detail.address;
    updateAndReload();
  });

  endRerouteComponent.addEventListener("addressSelected", (event) => {
    console.log("Nouvelle arrivée sélectionnée !");
    newItinerary.end = event.detail.address;
    updateAndReload();
  });

  function updateAndReload() {
    // On vérifie qu'on a bien les deux points avant de recharger
    if (newItinerary.start && newItinerary.end) {
      console.log("Nouvel itinéraire complet. Mise à jour et rechargement.");
      localStorage.setItem("itineraryData", JSON.stringify(newItinerary));
      window.location.reload();
    }
  }
});
