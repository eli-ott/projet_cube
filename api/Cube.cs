using System.Diagnostics;
using Cube.Data;
using MySql.Data.MySqlClient;


namespace Cube {
    public class Measure {
        /** <summary> Valeur  normalisée entre 0 et 1 </summary> **/                 public float valeur     { get; set; }
        /** <summary> Nombre de secondes depuis 1970-01-01T00:00:00Z </summary> **/  public long  instant    { get; set; }
        /** <summary> Concatenation de l'IPV4 et ID de l'appareil </summary> **/     public int   idAppareil { get; set; }
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

            static void PostMeasure(WebApplication app) => app.MapPost("/newmeasure", (Measure measure) => AddMeasure(measure));


            app.UseCors(SpecialOrigin);
            app.Run();

        } // void ..


        /// <summary>
        /// Ajoute une mesure dans la base de donnée si l'appareil associé est déjà enregistré.
        /// </summary>
        /// <param name="measure"> Une mesure générique. </param>
        static void AddMeasure(Measure measure) {

            DBConnection instance = DBConnection.Instance();
            if (!instance.IsConnect())
                instance.Connection?.Open();

            string query    = "INSERT INTO `mesure`(`valeur`, `instant`, `id_appareil`) VALUES (@valeur, @instant, @id_appareil)";
            string dateTime = DateTimeOffset.FromUnixTimeSeconds(measure.instant).UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            try {
                using var command = new MySqlCommand(query, instance.Connection);
                command.Parameters.AddWithValue("@valeur", measure.valeur);
                command.Parameters.AddWithValue("@instant", dateTime);
                command.Parameters.AddWithValue("@id_appareil", measure.idAppareil);
                command.ExecuteNonQuery();
            } catch (MySqlException _) { ConsoleLogger.LogError("Impossible d'ajouter la mesure du " + dateTime + " !"); }
        } // void ..
    } // class ..
} // namespace ..
