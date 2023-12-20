# Projet Cube
## Requêtes

Toutes les reqûetes retournent une `ApiResponse` à leur fin sous le format suivant.
```json
{
  "reussite": <bool>,
  "donnee":   <Any?>,
  "message":  <string?>
}
```

### GET
- `/measures/{datedebut}/{datefin}` → Renvoi toutes les mesures en fonction de leur appareil et de leur type dans une plage horaire donnée
  ```json
  {
    "1": {
      "61209234": {
        "valeurs":  "0.5554475689992842,0.4896293096661193",
        "instants": "2023-12-20 13:33:10,2023-12-20 15:23:31,"
      }
    },
    "2": {
      "1208923": {
        "valeurs":  "12.23121234",
        "instants": "2023-12-19 15:31:00"
      }
    }
  }
- `/devices` → Renvoi la liste des appareils
```json
    [
      {
        "idAppareil":  "182.168.0.32-23", // IPV4 (32 bits) + ID (32 bits) → doit être identique à celui de l'appareil à modifier
        "nomAppareil": "RaspberryPI 4",   // Nom permettant aux utilisateurs de distinguer les appareils
        "idType":       2,                // Identifiant du type de mesure associé
        "activation":   true              // Indique si l'API doit écouter cet appareil
      }
    ]
```
- `/measuretypes` → Renvoi la liste des types de mesure
```json
    [
      {
        "idType":       2,            // Identifiant auto incrémenté
        "nomType":     "Température", // Nom permettant aux utilisateurs de distinguer les types de mesure
        "uniteMesure": "°C",          // Unité de mesure des relevés associés
        "limiteMin":   -40,           // Plus petite valeur acceptée
        "limiteMax":   125            // Plus grande valeur acceptée
      }
    ]
```
---
### POST
Ces requêtes permettent d'ajouter des données au programme via l'API.

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
      "idAppareil":  "182.168.0.32-23", // IPV4 (32 bits) + ID (32 bits) → doit être identique à celui de l'appareil à modifier
      "nomAppareil": "RaspberryPI 4",   // Nom permettant aux utilisateurs de distinguer les appareils
      "idType":       2,                // Identifiant du type de mesure associé
      "activation":   true              // Indique si l'API doit écouter cet appareil (`true` par défaut)
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
      "idAppareil":  "182.168.0.32-23", // IPV4 (32 bits) + ID (32 bits) → doit être identique à celui de l'appareil à modifier
      "nomAppareil": "RaspberryPI 4",   // Nom permettant aux utilisateurs de distinguer les appareils (optionel)
      "idType":       2,                // Identifiant du type de mesure associé                       (optionel)
      "activation":   true              // Indique si l'API doit écouter cet appareil                  (`true` par défaut)
    }
  ```
---
### DELETE
- `/device-{id}` → Supprime un appareil à l'aide de son identifiant
- `/measuretype-{id}` → Supprime un appareil à l'aide de son identifiant

## Base de donnée MySQL
```sql
CREATE DATABASE cubes;

CREATE TABLE `type_mesure` (
  `id_type`      int         NOT NULL AUTO_INCREMENT,
  `nom_type`     varchar(50) NOT NULL,
  `unite_mesure` varchar(15) NOT NULL,
  `limite_min`   float       NOT NULL,
	`limite_max`   float       NOT NULL,
  PRIMARY KEY (`id_type`),
  UNIQUE KEY `nom_type_unique` (`nom_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci

CREATE TABLE `appareil` (
  `id_appareil`  bigint PRIMARY KEY NOT NULL,
  `nom_appareil` varchar(50)        NOT NULL,
  `id_type`      int                NOT NULL,
  `activation`   bit(1)             NOT NULL DEFAULT b'1',
  FOREIGN KEY (`id_type`) REFERENCES type_mesure(`id_type`),
  UNIQUE KEY `nom_appareil_unique` (`nom_appareil`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci

CREATE TABLE `mesure` (
  `id_mesure`   int PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `valeur`      float           NOT NULL,
  `instant`     timestamp       NOT NULL,
  `id_appareil` bigint          NOT NULL,
  FOREIGN KEY (`id_appareil`) REFERENCES appareil(`id_appareil`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
```