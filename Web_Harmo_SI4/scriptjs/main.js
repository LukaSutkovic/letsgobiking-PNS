document.addEventListener('DOMContentLoaded', () => {
    const startPointComponent = document.querySelector('#start-point');
    const endPointComponent = document.querySelector('#end-point');
    const infoMessage = document.querySelector('#info-message');

    // Un objet pour garder en mémoire les adresses choisies
    let itinerary = {
        start: null,
        end: null
    };

    // On écoute l'événement "addressSelected" sur le composant de départ
    startPointComponent.addEventListener('addressSelected', (event) => {
        console.log("Départ sélectionné :", event.detail.address);
        itinerary.start = event.detail.address;
        checkAndRedirect();
    });

    // On fait la même chose pour le composant d'arrivée
    endPointComponent.addEventListener('addressSelected', (event) => {
        console.log("Arrivée sélectionnée :", event.detail.address);
        itinerary.end = event.detail.address;
        checkAndRedirect();
    });

    function checkAndRedirect() {
        // On vérifie si les DEUX adresses ont été sélectionnées
        if (itinerary.start && itinerary.end) {
            infoMessage.textContent = "Itinéraire complet ! Redirection en cours...";
            infoMessage.style.color = "green";

            // --- Sauvegarde dans localStorage ---
            // On transforme notre objet en texte (JSON) pour le stocker.
            localStorage.setItem('itineraryData', JSON.stringify(itinerary));

            // On attend 1 seconde avant de rediriger, pour que l'utilisateur voie le message.
            setTimeout(() => {
                window.location.href = 'itineraire/itineraire.html';
            }, 1000);
        }
    }
});