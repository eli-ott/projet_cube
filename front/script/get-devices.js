/**
 * Get all the devices
 */
const getDevices = async () => {
    const res_devices = await fetch('http://localhost:5023/devices', {
        method: 'GET'
    });
    const ret_devices = await res_devices.json();

    if (ret_devices.reussite === true) {
        showDevices(ret_devices.donnee);
    } else {
        alert('Problème lors de la récupération des appareils');
    }

    const res_measures = await fetch('http://localhost:5023/measuretypes', {
        method: 'GET'
    });
    const ret_measures = await res_measures.json();

    if (ret_measures.reussite === true) {
        showMeasures(ret_measures.donnee);
    } else {
        alert('Problème lors de la récupération des types de mesures');
    }
}
getDevices();

/**
 * Show all the devices
 */
const showDevices = (data) => {
    data.forEach(device => {
        const card = `
        <div class="carte appareil">
            <div><u>Nom appareil</u> : ${device.idAppareil}</div>
            <br />
            <div><u>ID de l'appareil</u> : ${device.nomAppareil}</div>
            <br />
            <div><u>ID du type lié</u> : ${device.idType}</div>
            <br />
            <div><u>Appareil actif</u> : ${device.activation}</div>
            <br />
        </div>
        `;

        document.getElementsByClassName('appareils-container')[0].innerHTML += card;
        document.getElementsByClassName('appareils-default')[0].style.display = 'none';
    });
}

/**
 * Show all the measures types to the user
 */
const showMeasures = (data) => {
    data.forEach(measure => {
        const card = `
        <div class="carte mesure">
        <div><u>ID de la mesure</u> : ${measure.idType}</div>
        <br />
        <div><u>Nom du type de la mesure</u> : ${measure.nomType}</div>
        <br />
        <div><u>Unité de la mesure</u> : ${measure.uniteMesure}</div>
        <br />
        <div><u>Limite minimale de la mesure</u> : ${measure.limiteMin}</div>
        <br />
        <div><u>Limite maximale de la mesure</u> : ${measure.limiteMax}</div>
        <br />
        </div>
        `;

        document.querySelector('.mesures-container').innerHTML += card;
        document.getElementsByClassName('mesures-default')[0].style.display = 'none';
    });
}