const setDefaultDates = () => {
    const today = new Date();
    const past = new Date();
    past.setDate(past.getDate() - 5);

    document.getElementById('dateDebut').value = formatedDateAndTime(past);
    document.getElementById('dateFin').value = formatedDateAndTime(today);
}
setDefaultDates();

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

function colorLerpTwo(color1, color2, t) {
    return {
        r: color1.r + (color2.r - color1.r) * t,
        g: color1.g + (color2.g - color1.g) * t,
        b: color1.b + (color2.b - color1.b) * t
    }; // ..
} // function ..


function colorLerpThree(color1, color2, color3, t) {
    return colorLerpTwo(colorLerpTwo(color1, color2, t), colorLerpTwo(color2, color3, t), t);
} // function ..

function normalize(min, max, x) {
    return (x - min) / (max - min)
} // function ..

const BLUE  = { r: 0, g: 0, b: 255 }; // Blue
const GREEN = { r: 0, g: 255, b: 0 }; // Green
const RED   = { r: 255, g: 0, b: 0 }; // Red

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
    if (Object.keys(ret_all.donnee).length <= 0) cardContainer.innerHTML = 'Il n\'y a aucun élément à afficher à ces dates'
    Object.keys(ret_all.donnee).forEach((elem, i) => {
        let mesures = [];
        let instants = [];
        let data = [];
        let appareilIndexes = [];
        let nomsTypes;
        let unitesMesures;
        Object.keys(ret_all.donnee[elem]).forEach((key2) => {
            appareilIndexes.push(key2);
            data.push(ret_all.donnee[elem][key2]);
        });

        //ajoute une carte avec les données
        const card = `
        <section class="data-container">
                <button data-graph="${i + 1}" id="export" class="export_buttons" onclick="exportGraph()"><img src="../../assets/share.svg" alt="share icon"></button>
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

        let datasets = [];
        data.forEach((donnees, index) => {
            mesures = [...donnees.valeur.split(',')];
            instants = [...donnees.instant.split(',')];
            nomsTypes = donnees.nomType;
            unitesMesures = donnees.uniteMesure;

            const dataset = {
                label: appareilIndexes[index],
                data: mesures,
                pointRadius: 6,
                backgroundColor: mesures.map(x => {
                    let color = colorLerpThree(BLUE, GREEN, RED, normalize(donnees.limiteMin, donnees.limiteMax, x));
                    return `rgb(${Math.round(color.r)}, ${Math.round(color.g)}, ${Math.round(color.b)})`;
                }),
                borderWidth: 4,
                borderColor: `rgba(${Math.round(Math.random() * 180)}, ${Math.round(Math.random() * 180)}, ${Math.round(Math.random() * 180)}, 0.5)`

            }

            datasets.push(dataset);
        });

        const dataGraph = {
            labels: instants,
            datasets
        }

        setTimeout(() => {
            const canvas = document.getElementById(`graph_${i + 1}`);

            new Chart(canvas, {
                type: 'line',
                data: dataGraph,
                options: {
                    plugins: {
                        title: {
                            display: true,
                            text: nomsTypes + ' en ' + unitesMesures,
                        }
                    }
                }
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