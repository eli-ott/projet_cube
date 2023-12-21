if(localStorage.getItem('token')) {
    document.getElementById('se-connecter').style.display = "none";
}

/**
 * Check if the user already have a token to redirect to connect.html either do nothing
 */
const checkConnect = async () => {
    if (!localStorage.getItem('token')) window.location = 'http://localhost:5500/front/pages/connect.html';

    const res = await fetch('http://localhost:5023/usernames', {
        method: 'GET'
    });

    const ret = await res.json();

    if (!ret.donnee.some(username => username === localStorage.getItem('token'))) {
        window.location = 'http://localhost:5500/front/pages/connect.html';
    }
}
if (window.location.href !== 'http://localhost:5500/front/pages/connect.html' &&
    window.location.href !== 'http://localhost:5500/front/pages/data.html' &&
    window.location.href !== 'http://localhost:5500/front/pages/new-user.html') {
    checkConnect();
}

/**
 * Connect the user and set the token to its username
 */
const connect = async () => {
    const username = document.getElementById('username').value;
    const password = document.getElementById('pass').value;

    const data = {
        username,
        password
    }

    const res = await fetch('http://localhost:5023/checkconnection', {
        method: 'POST',
        headers: {
            "Content-type": 'application/json',
        },
        body: JSON.stringify(data)
    });
    const ret = await res.json();
    if (ret.reussite === true) {
        localStorage.setItem('token', username);
        window.location = 'http://localhost:5500/front/pages/data.html';
    } else {
        alert('Erreur lors de la connection');
    }
}

if (document.getElementById('connect')) document.getElementById('connect').addEventListener('click', connect);