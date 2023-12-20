/**
 * Adding new user when clicking on signup
 */
const newUser = async () => {
    let username = document.getElementById('username').value;
    let password = document.getElementById('pass').value;

    const data = {
        username,
        password
    }

    const res = await fetch('http://localhost:5023/newuser', {
        method: "POST",
        headers: {
            "Content-type": "application/json"
        },
        body: JSON.stringify(data)
    });
    const ret = await res.json();
    if (ret.reussite === true) {
        localStorage.setItem('token', username);
        window.location = 'http://localhost:5500/front/pages/data.html';
    } else {
        alert('Impossible d\'ajouter le nouvel utilisateur');
    }
}

document.getElementById('signup').addEventListener('click', newUser);