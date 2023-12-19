using System.Globalization;
using System.Numerics;
using Cube.Data;
using MySql.Data.MySqlClient;

namespace Cube {

        public class Measure {
            /** <summary> Valeur  normalisée entre 0 et 1 </summary> **/                 public float valeur     { get; set; }
            /** <summary> Nombre de secondes depuis 1970-01-01T00:00:00Z </summary> **/  public long  instant    { get; set; }
            /** <summary> Concatenation de l'IPV4 et ID de l'appareil </summary> **/     public int   idAppareil { get; set; }
        } // class ..

        public class Device {
            /** <summary> Concatenation de l'IPV4 et ID de l'appareil </summary> **/                  public int    idAppareil  { get; set; }
            /** <summary> Nom permettant aux utilisateurs de distinguer les appareils </summary> **/  public string nomAppareil { get; set; } = "Nouvel appareil";
            /** <summary> Identifiant du type de mesure associé </summary> **/                        public int    idType      { get; set; }
        } // class ..

        public class MeasureType {
            /** <summary> Nom permettant aux utilisateurs de distinguer les types de mesure </summary> **/  public string nomType     { get; set; } = "Nouveau type de mesure";
            /** <summary> Unité de mesure </summary> **/                                                    public string uniteMesure { get; set; } = "";
            /** <summary> Plus petite valeur acceptée </summary> **/                                        public float  limiteMin   { get; set; } = 0f;
            /** <summary> Plus grande valeur acceptée </summary> **/                                        public float  limiteMax   { get; set; } = 1f;
        } // class ..


        /// <summary>
        /// Une classe englobant les différents types de réponses de l'API
        /// </summary>
        /// <typeparam name="T"> Le type de donnée transférée </typeparam>
        public class ApiResponse<T>  {

            /** <summary> Indique si la requête s'est terminée avec succès </summary> **/ public bool success { get; set; }
            /** <summary> Les données transférées. </summary> **/                         public T?   data    { get; set; }

            public static ApiResponse<string> Error(string errorMessage) => new() { success = false, data = errorMessage };
            public static ApiResponse<T>      Success(T data)            => new() { success = true,  data = data };
            public static ApiResponse<T>      Success()                  => new() { success = true,  data = default };

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
