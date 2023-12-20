let date = new Date();

setInterval(() => {
    date = new Date();
    
    let showDate = `${date.toLocaleDateString()} ${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`;
    
    document.querySelector('#time').innerHTML = showDate;
}, 1000);