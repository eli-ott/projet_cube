let formulaire = document.getElementById('formulaire');
let infoContainer = document.getElementById('infos-container');
let info = document.getElementById('infos');
let retour = document.getElementById('retour');

formulaire.addEventListener('submit', async (event) => {
    event.preventDefault();

    info.innerHTML = '';

    //récupération du code postal
    let codePostal = document.getElementById('code-postal').value;

    let ret;
    


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

const getVille = async () => {
    let res = await fetch(`http://localhost:5023/citiesinzipcode-${codePostal}`);
    let ret = await res.json();

    return ret;
}