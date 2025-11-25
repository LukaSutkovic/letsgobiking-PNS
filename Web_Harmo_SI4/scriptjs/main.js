document.addEventListener("DOMContentLoaded", () => {
  const startPointComponent = document.querySelector("#start-point");
  const endPointComponent = document.querySelector("#end-point");
  const infoMessage = document.querySelector("#info-message");

  let itinerary = {
    start: null,
    end: null,
  };

  startPointComponent.addEventListener("addressSelected", (event) => {
    console.log("Départ sélectionné :", event.detail.address);
    itinerary.start = event.detail.address;
    checkAndRedirect();
  });

  endPointComponent.addEventListener("addressSelected", (event) => {
    console.log("Arrivée sélectionnée :", event.detail.address);
    itinerary.end = event.detail.address;
    checkAndRedirect();
  });

  function checkAndRedirect() {
    if (itinerary.start && itinerary.end) {
      infoMessage.textContent = "Itinéraire complet ! Redirection en cours...";
      infoMessage.style.color = "green";

      // --- NOUVEAU : Sauvegarde des préférences de notifications ---
      const notifPrefs = {};
      // On cherche toutes les cases à cocher sur la page d'accueil
      document.querySelectorAll(".notif-topic").forEach((checkbox) => {
        // On enregistre l'état (true/false) pour chaque topic
        notifPrefs[checkbox.dataset.topic] = checkbox.checked;
      });

      // On stocke dans le navigateur
      localStorage.setItem("notificationPrefs", JSON.stringify(notifPrefs));
      // -------------------------------------------------------------

      localStorage.setItem("itineraryData", JSON.stringify(itinerary));

      setTimeout(() => {
        window.location.href = "itineraire/itineraire.html";
      }, 1000);
    }
  }
});
