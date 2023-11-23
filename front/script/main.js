let formulaire = document.getElementById('formulaire');
let infoContainer = document.getElementById('infos-container');
let info = document.getElementById('infos');
let retour = document.getElementById('retour');

const API_PATH = "http://localhost:5023/";

formulaire.addEventListener('submit', async (event) => {
    event.preventDefault();

    info.innerHTML = '';

    let ret;

    let defaultPath = '/front/pages/'
    let currentPath = window.location.pathname;

    switch (currentPath) {
        case defaultPath + 'getVille.html':
            ret = await getVille();

            //affiche la réponse API
            ret.forEach((ville, i) => {
                info.innerHTML += `${ville.toUpperCase()}`;

                if (ret.length > 1 && ret.length - 1 !== i) {
                    info.innerHTML += '&nbsp;&mdash;&nbsp;';
                }
            });
            break;
        case defaultPath + 'getPos.html':
            ret = await getPos();
            console.log(ret);

            //affiche la réponse API
            info.innerHTML += `lat:&nbsp;${ret.lat}&nbsp;-&nbsp;&nbsp;long:&nbsp;${ret.lng}`;
            break;
    }

    //la réponse de l'api

    infoContainer.style.display = 'flex';
    formulaire.style.display = 'none';
});

retour.addEventListener('click', () => {
    formulaire.reset();
    infoContainer.style.display = 'none';
    formulaire.style.display = 'flex';
});

const slugify = (city) => {
    city.replace(/[-']/gm, ' ');

    return city.toLowerCase();
}

const getVille = async () => {
    //récupération du code postal
    let codePostal = document.getElementById('code-postal').value;

    let res = await fetch(`${API_PATH}citiesinzipcode-${codePostal}`);
    return await res.json();
}
const getPos = async () => {
    let codePostal = document.getElementById('code-postal').value.trim();
    let slug = document.getElementById('slug').value.toLowerCase().trim();

    let res = await fetch(`${API_PATH}citypos-${codePostal + slug}`)
    return await res.json();
}