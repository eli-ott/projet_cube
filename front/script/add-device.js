function create64BitInteger(ipv4, id) {

    // On converti l'IPV4 sous sa forme de 4 octet
    const binaryIP = BigInt('0b' + ipv4
        .split('.')
        .map(octet => {
            return parseInt(octet)
                .toString(2)
                .padStart(8, '0');
        } // ..
    ).join(''));


    // On combine l'ID avec l'IPV4
    const shiftedId = BigInt(id) << BigInt(32);
    return shiftedId | binaryIP;

} // function ..


// Example usage
const ipv4 = "192.168.1.1";
const id = 12345;
const result = create64BitInteger(ipv4, id);

console.log(result.toString()); // Output as string


/**
 * Adding a new device using the API when the user clicks on 'Envoyer'
 */
const addDevice = async () => {
    const id_appareil = document.getElementById('id_appareil');
    const nom_appareil = document.getElementById('nom_appareil');
    const id_type_mesure = document.getElementById('id_type_mesure');

    const data = {
        "idAppareil": id_appareil.value,
        "nomAppareil": nom_appareil.value,
        "idType": id_type_mesure.valuee
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
    id_appareil.value = '';
    nom_appareil.value = '';
    id_type_mesure.value = '';
}

document.getElementById('submit').addEventListener('click', addDevice);