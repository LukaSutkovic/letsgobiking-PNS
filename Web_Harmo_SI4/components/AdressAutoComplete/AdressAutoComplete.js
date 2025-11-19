class AddressAutocomplete extends HTMLElement {
    constructor() {
        super();
        this.debounceTimer = null; // Variable pour notre debounce
        this.loadAndAttachTemplate();
    }

    async loadAndAttachTemplate() {
        const responseCSS = await fetch('/Web_Harmo_SI4/components/AdressAutoComplete/AdressAutoComplete.css');
        const responseHTML = await fetch('/Web_Harmo_SI4/components/AdressAutoComplete/AdressAutoComplete.html');
        const styleText = await responseCSS.text();
        const htmlText = await responseHTML.text();

        const template = document.createElement('template');
        template.innerHTML = `<style>${styleText}</style>${htmlText}`;

        let shadowRoot = this.attachShadow({ mode: 'open' });
        shadowRoot.appendChild(template.content.cloneNode(true));

        this.addEventListeners(shadowRoot);
    }

    addEventListeners(shadowRoot) {
        const input = shadowRoot.querySelector('input');

        input.addEventListener('input', () => {
            clearTimeout(this.debounceTimer);

            this.debounceTimer = setTimeout(() => {
                if (input.value.length > 2) {
                    this.fetchAddresses(input.value, shadowRoot);
                } else {
                    this.clearSuggestions(shadowRoot);
                }
            }, 500);
        });
    }

    async fetchAddresses(query, shadowRoot) {
        // On encode la query pour gérer les espaces et caractères spéciaux
        const encodedQuery = encodeURIComponent(query);
        const url = `https://api-adresse.data.gouv.fr/search/?q=${encodedQuery}&limit=5`;

        try {
            const response = await fetch(url);
            const data = await response.json();
            this.displaySuggestions(data.features, shadowRoot);
        } catch (error) {
            console.error("Erreur lors de l'appel à l'API d'adresses:", error);
        }
    }

    displaySuggestions(features, shadowRoot) {
        const suggestionsList = shadowRoot.querySelector('.suggestions-list');
        this.clearSuggestions(shadowRoot);

        features.forEach(feature => {
            const li = document.createElement('li');
            li.textContent = feature.properties.label; // Le nom de l'adresse à afficher

            // On ajoute un écouteur de clic sur chaque suggestion
            li.addEventListener('click', () => {
                // On met à jour le texte dans le champ de recherche
                shadowRoot.querySelector('input').value = feature.properties.label;
                this.clearSuggestions(shadowRoot);

                // --- La Communication avec la Page Parente ---
                // On crée et on envoie un événement personnalisé pour dire "Une adresse a été choisie !"
                // On y attache l'adresse complète dans la propriété "detail".
                this.dispatchEvent(new CustomEvent('addressSelected', {
                    detail: { 
                        address: feature 
                    }
                }));
            });

            suggestionsList.appendChild(li);
        });
    }

    clearSuggestions(shadowRoot) {
        shadowRoot.querySelector('.suggestions-list').innerHTML = '';
    }
}

customElements.define('address-autocomplete', AddressAutocomplete);