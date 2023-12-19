/**
 * Deletting a device when the user click on 'supprimer'
 */
const deleteDevice = async () => {
    let id_appareil = document.getElementById('id_appareil').value;

    const res = await fetch('http://localhost:5023/device-' + id_appareil, {
        method: "DELETE"
    });

    if (res.status === 200) {
        alert('Appareil supprimer avec succès');
    } else {
        alert('Impossible de supprimer l\'appareil');
    }
    id_appareil = '';
}

document.getElementById('submit').addEventListener('click', deleteDevice);