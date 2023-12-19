/**
 * Adding a new device using the API when the user clicks on 'Envoyer'
 */
const addMeasure = async () => {
    const nom_mesure = document.getElementById('nom_mesure');
    const unit = document.getElementById('unit');
    const min = document.getElementById('min');
    const max = document.getElementById('max');

    const data = {
        "nomType": nom_mesure.value,
        "uniteMesure": unit.value,
        "limiteMin": min.value,
        "limiteMax": ma.valuex
    }

    const res = await fetch('http://localhost:5023/newmeasuretype', {
        method: "POST",
        headers: {
            "Content-type": "application/json"
        },
        body: JSON.stringify(data)
    });
    const ret = await res.text();
    if (ret.success === true) {
        alert('Appareil ajouter avec succès');
    } else {
        alert('Impossible d\'ajouter l\'appareil');
    }
    id_appareil.value = '';
    nom_appareil.value = '';
    id_type_mesure.value = '';
}

document.getElementById('submit').addEventListener('click', addMeasure);