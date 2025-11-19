// On attend que tout le contenu de la page soit chargé
document.addEventListener('DOMContentLoaded', () => {

    // 1. On récupère les éléments du DOM dont on a besoin
    const burgerMenu = document.getElementById('burger-menu');
    const header = document.getElementById('header');
    const overlay = document.getElementById('overlay');

    // 2. On vérifie que les éléments existent bien avant d'ajouter l'écouteur
    if (burgerMenu && header && overlay) {
        
        // 3. On écoute l'événement 'click' sur le bouton du menu
        burgerMenu.addEventListener('click', () => {
            
            // La méthode .toggle() est parfaite pour ça :
            // S'il a la classe, il l'enlève. Sinon, il l'ajoute.
            
            // Anime le burger en croix (et vice-versa)
            burgerMenu.classList.toggle('active');
            
            // Fait apparaître/disparaître le menu latéral (header)
            header.classList.toggle('visible');
            
            // Fait apparaître/disparaître l'overlay noir
            overlay.classList.toggle('visible');
        });
    }
});