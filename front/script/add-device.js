document.getElementById('submit').addEventListener('click', async () => {
    const id_appareil = document.getElementById('id_appareil');
    const nom_appareil = document.getElementById('nom_appareil');
    const id_type_mesure = document.getElementById('id_type_mesure');

    const res = await fetch('http://localhost:5023/newdevice', {
        method: "POST",
        body: {
            "idAppareil": id_appareil,
            "nomAppareil": nom_appareil,
            "idType": id_type_mesure
        }
    });
    const ret = await res.json();
    console.log(ret);
});