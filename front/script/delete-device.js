/**
 * Deletting a device when the user click on 'supprimer'
 */
const deleteDevice = async () => {
    let id_appareil = document.getElementById('id_appareil').value;
    let ip_appareil = document.getElementById('ip_appareil').value;

    let id = id_appareil + '-' + ip_appareil;

    const res = await fetch('http://localhost:5023/device-' + id, {
        method: "DELETE"
    });

    if (res.status === 200) {
        alert('Appareil supprimé avec succès');
    } else {
        alert('Impossible de supprimer l\'appareil');
    }
    id_appareil = '';
    ip_appareil = '';
}

document.getElementById('submit').addEventListener('click', deleteDevice);