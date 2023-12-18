# Projet Cube (préparation)
## Requêtes
### GET
- `/measures-{idAppareil}` → renvoi toutes les mesures de l'appareil
  ```json
  "/measures-15" : [
    {"measure":"15","date":"18-12-2023"},
    {"measure":"18","date":"18-12-2023"},
    {"measure":"8","date":"18-12-2023"}
  ]
  ```
  `/lastmeasure-{idAppareil}` → renvoi la dernière mesure de l'appareil
  ```json
  "/lastmeasure-15" : [
    {"measure":"8","date":"18-12-2023"}
  ]
  ```
### POST
- `/newmeasure` → Ajoute une nouvelle mesure
  ```json
  "/newmeasure" : [
    {
    "id_mesure": "12",
    "valeur": "12",
    "instant": 18-12-2023,
  }
  ]
  ```