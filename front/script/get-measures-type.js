const getMeasures = async () => {
    const res = await fetch('http://localhost:5023/measuretypes', {
        method: 'GET'
    });
    const ret = await res.json();
    console.log(ret);
    if (ret.reussite === true) {
        showMeasures(ret.donnee);
    }
}
getMeasures();
setInterval(() => {
    getMeasures();
}, 15000);

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
    });
}