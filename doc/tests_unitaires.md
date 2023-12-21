# Tests unitaires
## Initialisation
On ajoute dans la base de donnée deux types de mesure :
```json
[
    {
        "idType":       1,
        "nomType":     "Température",
        "uniteMesure": "°C",
        "limiteMin":   -40,
        "limiteMax":   125
    }, {
        "idType":       2,
        "nomType":     "Humidité",
        "uniteMesure": "%RH",
        "limiteMin":    0,
        "limiteMax":  100
    }
]
```
Puis trois appareils :
```json
[
    {
        "idAppareil":  "192.168.0.32-1",
        "nomAppareil": "RaspberryPI 4 - Humidité",
        "idType":       2
    }, {
        "idAppareil":  "192.168.0.32-5",
        "nomAppareil": "RaspberryPI 4 - Température",
        "idType":       1
    }, {
        "idAppareil":  "192.168.0.31-6",
        "nomAppareil": "RaspberryPI Zéro",
        "idType":       1
    },
]
```
Et enfin 3 mesures :
```json
[
    {
        "valeur":      0.0429705,
        "instant":    "2023-12-21 11:12:46",
        "idAppareil": "192.168.0.32-5"
    }, {
        "valeur":      0.331967,
        "instant":    "2023-12-22 00:00:00",
        "idAppareil": "192.168.0.32-6"
    }, {
        "valeur":      0.310992,
        "instant":    "2023-12-21 11:12:52",
        "idAppareil": "192.168.0.32-1"
    },
]
```
## Requêtes GET
### `/measures/{datedebut}/{datefin}`
En entrant `/measures/2023-12-21_00:00:00/2023-12-21_12:00:00`, on obtient :
```json
{
    "reussite":true,
    "message": null,
    "donnee": {
        "2": {
            "4831881408": {
                "valeur": "31.09915852546692",
                "instant":"2023-12-21 11:12:52"
            }
        }, "1": {
            "22011750592": {
                "valeur": "-32.90986120700836",
                "instant":"2023-12-21 11:12:46"
            }
        }
    }
}
```
Soit les mesures de tous les appareils actifs entre le 21 décembre à minuit et le 21 décembre à midi, groupés par leur type puis par leur appareil et enfin avec leur valeur à l'échelle ; soit -40 à 125 pour les températures (ID 1) et 0 à 100 pour l'humidité (ID 2).

### `/devices`
En entrant `/devices`, on obtient :
```json
{
    "reussite": true,
    "message":  null,
    "donnee": [
        {
            "idAppareil":  "192.168.0.32-1",
            "nomAppareil": "RaspberryPI 4 - Humidité",
            "idType":       2,
            "activation":   true
        }, {
            "idAppareil":  "192.168.0.32-5",
            "nomAppareil": "RaspberryPI 4 - Température",
            "idType":       1,
            "activation":   true
        }, {
            "idAppareil":  "192.168.0.32-6",
            "nomAppareil": "RaspberryPI Zéro",
            "idType":       1,
            "activation":   true
        }
    ]
}
```
Soit la liste des appareils que l'on a entré initialement.

### `/measuretypes`
En entrant `/devices`, on obtient :
```json
{
    "reussite": true,
    "message":  null,
    "donnee": [
        {
            "idType":       1,
            "nomType":     "Température",
            "uniteMesure": "°C",
            "limiteMin":   -40,
            "limiteMax":   125
        }, {
            "idType":       2,
            "nomType":     "Humidité",
            "uniteMesure": "%RH",
            "limiteMin":    0,
            "limiteMax":  100
        }
    ]
}
```
Soit la liste des types de mesure que l'on a entrée initialement.

### `/serverip`
En entrant `/serverip`, on obtient :
```json
{
    "reussite": true,
    "message":  null,
    "donnee":  "10.159.128.151"
}
```
Soit l'addresse IPV4 du serveur dans le réseau Ethernet.

## Requêtes POST
### `/newmeasure`
En entrant `/newmeasure` avec comme contenu :
```json
{
    "valeur":      1,
    "instant":     1,
    "idAppareil": "192.168.0.32-5"
}
```
Puis en entrant `1970-01-01_01:00:00/2023-12-25_00:00:00`, on obtient :
```json
{
    "reussite": true,
    "message":  null,
    "donnee": {
        "2": {
            "4831881408": {
                "valeur":  "31.09915852546692",
                "instant": "2023-12-21 11:12:52"
            }
        }, "1": {
            "22011750592": {
                "valeur":  "-32.90986120700836,125",
                "instant": "2023-12-21 11:12:46,1970-01-01 01:00:01"
            },
            "26306717888": {
                "valeur":  "14.774554371833801",
                "instant": "2023-12-22 00:00:00"
            }
        }
    }
}
```
Soit toutes les mesures depuis 1970 jusqu'au 25 décembre à minuit. On voit l'ajout de la mesure à l'instant 1 et à la valeur maximale de son type soit 125°C.

### `/newdevice`
En entrant `/newdevice` avec comme contenu :
```json
{
    "idAppareil":  "192.168.0.32-168",
    "nomAppareil": "RaspberryPI 4 - Alpha",
    "idType":       2,
    "activation":   false
}
```
Ainsi, on ajoute un appareil initialement désactivé.

### `/newmeasuretype`
En entrant `/newmeasuretype` avec comme contenu une mesure en Fahrenheit :
```json
{
    "nomType":     "Température",
    "uniteMesure": "°F",
    "limiteMin":   -40,
    "limiteMax":   257
}
```
Puis en entrant `/measuretypes`, on obtient bien :
```json
{
    "reussite": true,
    "message":  null,
    "donnee": [
        {
            "idType":       1,
            "nomType":     "Température",
            "uniteMesure": "°C",
            "limiteMin":   -40,
            "limiteMax":   125
        }, {
            "idType":       2,
            "nomType":     "Humidité",
            "uniteMesure": "%RH",
            "limiteMin":    0,
            "limiteMax":  100
        }, {
            "idType":       17,
            "nomType":     "Température",
            "uniteMesure": "°F",
            "limiteMin":   -40,
            "limiteMax":   257
        }
    ]
}
```

## Requêtes POST
### `/device`

En entrant `/device` avec comme contenu :
```json
{
    "idAppareil":  "192.168.0.32-1",
    "activation":   false
}
```
Puis en entrant `/measures/2023-12-21_00:00:00/2023-12-21_12:00:00`, on voit bien que l'appareil `192.168.0.32-1` a été désactivé :
```json
{
    "reussite": true,
    "message":  null,
    "donnee": {
        "1": {
            "22011750592": {
                "valeur":  "-32.90986120700836",
                "instant": "2023-12-21 11:12:46"
            }
        }
    }
}
```

## Requêtes DELETE
### `/device-{id}`

En entrant `/device-192.168.0.32-5` puis en entrant `\devices`, on obtient bien la liste des appareils avec "RaspberryPI - Température" et toutes les mesures associées en moins :
```json
{
    "reussite": true,
    "message":  null,
    "donnee": [
        {
            "idAppareil":  "192.168.0.32-1",
            "nomAppareil": "RaspberryPI 4 - Humidité",
            "idType":       2,
            "activation":   true
        }, {
            "idAppareil":  "192.168.0.32-6",
            "nomAppareil": "RaspberryPI Zéro",
            "idType":       1,
            "activation":   true
        }, {
            "idAppareil":  "192.168.0.32-168",
            "nomAppareil": "RaspberryPI 4 - Alpha",
            "idType":       2,
            "activation":   false
        }
    ]
}
```

### `/measuretype-{id}`
En entrant `/measuretype-17` puis en entrant `\measuretypes`, on obtient bien la liste des types de mesure avec "Fahrenheit" et tous les appareils et mesures associés en moins :
```json
{
    "reussite": true,
    "message":  null,
    "donnee": [
        {
            "idType":       1,
            "nomType":     "Température",
            "uniteMesure": "°C",
            "limiteMin":   -40,
            "limiteMax":   125
        }, {
            "idType":       2,
            "nomType":     "Humidité",
            "uniteMesure": "%RH",
            "limiteMin":    0,
            "limiteMax":  100
        }
    ]
}
```

## Gestion des erreurs
### Ajouter une entrée déjà existante

En postant `/device` avec comme contenu
```json
{
    "idAppareil":  "192.168.0.32-1",
    "nomAppareil": "RaspberryPI 4 - Alpha",
    "idType":       2,
    "activation":   false
}
```
On obtient l'erreur suivante car cette identifiant est déjà utilisé :
```json
{
    "reussite": false,
    "message": "Impossible d'ajouter RaspberryPI 4 - Alpha à la liste des appareils !",
    "donnee": null
}
```

### Utiliser un identifiant invalide

En supprimant `/device-192.168.0.32` on obtient l'erreur suivante car l'identifiant ne correspond pas au format `byte.byte.byte.byte-int` :
```json
{
    "reussite":  false,
    "message":  "192.168.0.32 n'est pas un identifiant sous le format IPV4-ID !","donnee":    null
}
```

### Lancer deux requêtes accédant à la base de donnée au même moment

En lançant une requête accédant à la base de donnée alors qu'une autre faisant de même est en train d'être gérée par l'API, on obtient :
```json
{
    "reussite":  false,
    "message":  "L'API est déjà en train de gérer une requête !",
    "donnee":    null
}
```
