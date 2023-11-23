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

    //fetch l'api en fonction du code postal
    const res = await fetch(`http://localhost:5023/${codePostal}`);
    const ret = await res.json();

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