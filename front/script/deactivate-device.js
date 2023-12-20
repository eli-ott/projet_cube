/**
 * Deactivating or activating a device using the API when the user clicks on 'Envoyer'
 */
const deactivateDevice = async () => {
    let id_appareil = document.getElementById('id_appareil').value;
    let ip_appareil = document.getElementById('ip_appareil').value;
    let activation = document.getElementById('activation').checked;

    const data = {
        "idAppareil": ip_appareil + '-' + id_appareil,
        'activation': activation
    }

    const res = await fetch('http://localhost:5023/device', {
        method: "PUT",
        headers: {
            "Content-type": "application/json"
        },
        body: JSON.stringify(data)
    });
    const ret = await res.json();
    if (ret.reussite === true) {
        alert('Appareil désactivé avec succès');
    } else {
        alert('Impossible de désactiver l\'appareil');
    }
    id_appareil = '';
    ip_appareil = '';
}

document.getElementById('submit').addEventListener('click', deactivateDevice);