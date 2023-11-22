using Cube.Data;
using MySql.Data.MySqlClient;

namespace Cube {
    public class Program {
        public static void Main(string[] args) {

            var SpecialOrigin = "special_origin";
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(option => {
                option.AddPolicy(name: SpecialOrigin, policy => {
                    policy.WithOrigins("http://127.0.0.1:5500", "http://127.0.0.1:5023");
                });
            });

            builder.Services.AddControllers();
          
            WebApplication app = builder.Build();

            DBConnection connection = DBConnection.Instance();
            connection.Server       = "localhost";
            connection.DatabaseName = "codes_postaux";
            connection.UserName     = "root";
            connection.Password     = "ESuKyuERu#2023";

            if (connection.IsConnect()) {
                
                string query = "SELECT zip_code, name FROM cities";
                MySqlCommand    command = new (query, connection.Connection);
                MySqlDataReader reader  = command.ExecuteReader();
                while(reader.Read()) {

                    // On vérifie que les données ne soient pas nulles
                    if (!reader.IsDBNull(0) && !reader.IsDBNull(1)) {

                        string someStringFromColumnZero = reader.GetString("zip_code");
                        string someStringFromColumnOne  = reader.GetString("name");
                        Console.WriteLine(someStringFromColumnZero + "," + someStringFromColumnOne);

                    } // if ..
                } // while ..

                connection.Close();

            } // if ..

            app.UseCors(SpecialOrigin);

            app.Run();

        } // void ..
    } // class ..
} // namespace ..
