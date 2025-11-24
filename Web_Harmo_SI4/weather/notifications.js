// js/notifications.js

let stompClient = null;
const activeSubscriptions = new Map(); // topicName -> subscription object

function connectToBroker() {
  // URL à ADAPTER selon la config ActiveMQ STOMP / WebSocket
  // Exemples possibles :
  //   ws://localhost:61614/stomp
  //   ws://localhost:61614
  // Cf. code fourni par le prof.
  const socket = new WebSocket("ws://localhost:61614/stomp");

  stompClient = Stomp.over(socket);
  stompClient.debug = () => {}; // désactive les logs STOMP si tu veux

  stompClient.connect(
    "admin", // user
    "admin", // pass
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
  // éventuellement tenter un reconnect après un timeout
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

    // abonnement initial si déjà coché
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

  // côté ActiveMQ, ton producteur envoie sur "meteo","pollution", etc.
  // côté STOMP / JS, selon config, ce sera souvent "/topic/meteo", "/topic/pollution", ...
  const destination = "/topic/" + topicName;

  const sub = stompClient.subscribe(destination, (msg) => {
    try {
      const payload = JSON.parse(msg.body);
      displayNotification(payload);
    } catch (e) {
      console.error("[NotifFront] Erreur parsing message:", e, msg.body);
    }
  });

  activeSubscriptions.set(topicName, sub);
  console.log("[NotifFront] Abonne au topic:", destination);
}

function unsubscribeTopic(topicName) {
  const sub = activeSubscriptions.get(topicName);
  if (sub) {
    sub.unsubscribe();
    activeSubscriptions.delete(topicName);
    console.log("[NotifFront] Desabonne du topic:", topicName);
  }
}

function displayNotification(payload) {
  const container = document.getElementById("notifications-stream");
  if (!container) return;

  const div = document.createElement("div");
  div.className = "notif notif-" + (payload.severity || "LOW").toLowerCase();

  div.innerHTML = `
    <div class="notif-header">
      <span class="notif-topic">${payload.topic}</span>
      <span class="notif-severity">${payload.severity}</span>
      <span class="notif-time">${payload.timestamp || ""}</span>
    </div>
    <div class="notif-message">${payload.message}</div>
  `;

  container.prepend(div); // dernier message en haut

  // tu peux aussi afficher une icône globale rouge/orange/verte ailleurs dans l'UI
}

// démarrage
document.addEventListener("DOMContentLoaded", () => {
  connectToBroker();
});
