let stompClient = null;
const activeSubscriptions = new Map();

// Lancement automatique à la fin du chargement
document.addEventListener("DOMContentLoaded", () => {
  connectToBroker();
});

function connectToBroker() {
  // Connexion au port 61614 (WebSocket par défaut d'ActiveMQ)
  const socket = new WebSocket("ws://localhost:61614/stomp", "stomp");
  stompClient = Stomp.over(socket);
  stompClient.debug = () => {}; // Désactiver le debug verbeux
  stompClient.connect("admin", "admin", onConnected, onError);
}

function onConnected() {
  console.log("[NotifFront] Connecté au broker STOMP");

  // --- LOGIQUE INTELLIGENTE ---

  // 1. Récupérer les préférences stockées depuis l'accueil
  const savedPrefs = localStorage.getItem("notificationPrefs");
  let prefs = null;
  if (savedPrefs) {
    prefs = JSON.parse(savedPrefs);
  }

  // 2. S'abonner selon les préférences (Priorité)
  if (prefs) {
    console.log("[NotifFront] Chargement des préférences utilisateur :", prefs);
    for (const [topic, isChecked] of Object.entries(prefs)) {
      if (isChecked) {
        subscribeTopic(topic);
      }
    }
  }

  // 3. Gestion des cases à cocher (si elles sont présentes sur la page, ex: Accueil)
  const checkboxes = document.querySelectorAll(".notif-topic");
  checkboxes.forEach((cb) => {
    const topicName = cb.dataset.topic;

    // Si on a des prefs, on met à jour l'état visuel de la case
    if (prefs && prefs[topicName] !== undefined) {
      cb.checked = prefs[topicName];
    }
    // Si pas de prefs et pas encore abonné, on s'abonne si c'est coché par défaut dans le HTML
    else if (cb.checked && !activeSubscriptions.has(topicName)) {
      subscribeTopic(topicName);
    }

    // Écouteur pour changement dynamique (si l'utilisateur change d'avis sur l'accueil)
    cb.addEventListener("change", () => {
      if (cb.checked) subscribeTopic(topicName);
      else unsubscribeTopic(topicName);
    });
  });
}

function onError(error) {
  console.error("[NotifFront] Erreur STOMP:", error);
}

function subscribeTopic(topicName) {
  if (activeSubscriptions.has(topicName)) return; // Déjà abonné

  const destination = "/topic/" + topicName;

  if (!stompClient || !stompClient.connected) return;

  const sub = stompClient.subscribe(destination, (msg) => {
    try {
      const payload = JSON.parse(msg.body);
      displayNotification(payload);
      updateMapOverlay(payload);
    } catch (e) {
      console.error(e);
    }
  });

  activeSubscriptions.set(topicName, sub);
  console.log("[NotifFront] Abonné :", topicName);
}

function unsubscribeTopic(topicName) {
  const sub = activeSubscriptions.get(topicName);
  if (sub) {
    sub.unsubscribe();
    activeSubscriptions.delete(topicName);
    console.log("[NotifFront] Désabonné :", topicName);
  }
}

function displayNotification(payload) {
  // On cherche le conteneur. S'il n'existe pas (page sans notifs), on ne fait rien.
  const container = document.getElementById("notifications-stream");
  if (!container) return;

  const severity = (payload.severity || "LOW").toUpperCase();
  const div = document.createElement("div");
  div.className = "notif notif-" + severity.toLowerCase();
  div.innerHTML = `
    <div class="notif-header">
      <span class="notif-topic">${payload.topic}</span>
      <span class="notif-severity">${severity}</span>
    </div>
    <div class="notif-message">${payload.message}</div>
  `;
  container.prepend(div);

  // Limite à 5 notifications visibles pour ne pas polluer
  if (container.children.length > 5) {
    container.lastChild.remove();
  }
}

// --- GESTION CARTE LEAFLET ---
let meteoMarker = null;
let airQualityCircle = null;
let pollutionCircle = null;

function updateMapOverlay(payload) {
  // Vérifie que la carte existe (window.map est défini dans itineraire.js)
  if (!window.map || typeof L === "undefined") return;

  const topic = payload.topic;
  const severity = (payload.severity || "LOW").toUpperCase();
  const center = window.map.getCenter(); // On affiche au centre de la vue actuelle

  if (topic === "meteo") updateMeteoMarker(center, severity);
  else if (topic === "airquality") updateAirQualityCircle(center, severity);
  else if (topic === "pollution") updatePollutionCircle(center, severity);
}

function updateMeteoMarker(center, severity) {
  let iconChar =
    severity === "HIGH" ? "🌧" : severity === "MEDIUM" ? "⛅" : "☀️";
  const icon = L.divIcon({
    className: "meteo-icon",
    html: `<span style="font-size: 30px;">${iconChar}</span>`,
    iconSize: [30, 30],
    iconAnchor: [15, 15],
  });

  if (meteoMarker) {
    meteoMarker.setLatLng(center);
    meteoMarker.setIcon(icon);
  } else {
    meteoMarker = L.marker(center, { icon }).addTo(window.map);
  }
}

function updateAirQualityCircle(center, severity) {
  let color =
    severity === "HIGH"
      ? "#c0392b"
      : severity === "MEDIUM"
      ? "#e67e22"
      : "#27ae60";
  if (airQualityCircle) {
    airQualityCircle.setLatLng(center);
    airQualityCircle.setStyle({ color, fillColor: color });
  } else {
    airQualityCircle = L.circle(center, {
      radius: 1500,
      color,
      fillColor: color,
      fillOpacity: 0.25,
    }).addTo(window.map);
  }
}

function updatePollutionCircle(center, severity) {
  let color =
    severity === "HIGH"
      ? "#8e44ad"
      : severity === "MEDIUM"
      ? "#9b59b6"
      : "#bdc3c7";
  if (pollutionCircle) {
    pollutionCircle.setLatLng(center);
    pollutionCircle.setStyle({ color, fillColor: color });
  } else {
    pollutionCircle = L.circle(center, {
      radius: 2500,
      color,
      fillColor: color,
      fillOpacity: 0.2,
    }).addTo(window.map);
  }
}
