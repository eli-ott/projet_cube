# Projet Cube (préparation)
## Requêtes
### GET
- `/citiesinzipcode-{ZIP_CODE}` → renvois les villes ayant ce code postal 
  ```json
  "/citiesinzipcode-59112" : [
    {"zipCode":"59112","cityName":"annoeullin"},
    {"zipCode":"59112","cityName":"carnin"}
  ]
  ```
- `/citypos-{ZIP_CODE+SLUG}` → renvois la position GPS de la ville
  ```json
  "/citypos-59000lille" : { "lat" : 50.61381, "lng" : 3.0423598 }
  ```
- `/citydist-{ZIP_CODE+SLUG}-{ZIP_CODE+SLUG}` → renvois la distance en kilomètre entre deux villes
  ```json
  "/citydist-59112annoeullin-59000lille" : { "dist" : 12.774501 }
  ```
  `/zipcodesfromcity-{SLUG}` → renvois le(s) code(s) postal / postaux de la ville recherchée 
  ```json
  "/zipcodesfromcity-lille" : ["59160","59260","59000","59777","59800"]
  ```
- `/citiesinradius-{ZIP_CODE+SLUG}-{RADIUS}` → renvois la liste des villes autour d’une ville dans un rayon donné en kilomètre
  ```json
  "/citiesinradius-59000lille-4" : [
    {"zipCode":"59320","cityName":"emmerin"},
    {"zipCode":"59155","cityName":"faches_thumesnil"},
    {"zipCode":"59000","cityName":"lille"},
    {"zipCode":"59777","cityName":"lille"},
    {"zipCode":"59800","cityName":"lille"},
    {"zipCode":"59120","cityName":"loos"},
    {"zipCode":"59790","cityName":"ronchin"},
    {"zipCode":"59139","cityName":"wattignies"}
  ]
  ```
### POST
- `/newcity` → ajoute une nouvelle ville
  ```json
  {
    "cityName": "New York"
    "zipCode":  "12345",
    "gpsLat":    40.7648,
    "gpsLng":   -73.9808,
  }
  ```
