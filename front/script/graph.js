/**
 * Permet d'exporter le graph en .jpg
 */
const exportGraph = (event) => {
    //récupère le graphique à exporter
    const graphId = event.target.dataset.graph;
    const canvasToExport = document.getElementById('graph_' + graphId);

    //transforme le graphique en URI
    const canvasUrl = canvasToExport.toDataURL('image/jpg');
    //permet de télécharger le graphique
    const a = document.createElement('a');
    a.href = canvasUrl;
    a.download = "chart.jpg";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}

/**
 * Get all the meseaures from the WS
 */
const getAll = async () => {
    const cardContainer = document.getElementsByTagName('main')[0];
    cardContainer.innerHTML = '';

    let dateDebut = formatedDateAndTime(document.getElementById('dateDebut').value);
    let dateFin = formatedDateAndTime(document.getElementById('dateFin').value);
    const res_all = await fetch('http://localhost:5023/measures/' + dateDebut + '/' + dateFin, {
        method: 'GET'
    });
    const ret_all = await res_all.json();
    console.log(ret_all.donnee);

    Object.keys(ret_all.donnee).forEach((elem, i) => {
        let mesures = [];
        let instants = [];
        let data = [];
        console.log(ret_all.donnee[elem], ret_all.donnee[elem][key2]);
        Object.keys(ret_all.donnee[elem]).forEach(key2 => {
            data.push(ret_all.donnee[elem][key2]);
        });

        data.forEach(donnees => {
            mesures = [...donnees.valeur.split(',')];
            instants = [...donnees.instant.split(',')];
        });
        //ajoute une carte avec les données
        const card = `
        <section class="data-container">
        <div class="infos">
                <div class="device-info">
                <span class="info-title">Mesure de température</span> <br />
                </div>
                <div class="value-info">
                <span class="info-title">Dernière valeur</span> <br />
                12°C mesuré le: 12/12/2023 à 5h00 <br />
                <br />
                <button data-graph="${i + 1}" id="export" class="export_buttons">Exporter le graphique</button>
                </div>
                </div>
                <div class="graph-container" id="container_${i + 1}">
                    <canvas id="graph_${i + 1}"></canvas>
                </div>
        </section>
      `;
        cardContainer.innerHTML += card;

        const buttons = document.querySelectorAll('.export_buttons');
        buttons.forEach(button => {
            button.addEventListener('click', exportGraph);
        });

        setTimeout(() => {
            const canvas = document.getElementById(`graph_${i + 1}`);

            new Chart(canvas, {
                type: 'line',
                data: {
                    labels: instants,
                    datasets: [{
                        label: '# of Votes',
                        data: mesures,
                        borderWidth: 1
                    }]
                },
            });
        });
    });
};
getAll();
setInterval(() => {
    getAll();
}, 30000);

function formatedDateAndTime(date) {
    // Créer un objet Date à partir de la valeur de l'input
    const userDate = new Date(date);
    console.log(date)

    // Formater la date et l'heure
    const year = userDate.getFullYear();
    const month = String(userDate.getMonth() + 1).padStart(2, '0');
    const day = String(userDate.getDate()).padStart(2, '0');
    const hours = String(userDate.getHours()).padStart(2, '0');
    const minutes = String(userDate.getMinutes()).padStart(2, '0');
    const seconds = String(userDate.getSeconds()).padStart(2, '0');

    const dateFormatted = `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;

    return dateFormatted;
}