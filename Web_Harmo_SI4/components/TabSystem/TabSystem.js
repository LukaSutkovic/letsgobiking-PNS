class TabSystem extends HTMLElement {
    constructor() {
        super();
        this.loadAndAttachTemplate();
    }

    async loadAndAttachTemplate() {
        const responseCSS = await fetch('/Web_Harmo_SI4/components/TabSystem/TabSystem.css');
        const responseHTML = await fetch('/Web_Harmo_SI4/components/TabSystem/TabSystem.html');
        const styleText = await responseCSS.text();
        const htmlText = await responseHTML.text();

        const template = document.createElement('template');
        template.innerHTML = `<style>${styleText}</style>${htmlText}`;

        let shadowRoot = this.attachShadow({ mode: 'open' });
        shadowRoot.appendChild(template.content.cloneNode(true));
        
        // On attend que les slots soient remplis pour lancer la logique
        setTimeout(() => this.addEventListeners(shadowRoot), 0);
    }

    addEventListeners(shadowRoot) {
        const tabSlot = shadowRoot.querySelector('slot[name="tab"]');
        const panelSlot = shadowRoot.querySelector('slot[name="panel"]');

        // .assignedElements() récupère les vrais éléments HTML passés dans le slot
        const tabs = tabSlot.assignedElements();
        const panels = panelSlot.assignedElements();

        // On active le premier onglet par défaut
        if(tabs.length > 0) tabs[0].classList.add('active');
        if(panels.length > 0) panels[0].classList.add('active');

        tabs.forEach(tab => {
            tab.addEventListener('click', () => {
                // D'abord, on désactive tout
                tabs.forEach(t => t.classList.remove('active'));
                panels.forEach(p => p.classList.remove('active'));

                // Ensuite, on active l'onglet cliqué
                tab.classList.add('active');

                // Et on trouve le panneau correspondant pour l'activer aussi
                const panelId = tab.getAttribute('data-panel');
                const panelToShow = panels.find(p => p.id === panelId);
                if(panelToShow) {
                    panelToShow.classList.add('active');
                }
            });
        });
    }
}

customElements.define('tab-system', TabSystem);