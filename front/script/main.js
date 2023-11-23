document.getElementById('formulaire').addEventListener('submit', async (event) => {
    event.preventDefault();

    //récupération du code postal
    let codePostal = document.getElementById('code-postal').value;

    //fetch l'api en fonction du code postal
    const res = await fetch(`http://localhost:5023/${codePostal}`);
    const ret = await res.text();

    //la réponse de l'api
    console.log(ret);
});