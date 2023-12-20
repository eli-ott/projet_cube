/** Le canvas où doit être le graphique */
const ctx = document.getElementById('graph_1');

/**
 * Permet de créer les graphiques
 */
window.addEventListener('load', () => {
    new Chart(ctx, {
        data: {
            datasets: [{
                type: 'bar',
                label: 'Bar Dataset',
                data: [10, 20, 30, 40]
            }, {
                type: 'line',
                label: 'Line Dataset',
                data: "12,51,85,74".split(','),
            }],
            labels: ['January', 'February', 'March', 'April']
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            maintainAspectRatio: false
        }
    });
});

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

document.getElementById('export').addEventListener('click', exportGraph);

/**
 * Get all the meseaures from the WS
 */
const getAll = async () => {
    const res = await fetch('http://localhost:5023/measures-1', {
        method: "GET"
    });
    const ret = await res.json();
    console.log(ret);
};
getAll();