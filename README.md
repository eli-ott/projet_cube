# Projet Cube (préparation)
## Requêtes
- `/citiesinzipcode-{ZIP_CODE}` → renvois les villes ayant ce code postal 
  ```json
  "/citiesinzipcode-59112" : [ "annoeullin","carnin"]
  ```
- `/citypos-{ZIP_CODE+SLUG}` → renvois la position GPS de la ville
  ```json
  "/citypos-59000lille" : { "lat" : 50.61381, "lng" : 3.0423598 }
  ```
- `/citydist-{ZIP_CODE+SLUG}-{ZIP_CODE+SLUG}` → renvois la distance en kilomètre entre deux villes
  ```json
  "/citydist-59112annoeullin-59000lille" : { "dist" : 12.774501 }
  ```
- `/citiesinradius-{ZIP_CODE+SLUG}-{RADIUS}` → renvois la liste des villes autour d’une ville dans un rayon donné en kilomètre
  ```json
  "/citiesinradius-59000lille-4" : ["59320emmerin", "59155faches_thumesnil", "59000lille", "59777lille", "59800lille", "59120loos", "59790ronchin", "59139wattignies"]
  ```
