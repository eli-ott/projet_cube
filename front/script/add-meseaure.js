/**
 * Adding a new measure using the API when the user clicks on 'Envoyer'
 */
const addMeasure = async () => {
    let nom_mesure = document.getElementById('nom_mesure').value;
    let unit = document.getElementById('unit').value;
    let min = document.getElementById('min').value;
    let max = document.getElementById('max').value;

    const data = {
        "nomType": nom_mesure,
        "uniteMesure": unit,
        "limiteMin": min,
        "limiteMax": max
    }

    const res = await fetch('http://localhost:5023/newmeasuretype', {
        method: "POST",
        headers: {
            "Content-type": "application/json"
        },
        body: JSON.stringify(data)
    });
    const ret = await res.json();
    console.log(ret);
    if (ret.reussite === true) {
        alert('Type de mesure ajouté avec succès');
    } else {
        alert('Impossible d\'ajouter le type de mesure');
    }
    nom_mesure = '';
    unit = '';
    min = '';
    max = '';
}

document.getElementById('submit').addEventListener('click', addMeasure);