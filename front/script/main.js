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
                console.log(ville, ret);

                info.innerHTML += `${ville.zipCode.toUpperCase()} ${ville.cityName.toUpperCase()}`;

                if (ret.length > 1 && ret.length - 1 !== i) {
                    info.innerHTML += '&nbsp;&mdash;&nbsp;';
                }
            });
            break;
        case defaultPath + 'getCode.html':
            ret = await getCode();
            console.log(ret);

            //affiche la réponse API
            ret.forEach((code, i) => {
                info.innerHTML += `${code}`;

                if (ret.length > 1 && ret.length - 1 !== i) {
                    info.innerHTML += '&nbsp;&mdash;&nbsp;';
                }
            });
            break;
        case defaultPath + 'getPos.html':
            ret = await getPos();

            //affiche la réponse API
            info.innerHTML += `lat:&nbsp;${ret.lat}&nbsp;-&nbsp;&nbsp;long:&nbsp;${ret.lng}`;
            break;
        case defaultPath + 'getDistance.html':
            ret = await getDistance();

            info.innerHTML = `distance:&nbsp;${Math.round(ret.dist)}`;
            break;
        case defaultPath + 'getCitiesInRadius.html':
            ret = await getCityInRadius();

            //affiche la réponse API
            ret.forEach((ville, i) => {
                console.log(ville, ret);
                info.innerHTML += `${ville.zipCode.toUpperCase()} ${ville.cityName.toUpperCase()}`;

                if (ret.length > 1 && ret.length - 1 !== i) {
                    info.innerHTML += '&nbsp;&mdash;&nbsp;';
                }
                if (i % 5 === 0) {
                    info.innerHTML += '<br/><br/>';
                }
            });
            break;
        case defaultPath + 'addCity.html':
            ret = await addVille();

            window.location.reload();
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
    let codePostal = document.getElementById('code-postal').value.trim();

    let res = await fetch(`${API_PATH}citiesinzipcode-${codePostal}`);
    return await res.json();
}
const getCode = async () => {
    let name = document.getElementById('slug').value.toLowerCase().trim();

    let res = await fetch(`${API_PATH}zipcodesfromcity-${slugify(name)}`);
    return await res.json()
}
const getPos = async () => {
    let codePostal = document.getElementById('code-postal').value.trim();
    let slug = document.getElementById('slug').value.toLowerCase().trim();

    let res = await fetch(`${API_PATH}citypos-${codePostal + slugify(slug)}`)
    return await res.json();
}
const getDistance = async () => {
    let codePostal1 = document.getElementById('code-postal1').value.trim();
    let slug1 = document.getElementById('slug1').value.toLowerCase().trim();
    let codePostal2 = document.getElementById('code-postal2').value.trim();
    let slug2 = document.getElementById('slug2').value.toLowerCase().trim();

    let res = await fetch(`${API_PATH}citydist-${codePostal1 + slugify(slug1)}-${codePostal2 + slugify(slug2)}`);
    return await res.json();
}
const getCityInRadius = async () => {
    let codePostal = document.getElementById('code-postal').value.trim();
    let slug = document.getElementById('slug').value.toLowerCase().trim();
    let radius = document.getElementById('radius').value.toLowerCase().trim();

    let res = await fetch(`${API_PATH}citiesinradius-${codePostal + slugify(slug)}-${radius}`)
    return await res.json();
}
const addVille = async () => {
    let name = document.getElementById('nom').value;
    let code = document.getElementById('code-postal').value;
    let lat = document.getElementById('lat').value;
    let lng = document.getElementById('lng').value;

    const data = {
        cityName: name | null,
        zipCode: code | null,
        gpsLat: lat | null,
        gpsLng: lng | null
    };

    let res = await fetch(`${API_PATH}newcity`, {
        method: 'POST'
    });
    console.log(res.text());
}