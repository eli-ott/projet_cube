document.getElementById('formulaire').addEventListener('submit', async (event) => {
    event.preventDefault()

    const res = await fetch('http://localhost:5023/');
    console.log(await res.text());
});