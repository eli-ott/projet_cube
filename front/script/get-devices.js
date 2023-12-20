/**
 * Get all the devices
 */
const getDevices = async () => {
    const res = await fetch('http://localhost:5023/devices', {
        method: 'GET'
    });
    const ret = await res.json();

    if (ret.reussite === true) {
        showDevices(ret.donnee);
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
    });
}