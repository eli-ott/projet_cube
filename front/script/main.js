let formulaire = document.getElementById('formulaire');
let infoContainer = document.getElementById('infos-container');
let info = document.getElementById('infos');
let retour = document.getElementById('retour');

formulaire.addEventListener('submit', async (event) => {
    event.preventDefault();

    console.log(info);
    info.innerHTML = '';

    //récupération du code postal
    let codePostal = document.getElementById('code-postal').value;

    let res, ret;
    //vérifie si l'input est un code postal ou un nom de ville
    if(parseInt(codePostal) > 1) {
        res = await fetch(`http://localhost:5023/citiesinzipcode-${codePostal}`);
        ret = await res.json();
    } else {
        //fetch city name ?
    }

    //la réponse de l'api
    console.log(ret);
    ret.forEach((ville, i) => {
        info.innerHTML += `${ville.toUpperCase()}`;

        if (ret.length > 1 && ret.length - 1 !== i) {
            info.innerHTML += '&nbsp;&mdash;&nbsp;';
        }
    });

    infoContainer.style.display = 'flex';
    formulaire.style.display = 'none';
});

retour.addEventListener('click', () => {
    formulaire.reset();
    infoContainer.style.display = 'none';
    formulaire.style.display = 'flex';
});