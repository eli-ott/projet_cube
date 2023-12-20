/**
 * Deletting a device when the user click on 'supprimer'
 */
const deleteMeseaure = async () => {
    let id_mesure = document.getElementById('id_mesure').value;

    const res = await fetch('http://localhost:5023/measuretype-' + id_mesure, {
        method: "DELETE"
    });

    if (res.status === 200) {
        alert('Type de mesure supprimer avec succès');
    } else {
        alert('Impossible de supprimer le type de mesure');
    }
    id_appareil = '';
}

document.getElementById('submit').addEventListener('click', deleteMeseaure);