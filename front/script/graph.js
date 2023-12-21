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
    // const res = await fetch('http://localhost:5023/measures-1', {
    //     method: "GET"
    // });
    // const ret = await res.json();

    //ajoute une carte avec les données
    const cardContainer = document.getElementsByTagName('main')[0];
    for (let i = 0; i < 2; i++) {
        const card = `
      <section class="data-container">
      <div class="infos">
            <div class="device-info">
            <span class="info-title">Mesure de température</span> <br />
            Créer le 12/12/2020 <br />
            <br />
            Etat: fonctionnel
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

        const canvas = document.querySelectorAll('canvas');
        canvas.forEach(canva => {
            new Chart(canva, {
                type: 'line',
                data: {
                    labels: ['Red', 'Blue', 'Yellow'],
                    datasets: [{
                        label: '# of Votes',
                        data: [-12, 19, 3, -5, 2, 3],
                        borderWidth:3,
                        borderColor: '#000000',
                        pointRadius: 10,
                        pointHoverRadius: 5,
                        backgroundColor: ['#fff585', '#00ff58', '#548452'],
                        borderWidth: 1
                    }]
                },
            });
        })
    }
};
getAll();