let date = new Date();
let ip   = ""

/**
 * Récupère l'addresse IPV4 du serveur
 */
const getServerIP = async () => {
    const res_ip = await fetch('http://localhost:5023/serverip', {
        method: 'GET'
    });
    const ret_ip = await res_ip.json();

    if (ret_ip.reussite === true) {
        ip = ret_ip.donnee;
    } else {
        alert('Problème lors de la récupération des appareils');
    }
}

getServerIP();

setInterval(() => {
    date = new Date();
    
    let showDate = `${date.toLocaleDateString()} ${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`;
    
    document.querySelector('#time').innerHTML = `${showDate} (${ip})`;
}, 1000);