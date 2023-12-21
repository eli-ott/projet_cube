document.querySelector('#burger').addEventListener('click', () => {
    document.querySelector('nav').style.display = "flex";
});
document.querySelector('#x').addEventListener('click', () => {
    document.querySelector('nav').style.display = "none";
});