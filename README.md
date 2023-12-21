# Projet Cube
## Présentation
...

## Premier démarage

La première étape consiste en la création d'un base de donnée MySQL avec comme paramètres :
- serveur : `localhost`
- nom : `cubes`
- utilisateur : `root`
- mot de passe : `ESuKyuERu#2023`

Puis lancer dans l'ordre ces commandes SQL :
```sql
CREATE DATABASE cubes;
```
```sql
CREATE TABLE `type_mesure` (
  `id_type`      int         NOT NULL AUTO_INCREMENT,
  `nom_type`     varchar(50) NOT NULL,
  `unite_mesure` varchar(15) NOT NULL,
  `limite_min`   float       NOT NULL,
  `limite_max`   float       NOT NULL,
  PRIMARY KEY (`id_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
```
```sql
CREATE TABLE `appareil` (
  `id_appareil`  bigint PRIMARY KEY NOT NULL,
  `nom_appareil` varchar(50)        NOT NULL,
  `id_type`      int                NOT NULL,
  `activation`   bit(1)             NOT NULL DEFAULT b'1',
  FOREIGN KEY (`id_type`) REFERENCES type_mesure(`id_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
```
```sql
CREATE TABLE `mesure` (
  `id_mesure`   int PRIMARY KEY NOT NULL AUTO_INCREMENT,
  `valeur`      float           NOT NULL,
  `instant`     timestamp       NOT NULL,
  `id_appareil` bigint          NOT NULL,
  FOREIGN KEY (`id_appareil`) REFERENCES appareil(`id_appareil`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci
```

La seconde étape consiste en l'installation de [.NET SDK 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0). Il faut ensuite s'assurer d'héberger le répertoire afin de pouvoir utiliser l'interface web. Une fois terminée, il ne reste plus qu'à entrer la commande dans le répertoire : `dotnet run`.

## Annexes
- [Documentation de l'API](doc/documentation.md)
- [Tests unitaires](doc/tests_unitaires.md)
