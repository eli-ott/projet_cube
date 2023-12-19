# Projet Cube
## Requêtes
### GET
- `/measures-{idAppareil}` → renvoi toutes les mesures de l'appareil
  ```json
  "/measures-15" : [
    {"mesure":"15","instant":"18-12-2023"},
    {"mesure":"18","instant":"18-12-2023"},
    {"mesure":"8","instant":"18-12-2023"}
  ]
  ```
  `/lastmeasure-{idAppareil}` → renvoi la dernière mesure de l'appareil
  ```json
  "/lastmeasure-15" : [
    {"mesure":"8","instant":"18-12-2023"}
  ]
  ```
---
### POST
- `/newmeasure` → Ajoute une nouvelle mesure
    ```json
    {
      "valeur":     0.6927177, // Mesure normalisée entre 0 et 1
      "instant":    24770,     // Nombre de secondes depuis 1970-01-01T00:00:00Z
      "idAppareil": 396        // IPV4 (32 bits) | ID (32 bits) 
    }
  ```
- `/newdevice` → Ajoute un nouvel appareil
    ```json
    {
      "idAppareil":   396,               // IPV4 (32 bits) | ID (32 bits) 
      "nomAppareil": "RaspberryPI Zero", // Nom permettant aux utilisateurs de distinguer les appareils
      "idType":       2                  // Identifiant du type de mesure associé
    }
  ```
- `/newmeasuretype` → Ajoute un nouveau type de mesure
    ```json
    {
      "nomType":     "Température", // Nom permettant aux utilisateurs de distinguer les types de mesure
      "uniteMesure": "°C",          // Unité de mesure
      "limiteMin":   -40,           // Plus petite valeur acceptée
      "limiteMax":   125            // Plus grande valeur acceptée
    }
  ```
---
### PUT
- `/device` → Mets à jour un appareil et l'ajoute s'il n'existe pas
    ```json
    {
      "idAppareil":   396,            // IPV4 (32 bits) | ID (32 bits) → doit être identique à celui de l'appareil à modifier
      "nomAppareil": "RaspberryPI 4", // Nom permettant aux utilisateurs de distinguer les appareils
      "idType":       2               // Identifiant du type de mesure associé
    }
  ```
---
### DELETE
- `/device-{id}` → Supprime un appareil à l'aide de son identifiant
- `/measuretype-{id}` → Supprime un appareil à l'aide de son identifiant
