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

            app.UseCors(SpecialOrigin);
            app.Run();

        } // void ..


        /// <summary>
        /// Retourne la position GPS d'une ville donnée
        /// <para> Exemple: 59000 Lille -> `/citypos59000lille` </para>
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
    } // class ..
} // namespace ..
