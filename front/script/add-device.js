/**
 * Adding a new device using the API when the user clicks on 'Envoyer'
 */
const addDevice = async () => {
    const id_appareil = document.getElementById('id_appareil').value;
    const ip_appareil = document.getElementById('ip_appareil').value;
    const nom_appareil = document.getElementById('nom_appareil').value;
    const id_type_mesure = document.getElementById('id_type_mesure').value;

    const data = {
        "idAppareil": ip_appareil + '-' + id_appareil,
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
    const ret = await res.json();
    if (ret.success === true) {
        alert('Appareil ajouter avec succès');
    } else {
        alert('Impossible d\'ajouter l\'appareil');
    }
    id_appareil = '';
    ip_appareil = '';
    nom_appareil = '';
    id_type_mesure = '';
}

document.getElementById('submit').addEventListener('click', addDevice);