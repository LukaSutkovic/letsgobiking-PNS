// weather/notifications.js

let stompClient = null;
const activeSubscriptions = new Map(); // topicName -> subscription object

// on garde ça si tu veux encore afficher les notifs sous forme de liste
// dans #notifications-stream

function connectToBroker() {
  const socket = new WebSocket("ws://localhost:61614/stomp", "stomp");

  stompClient = Stomp.over(socket);
  stompClient.debug = () => {};
  stompClient.connect(
    "admin",
    "admin",
    onConnected,
    onError
  );
}


function onConnected() {
  console.log("[NotifFront] Connecté au broker STOMP");
  setupTopicCheckboxes();
}

function onError(error) {
  console.error("[NotifFront] Erreur STOMP:", error);
}

function setupTopicCheckboxes() {
  const checkboxes = document.querySelectorAll(".notif-topic");
  checkboxes.forEach(cb => {
    cb.addEventListener("change", () => {
      const topicName = cb.dataset.topic;
      if (cb.checked) {
        subscribeTopic(topicName);
      } else {
        unsubscribeTopic(topicName);
      }
    });

    if (cb.checked) {
      subscribeTopic(cb.dataset.topic);
    }
  });
}

function subscribeTopic(topicName) {
  if (!stompClient || !stompClient.connected) {
    console.warn("[NotifFront] STOMP non connecté, impossible de s'abonner pour l'instant.");
    return;
  }

  if (activeSubscriptions.has(topicName)) {
    return; // déjà abonné
  }

  const destination = "/topic/" + topicName;

  const sub = stompClient.subscribe(destination, (msg) => {
    try {
      const payload = JSON.parse(msg.body);
      displayNotification(payload);
      updateMapOverlay(payload);
    } catch (e) {
      console.error("[NotifFront] Erreur parsing message:", e, msg.body);
    }
  });

  activeSubscriptions.set(topicName, sub);
  console.log("[NotifFront] Abonné au topic:", destination);
}

function unsubscribeTopic(topicName) {
  const sub = activeSubscriptions.get(topicName);
  if (sub) {
    sub.unsubscribe();
    activeSubscriptions.delete(topicName);
    console.log("[NotifFront] Désabonné du topic:", topicName);
  }
}

function displayNotification(payload) {
  const container = document.getElementById("notifications-stream");
  if (!container) return;

  const severity = (payload.severity || "LOW").toUpperCase();

  const div = document.createElement("div");
  div.className = "notif notif-" + severity.toLowerCase();

  div.innerHTML = `
    <div class="notif-header">
      <span class="notif-topic">${payload.topic}</span>
      <span class="notif-severity">${severity}</span>
      <span class="notif-time">${payload.timestamp || ""}</span>
    </div>
    <div class="notif-message">${payload.message}</div>
  `;

  container.prepend(div);
}

/* ============================
   OVERLAYS LEAFLET SUR LA MAP
   ============================ */

let meteoMarker = null;
let airQualityCircle = null;
let pollutionCircle = null;

function updateMapOverlay(payload) {
  if (!window.map || typeof L === "undefined") {
    // pas sur la page avec carte
    return;
  }

  const topic = payload.topic;
  const severity = (payload.severity || "LOW").toUpperCase();
  const center = window.map.getCenter();

  if (topic === "meteo") {
    updateMeteoMarker(center, severity);
  } else if (topic === "airquality") {
    updateAirQualityCircle(center, severity);
  } else if (topic === "pollution") {
    updatePollutionCircle(center, severity);
  }
}

// météo → icône soleil / nuage / pluie
function updateMeteoMarker(center, severity) {
  let iconChar;
  switch (severity) {
    case "HIGH":
      iconChar = "🌧"; // grosse pluie
      break;
    case "MEDIUM":
      iconChar = "⛅"; // nuageux
      break;
    default:
      iconChar = "☀️"; // soleil
      break;
  }

  const icon = L.divIcon({
    className: "meteo-icon",
    html: `<span style="font-size: 24px;">${iconChar}</span>`,
    iconSize: [30, 30],
    iconAnchor: [15, 15]
  });

  if (meteoMarker) {
    meteoMarker.setLatLng(center);
    meteoMarker.setIcon(icon);
  } else {
    meteoMarker = L.marker(center, { icon }).addTo(window.map);
  }
}

// qualité de l'air → cercle coloré vert / orange / rouge
function updateAirQualityCircle(center, severity) {
  let color;
  switch (severity) {
    case "HIGH":
      color = "#c0392b"; // rouge
      break;
    case "MEDIUM":
      color = "#e67e22"; // orange
      break;
    default:
      color = "#27ae60"; // vert
      break;
  }

  const radius = 1500; // en mètres, adapte si tu veux

  if (airQualityCircle) {
    airQualityCircle.setLatLng(center);
    airQualityCircle.setStyle({
      color,
      fillColor: color
    });
    airQualityCircle.setRadius(radius);
  } else {
    airQualityCircle = L.circle(center, {
      radius,
      color,
      fillColor: color,
      fillOpacity: 0.25
    }).addTo(window.map);
  }
}

// pollution → autre cercle autour (plus grand)
function updatePollutionCircle(center, severity) {
  let color;
  switch (severity) {
    case "HIGH":
      color = "#8e44ad"; // violet foncé
      break;
    case "MEDIUM":
      color = "#9b59b6";
      break;
    default:
      color = "#bdc3c7";
      break;
  }

  const radius = 2500;

  if (pollutionCircle) {
    pollutionCircle.setLatLng(center);
    pollutionCircle.setStyle({
      color,
      fillColor: color
    });
    pollutionCircle.setRadius(radius);
  } else {
    pollutionCircle = L.circle(center, {
      radius,
      color,
      fillColor: color,
      fillOpacity: 0.2
    }).addTo(window.map);
  }
}

// démarrage
document.addEventListener("DOMContentLoaded", () => {
  connectToBroker();
});
