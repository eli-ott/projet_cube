/**
 * Adding a new device using the API when the user clicks on 'Envoyer'
 */
const addMeasure = async () => {
    const nom_mesure = document.getElementById('nom_mesure').value;
    const unit = document.getElementById('unit').value;
    const min = document.getElementById('min').value;
    const max = document.getElementById('max').value;

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
    const ret = await res.text();
}

document.getElementById('submit').addEventListener('click', addMeasure);