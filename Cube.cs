using Cube.Data;
using MySql.Data.MySqlClient;

namespace Cube {
    public class Program {
        public static void Main(string[] args) {
            
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            WebApplication app            = builder.Build();


            DBConnection connection = DBConnection.Instance();
            connection.Server       = "localhost";
            connection.DatabaseName = "codes_postaux";
            connection.UserName     = "root";
            connection.Password     = "ESuKyuERu#2023";

            Dictionary<string, List<string>> zipCodes = new ();

            if (connection.IsConnect()) {
                
                string query = "SELECT zip_code, slug FROM cities";
                MySqlCommand    command = new (query, connection.Connection);
                MySqlDataReader reader  = command.ExecuteReader();
                while(reader.Read()) {

                    const int ZIP_CODE_ID = 0;
                    const int SLUG_ID     = 1;

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

                    } // if ..
                } // while ..

                connection.Close();

            } // if ..

            app.MapGet("/{zipCOde}", (string zipCode) => {

                // On vérifie s'il y a déjà un code postal enregistré
                zipCodes.TryGetValue(zipCode, out List<string>? cityNamesOrNull);

                // S'il n'y a aucune ville liée, on sort une liste avec un message comme valeur
                return cityNamesOrNull ?? new() { "Aucune ville" };
                
            }); // ..

            app.Run();

        } // void ..
    } // class ..
} // namespace ..
