/**
 * Adding a new device using the API when the user clicks on 'Envoyer'
 */
const addDevice = async () => {
    const id_appareil = document.getElementById('id_appareil');
    const nom_appareil = document.getElementById('nom_appareil');
    const id_type_mesure = document.getElementById('id_type_mesure');

    const data = {
        "idAppareil": id_appareil.value,
        "nomAppareil": nom_appareil.value,
        "idType": id_type_mesure.valuee
    }

    const res = await fetch('http://localhost:5023/device', {
        method: "PUT",
        headers: {
            "Content-type": "application/json"
        },
        body: JSON.stringify(data)
    });
    const ret = await res.json();
    if (ret.success === true) {
        alert('Appareil ajouter avec succès');
    } else {
        alert('Impossible d\'ajouter l\'appareil');
    }
    id_appareil.value = '';
    nom_appareil.value = '';
    id_type_mesure.value = '';
}

document.getElementById('submit').addEventListener('click', addDevice);