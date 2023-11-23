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
                });
            });

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


            app.MapGet("/zipcode{zipCode}", (string zipCode) => {

                // On vérifie s'il y a déjà un code postal enregistré
                zipCodes.TryGetValue(zipCode, out List<string>? cityNamesOrNull);

                // S'il n'y a aucune ville liée, on sort une liste avec un message comme valeur
                return cityNamesOrNull ?? new() { "Aucune ville" };
                
            }); // ..

            GetCityPosition(app, positions);
            GetCityDistance(app, positions);

            app.UseCors(SpecialOrigin);
            app.Run();

        } // void ..


        /// <summary>
        /// Retourne la position GPS d'une ville donnée
        /// <para> Exemple: 59000 Lille -> `/citypos59000lille`</para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="positions"></param>
        private static void GetCityPosition(
            WebApplication app,
            Dictionary<string, Vector2> positions
        ) {

            app.MapGet("/citypos{citykey}", (string citykey) => {

                // On vérifie s'il y a déjà une clé de ville enregistrée
                if (positions.TryGetValue(citykey, out Vector2 position))
                    return new Dictionary<string, float>() {{ "lat", position.X }, { "lng", position.Y }};

                else return null;
                
            }); // ..
        } // void ..


        /// <summary>
        /// Retourne la distance en KM entre deux villes données
        /// <para> Exemple: 59000 Lille à 75001 Paris -> `/citydist59000lille-75001paris`</para>
        /// <para> En cas d'erreur, la valeur sera de -1.KM </para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="positions"></param>
        private static void GetCityDistance(
            WebApplication app,
            Dictionary<string, Vector2> positions
        ) {

            app.MapGet("/citydist{citykeys}", (string citykeys) => {

                String[] cityKeysParsed = citykeys.Split('-', 2);

                // On vérifie s'il y a bien deux clés puis si les deux clé de ville sont enregistrées
                return new Dictionary<string, float?>() {{
                    "dist", (cityKeysParsed.Length == 2
                        && positions.TryGetValue(cityKeysParsed[0], out Vector2 positionA)
                        && positions.TryGetValue(cityKeysParsed[1], out Vector2 positionB))
                    ? GetGPSDistance(positionA, positionB)
                    : null
                }}; // ..
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
        private static float GetGPSDistance(
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
    } // class ..
} // namespace ..
