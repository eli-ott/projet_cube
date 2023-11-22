document.getElementById('formulaire').addEventListener('submit', async (event) => {
    event.preventDefault()

    const res = await fetch('http://localhost:5023/');
    const ret = await res.text();

    console.log(ret);
});