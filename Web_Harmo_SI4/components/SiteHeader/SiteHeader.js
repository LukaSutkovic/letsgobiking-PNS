class SiteHeader extends HTMLElement {
    constructor() {
        super();
        // Il ne fait qu'appeler une autre fonction pour charger le template.
        this.loadAndAttachTemplate();
    }

    async loadAndAttachTemplate() {
        const responseCSS = await fetch('../components/SiteHeader/SiteHeader.css');
        const responseHTML = await fetch('../components/SiteHeader/SiteHeader.html');

        // 2. On lit le contenu de ces réponses en tant que texte.
        // .text() aussi renvoie une promesse, donc on utilise 'await' à nouveau.
        const styleText = await responseCSS.text();
        const htmlText = await responseHTML.text();

        // 3. On crée un élément <template> dynamiquement.
        const template = document.createElement('template');
        template.innerHTML = `
            <style>
                ${styleText}
            </style>
            ${htmlText}
        `;

        let shadowRoot = this.attachShadow({ mode: 'open' });
        shadowRoot.appendChild(template.content.cloneNode(true));

        this.addEventListeners(shadowRoot);
    }

    addEventListeners(shadowRoot) {
        const burgerMenu = shadowRoot.querySelector('.burger-menu');
        const headerElement = shadowRoot.querySelector('header');
        const overlay = document.querySelector('.overlay');

        if (burgerMenu && headerElement && overlay) {
            burgerMenu.addEventListener('click', () => {
                burgerMenu.classList.toggle('active');
                headerElement.classList.toggle('visible');
                overlay.classList.toggle('visible');
            });
        }
    }
}

customElements.define('site-header', SiteHeader);