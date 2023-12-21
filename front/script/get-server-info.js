/** L'addresse IPV4 actuelle. */
let ip = ""

/** Récupère l'addresse IPV4 du serveur. */
const getServerIP = async () => {

    const res_ip = await fetch('http://localhost:5023/serverip', { method: 'GET' });
    const ret_ip = await res_ip.json();

    if (ret_ip.reussite === true) { ip = ret_ip.donnee; }
    else                          { alert('Problème lors de la récupération des appareils'); }
} // const ..

/** Récupère la date actuelle sous le format yyyy-MM-dd HH:mm:ss */
function getCurrentDateTime() {

    const now = new Date();

    const year  =  now.getFullYear();
    const month = (now.getMonth() + 1).toString().padStart(2, '0'); // getMonth() returns 0-11
    const day   =  now.getDate().toString().padStart(2, '0');

    const hours   = now.getHours().toString().padStart(2,   '0');
    const minutes = now.getMinutes().toString().padStart(2, '0');
    const seconds = now.getSeconds().toString().padStart(2, '0');

    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
} // function ..


getServerIP();

setInterval(() => {
    document
        .querySelector('#time')
        .innerHTML = `${getCurrentDateTime()} (${ip})`;
}, 1000);