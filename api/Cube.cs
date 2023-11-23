using System.Globalization;
using System.Numerics;
using Cube.Data;
using MySql.Data.MySqlClient;

namespace Cube {
    public class Program {
        public static void Main(string[] args) {

            var SpecialOrigin = "cors_app";
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(option => {
                option.AddPolicy(SpecialOrigin, builder => {
                    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                }); // ..
            }); // ..

            builder.Services.AddControllers();
          
            WebApplication app = builder.Build();

            DBConnection connection = DBConnection.Instance();
            connection.Server       = "localhost";
            connection.DatabaseName = "codes_postaux";
            connection.UserName     = "root";
            connection.Password     = "ESuKyuERu#2023";

            // Noms de ville en fonction du code postal
            Dictionary<string, List<string>> zipCodes = new ();

            // Position GPS en fonction du code postal et du nom de la ville
            Dictionary<string, Vector2> positions = new ();

            if (connection.IsConnect()) {
                
                string query = "SELECT zip_code, slug, gps_lat, gps_lng FROM cities";
                MySqlCommand    command = new (query, connection.Connection);
                MySqlDataReader reader  = command.ExecuteReader();
                while(reader.Read()) {

                    const int ZIP_CODE_ID = 0;
                    const int SLUG_ID     = 1;
                    const int GPS_LAT_ID  = 2;
                    const int GPS_LNG_ID  = 3;

                    // On vérifie que les données ne soient pas nulles
                    if (!reader.IsDBNull(ZIP_CODE_ID) && !reader.IsDBNull(SLUG_ID)) {

                        string zipCode  = reader.GetString(ZIP_CODE_ID);
                        string cityName = reader.GetString(SLUG_ID).Replace(' ', '_'); // On remplace les espaces par des '_'

                        // On vérifie s'il y a déjà un code postal enregistré
                        if (zipCodes.TryGetValue(zipCode, out List<string>? cityNamesOrNull)) {

                            // Si la liste n'est pas initialisée, on en créé une
                            if (cityNamesOrNull is List<string> cityNames) cityNames.Add(cityName);
                            else zipCodes[zipCode] = new () { cityName };

                         } else zipCodes.Add(zipCode, new () { cityName });


                        // On vérifie que les positions ne soient pas nulles
                        if (!reader.IsDBNull(GPS_LAT_ID) && !reader.IsDBNull(GPS_LNG_ID)) {

                            Vector2 gpsPosition = new (reader.GetFloat(GPS_LAT_ID), reader.GetFloat(GPS_LNG_ID));
                            positions.TryAdd(zipCode + cityName, gpsPosition);

                         } // if ..
                    } // if ..
                } // while ..

                connection.Close();

            } // if ..


            GetCitiesInZipCode(app, zipCodes);
            GetCityPosition(app, positions);
            GetZipCode(app, zipCodes);
            GetCityDistance(app, positions);
            GetCitiesInRadius(app, positions);


            app.UseCors(SpecialOrigin);
            app.Run();

        } // void ..


        /// <summary>
        /// Retourne les codes postaux liées à un nom de ville
        /// </summary>
        /// <param name="app"></param>
        /// <param name="zipCodes"></param>
        private static void GetZipCode(
            WebApplication app,
            Dictionary<string, List<string>> zipCodes
        ) {
            app.MapGet("/zipcodesfromcity-{cityName}", (string cityName) => {

                List<string> foundZipCodes = new ();   

                foreach(KeyValuePair<string, List<string>> entry in zipCodes)
                    if(entry.Value.Contains(cityName))
                        foundZipCodes.Add(entry.Key);

                if(foundZipCodes.Count == 0)
                     Console.WriteLine(cityName + " n'est associé(e) à aucun code postal !");
                else Console.WriteLine(cityName + " est associé au code postal : " + foundZipCodes.ToString());
                return foundZipCodes;

            }); // app.MapGet ..
        } // void ..


        /// <summary>
        /// Retourne les villes possédant un même code postal
        /// <para> Exemple: 59000 -> `/citiesinzipcode-59000` </para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="zipCodes"></param>
        private static void GetCitiesInZipCode(
            WebApplication app,
            Dictionary<string, List<string>> zipCodes
        ) {
            app.MapGet("/citiesinzipcode-{zipCode}", (string zipCode) => {

                // On vérifie s'il y a déjà un code postal enregistré
                zipCodes.TryGetValue(zipCode, out List<string>? cityNamesOrNull);
                if (cityNamesOrNull is List<string> cityNames)
                     Console.WriteLine(cityNames.ToString() + " est / sont associé(es) au code postal : " + zipCode);

                else Console.WriteLine(zipCode + " n'est associé à aucune ville !");

                return cityNamesOrNull?.Select(cityName => (zipCode + cityName).ParseCityKey() );
                
            }); // ..
        } // void ..


        /// <summary>
        /// Retourne la position GPS d'une ville donnée
        /// <para> Exemple: 59000 Lille -> `/citypos-59000lille`</para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="positions"></param>
        private static void GetCityPosition(
            WebApplication app,
            Dictionary<string, Vector2> positions
        ) {
            app.MapGet("/citypos-{citykey}", (string citykey) => {

                // On vérifie s'il y a déjà une clé de ville enregistrée
                if (positions.TryGetValue(citykey, out Vector2 position)) {

                    Console.WriteLine("Le ville de code: " + citykey + " est située à la latitude : " + position.X + " et la longitude : " + position.Y);
                    return new Dictionary<string, float>() {{ "lat", position.X }, { "lng", position.Y }};

                } else {

                    Console.WriteLine("Le code de ville : " + citykey + " n'est associé à aucune position GPS !");
                    return null;

                } // if ..
            }); // ..
        } // void ..


        /// <summary>
        /// Retourne la distance en KM entre deux villes données
        /// <para> Exemple: 59000 Lille à 75001 Paris -> `/citydist-59000lille-75001paris`</para>
        /// <para> En cas d'erreur, la valeur sera de -1.KM </para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="positions"></param>
        private static void GetCityDistance(
            WebApplication app,
            Dictionary<string, Vector2> positions
        ) {
            app.MapGet("/citydist-{args}", (string args) => {

                string[] argsSplit = args.Split('-');

                // On vérifie s'il y a bien deux clés
                if (argsSplit.Length == 2)
                    // Puis si les deux clé de ville sont enregistrées
                    // Dans le cas contraire on renvois une valleur nulle
                    if (positions.TryGetValue(argsSplit[0], out Vector2 positionA))
                        if (positions.TryGetValue(argsSplit[1], out Vector2 positionB))
                            return new Dictionary<string, float>() {{ "dist", ComputeGPSDistance(positionA, positionB) }};

                        else Console.WriteLine("Le code de ville : " + argsSplit[1] + " n'est associé à aucune position GPS !");
                    else Console.WriteLine("Le code de ville : " + argsSplit[0] + " n'est associé à aucune position GPS !");
                else Console.WriteLine("Le nombre d'argument est incorrect (" + argsSplit.Length + "/2) !");

                return null;
            }); // ..
        } // void ..


        /// <summary>
        /// Retourne la liste des villes autour d'une ville donnée et d'un rayon en KM donné
        /// <para> Exemple: 59000 Lille dans un rayon de 24.3.KM -> `/citiesinradius-59000lille-24.3` </para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="positions"></param>
        private static void GetCitiesInRadius(
            WebApplication app,
            Dictionary<string, Vector2> positions
        ) {
            app.MapGet("/citiesinradius-{args}", (string args) => {

                string[] argsSplit = args.Split('-');
                // On vérifie s'il y a bien deux clés
                if (argsSplit.Length == 2)
                    // Puis si la clé de la ville centre est entregistrée
                    // Dans le cas contraire on renvois une valleur nulle
                    if (positions.TryGetValue(argsSplit[0], out Vector2 center))

                        // Enfin on vérifie si le rayon est un nombre
                        if (float.TryParse(argsSplit[1], CultureInfo.InvariantCulture.NumberFormat, out float radius))
                            return ComputeCitiesInRadius(positions, center, radius);

                        else Console.WriteLine(argsSplit[1] + " n'est pas un nombre valide !");
                    else Console.WriteLine("Le code de ville : " + argsSplit[0] + " n'est associé à aucune position GPS !");
                else Console.WriteLine("Le nombre d'argument est incorrect (" + argsSplit.Length + "/2) !");

                return null
            }); // ..
        } // void ..


        /// <summary> Diamètre de la Terre en kilomètres </summary>
        const float EARTH_RADIUS_KM = 6367.445f;

        /// <summary> Circonférence de la Terre en kilomètres </summary>
        const float EARTH_CIRCUMFERENCE_KM = 40075f;
        
        /// <summary> Nombre de kilomètre entre un degré de latitude </summary>
        const float KM_PER_DEGREE_LAT = EARTH_CIRCUMFERENCE_KM / 360f;

        /// <summary> Permets de convertir des degrés en radians </summary>
        const float DEGREE_TO_RADIAN = MathF.PI / 180;

        /// <summary>
        /// Permet de calculer la distance entre deux coordonnées gps sur Terre
        /// </summary>
        /// <param name="gpsLocationA"></param>
        /// <param name="gpsLocationB"></param>
        /// <returns></returns>
        private static float ComputeGPSDistance(
            Vector2 gpsLocationA,
            Vector2 gpsLocationB
        ) {

            // Conversion en radians
            float lat1Rad = gpsLocationA.X * DEGREE_TO_RADIAN;
            float lat2Rad = gpsLocationA.X * DEGREE_TO_RADIAN;
            float deltaLatRad = (gpsLocationB.X - gpsLocationA.X) * DEGREE_TO_RADIAN;
            float deltaLngRad = (gpsLocationB.Y - gpsLocationA.Y) * DEGREE_TO_RADIAN;

            float halfDeltaLatRad = deltaLatRad * 0.5f;
            float halfDeltaLngRad = deltaLngRad * 0.5f;

            // Formule de Haversine
            float a = MathF.Sin(halfDeltaLatRad) * MathF.Sin(halfDeltaLatRad)
                + MathF.Cos(lat1Rad) * MathF.Cos(lat2Rad)
                * MathF.Sin(halfDeltaLngRad) * MathF.Sin(halfDeltaLngRad);

            float c = 2 * MathF.Atan2(MathF.Sqrt(a), MathF.Sqrt(1 - a));

            // Calcul de la distance
            float distance = EARTH_RADIUS_KM * c;

            return distance;

        } // float ..

        /// <summary>
        /// Calcule la différence maximale de latitude et longitude dans un rayon donné sur Terre basée sur une latitude et un rayon donné
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static Vector2 ComputeMaxGPSDistance(
            float lat,
            float radius
        ) {
        
            // La différence de KM par latitude est constante
            float maxLatitudeDiff = radius / KM_PER_DEGREE_LAT;
        
            // La différence de KM par longitude est dépendante de la latitude
            float maxLonDiff = radius / (KM_PER_DEGREE_LAT * MathF.Cos(lat * DEGREE_TO_RADIAN));
        
            // Prise en considération des données extrêmes
            if (lat + maxLatitudeDiff > 90)       maxLatitudeDiff = 90 - lat;
            else if (lat - maxLatitudeDiff < -90) maxLatitudeDiff = 90 + lat;
        
            return new Vector2(maxLatitudeDiff, maxLonDiff);

        } // Vector2 ..


        /// <summary>
        /// Calcule la liste des villes dans un rayon donné en KM
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static List<string> ComputeCitiesInRadius(
            Dictionary<string, Vector2> positions,
            Vector2 center,
            float   radius
        ) {
            List<string> cities = new ();
            Vector2 maxDiff     = ComputeMaxGPSDistance(center.X, radius);
            foreach (KeyValuePair<string, Vector2> position in positions)

                // On vérifie si la ville est à une distance dite `mannhattan` inférieure au rayon donné (plus rapide)
                if (MathF.Abs(position.Value.X - center.X) <= maxDiff.X && MathF.Abs(position.Value.Y - center.Y) <= maxDiff.Y)

                    // Ensuite, on calcule si la ville est à une distance `euclidienne` inférieure au rayon donné (plus long)
                    if (ComputeGPSDistance(center, position.Value) <= radius)
                        cities.Add(position.Key);

            return cities;
        } // List<string> ..
    } // class ..
} // namespace ..
