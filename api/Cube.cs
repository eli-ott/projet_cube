using System.Globalization;
using System.Numerics;
using Cube.Data;
using MySql.Data.MySqlClient;

namespace Cube {

    public class Measure {
        public float valeur     { get; set; }
        public long  instant    { get; set; }
        public int   idAppareil { get; set; }
    } // class ..


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
            connection.DatabaseName = "cubes";
            connection.UserName     = "root";
            connection.Password     = "ESuKyuERu#2023";

            if (connection.IsConnect()) {



            } // void ..

            GetAllMeasures(app);
            GetLastMeasure(app);
            PostMeasure(app);

            static void GetAllMeasures(WebApplication app){
                app.MapGet("/measures-{idAppareil}", (int idAppareil) => {
                    
                }); //app.MapGet ..
            } //void ..

            static void GetLastMeasure(WebApplication app){
                app.MapGet("/lastmeasure-{idAppareil}", (int idAppareil) => {

                }); //app.MapGet ..
            } //void ..

            static void PostMeasure(WebApplication app){
                app.MapPost("/newmeasure", (Measure measure) => {
                    
                }); //app.MapGet ..
            } //void ..

            app.UseCors(SpecialOrigin);
            app.Run();

        } // void ..
    } // class ..
} // namespace ..
