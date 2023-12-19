# Projet Cube (préparation)
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
### POST
Ces requêtes permettent d'ajouter des données au programme via l'API. Elles retournent une `ApiResponse` à leur fin sous le format suivant.
```json
{
  "success": <bool>,
  "data":    <string?>
}
```

- `/newmeasure` → Ajoute une nouvelle mesure
  ```json
  "/newmeasure" : [
    {
    "valeur": "12",
    "instant": "18-12-2023",
  }
  ]
  ```
## Base de donnée MySQL
```sql
CREATE DATABASE cubes;

CREATE TABLE `type_mesure` (
  `id_type`      INT(11) NOT NULL AUTO_INCREMENT,
  `nom_type`     VARCHAR(50) NOT NULL,
  `unite_mesure` VARCHAR(15) NOT NULL,
  `limite_min`   FLOAT NOT NULL,
  `limite_max`   FLOAT NOT NULL,
  PRIMARY KEY (`id_type`),
  UNIQUE  KEY `nom_type_unique` (`nom_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci

CREATE TABLE `appareil` (
  `id_appareil`  INT(64) PRIMARY KEY NOT NULL,
  `nom_appareil` VARCHAR(50) NOT NULL,
  `id_type`      INT(11) NOT NULL,
  `activation`   BIT(1) NOT NULL DEFAULT b'1',
  FOREIGN KEY (`id_type`) REFERENCES type_mesure(`id_type`),
  UNIQUE  KEY `nom_appareil_unique` (`nom_appareil`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci

CREATE TABLE `mesure` (
  `id_mesure`   INT(11) PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `valeur`      FLOAT NOT NULL,
  `instant`     TIMESTAMP NOT NULL,
  `id_appareil` INT(11) NOT NULL,
  FOREIGN KEY (`id_appareil`) REFERENCES appareil(`id_appareil`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
```