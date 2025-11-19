class SiteFooter extends HTMLElement {
  constructor() {
    super();

    let footerTemplate = `
      <template>
          <style>
              footer {
                  height: 50px;
                  display: flex;
                  justify-content: center;
                  align-items: center;
                  border-top: 2px solid #e0e0e0;
                  background-color: #f9f9f9;
                  font-family: sans-serif;
                  width: 100%;
              }
              p {
                  margin: 0;
                  color: #333;
              }
          </style>
          <footer>
              <p>Luka SUTKOVIC & Alban MAVEYRAUD - LET'S GO BIKING! - MwSoc SI4</p>
          </footer>
      </template>
    `;

    const parsedFooterTemplate = new DOMParser()
      .parseFromString(footerTemplate, "text/html")
      .querySelector("template").content;

    let shadowRoot = this.attachShadow({ mode: "open" });
    shadowRoot.appendChild(parsedFooterTemplate.cloneNode(true));
  }
}

customElements.define("site-footer", SiteFooter);
