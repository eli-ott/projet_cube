/**
 * Adding a new device using the API when the user clicks on 'Envoyer'
 */
const addDevice = async () => {
    const id_appareil = document.getElementById('id_appareil').value;
    const nom_appareil = document.getElementById('nom_appareil').value;
    const id_type_mesure = document.getElementById('id_type_mesure').value;

    const data = {
        "idAppareil": id_appareil,
        "nomAppareil": nom_appareil,
        "idType": id_type_mesure
    }

    const res = await fetch('http://localhost:5023/device', {
        method: "PUT",
        headers: {
            "Content-type": "application/json"
        },
        body: JSON.stringify(data)
    });
    const ret = await res.text();
}

document.getElementById('submit').addEventListener('click', addDevice);