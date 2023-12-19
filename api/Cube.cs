using System.Diagnostics;
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
            PostDevice(app);
            PostMeasureType(app);


            static void GetAllMeasures(WebApplication app){
                app.MapGet("/measures-{idAppareil}", (int idAppareil) => {
                    
                }); //app.MapGet ..
            } //void ..

            static void GetLastMeasure(WebApplication app){
                app.MapGet("/lastmeasure-{idAppareil}", (int idAppareil) => {

                }); //app.MapGet ..
            } //void ..

            static void PostMeasure(WebApplication app)     => app.MapPost("/newmeasure",     (Measure     measure)     => AddMeasure(measure));
            static void PostDevice(WebApplication app)      => app.MapPost("/newdevice",      (Device      device)      => AddDevice(device));
            static void PostMeasureType(WebApplication app) => app.MapPost("/newmeasuretype", (MeasureType measureType) => AddMeasureType(measureType));


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

            if (measure.valeur < 0f || measure.valeur > 1f)
                ConsoleLogger.LogWarning("La mesure du " + dateTime + " n'est pas normalisée entre 0 et 1 ! Cela peut causer des problèmes lors de la lecture.");

            try {
                using var command = new MySqlCommand(query, instance.Connection);
                command.Parameters.AddWithValue("@valeur",      measure.valeur);
                command.Parameters.AddWithValue("@instant",     dateTime);
                command.Parameters.AddWithValue("@id_appareil", measure.idAppareil);
                command.ExecuteNonQuery();
            } catch (MySqlException _) { ConsoleLogger.LogError("Impossible d'ajouter la mesure du " + dateTime + " !"); }
        } // void ..


        /// <summary>
        /// Ajoute un appareil dans la base de donnée si le type de mesure associé est déjà enregistré.
        /// </summary>
        /// <param name="device"> Un appareil de mesure. </param>
        static void AddDevice(Device device) {

            DBConnection instance = DBConnection.Instance();
            if (!instance.IsConnect())
                instance.Connection?.Open();

            string query = "INSERT INTO `appareil`(`id_appareil`, `nom_appareil`, `id_type`) VALUES (@id_appareil, @nom_appareil, @id_type)";

            try {
                using var command = new MySqlCommand(query, instance.Connection);
                command.Parameters.AddWithValue("@id_appareil",  device.idAppareil);
                command.Parameters.AddWithValue("@nom_appareil", device.nomAppareil);
                command.Parameters.AddWithValue("@id_type",      device.idType);
                command.ExecuteNonQuery();
            } catch (MySqlException _) { ConsoleLogger.LogError("Impossible d'ajouter " + device.nomAppareil + " à la liste des appareils !"); }
        } // void ..


        /// <summary>
        /// Ajoute un type de mesure dans la base de donnée.
        /// </summary>
        /// <param name="measureType"> Un type de mesure. </param>
        static void AddMeasureType(MeasureType measureType) {

            DBConnection instance = DBConnection.Instance();
            if (!instance.IsConnect())
                instance.Connection?.Open();

            string query = "INSERT INTO `type_mesure`(`nom_type`, `unite_mesure`, `limite_min`, `limite_max`) VALUES (@nom_type, @unite_mesure, @limite_min, @limite_max)";

            try {
                using var command = new MySqlCommand(query, instance.Connection);
                command.Parameters.AddWithValue("@nom_type",     measureType.nomType);
                command.Parameters.AddWithValue("@unite_mesure", measureType.uniteMesure);
                command.Parameters.AddWithValue("@limite_min",   measureType.limiteMin);
                command.Parameters.AddWithValue("@limite_max",   measureType.limiteMax);
                command.ExecuteNonQuery();
            } catch (MySqlException _) { ConsoleLogger.LogError("Impossible d'ajouter " + measureType.nomType + " à la liste des types de mesure !"); }
        } // void ..
    } // class ..
} // namespace ..
