using Cube.Data;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;


namespace Cube {

    //===============
    // D O N N E E S
    //===============

        public class Measure {
            /** <summary> Valeur  normalisée entre 0 et 1 </summary> **/                 public          float  valeur     { get; set; }
            /** <summary> Nombre de secondes depuis 1970-01-01T00:00:00Z </summary> **/  public          long   instant    { get; set; }
            /** <summary> Concatenation de l'IPV4 et ID de l'appareil </summary> **/     public required string idAppareil { get; set; }
        } // class ..

        public class MeasuresValues{
            /** <summary> Chaine de caractères composée des valeurs mesurées séparées par une virgule </summary> **/ public required string valeurs;
            /** <summary> Chaine de caractères composée des instants séparées par une virgule </summary> **/         public required string instants;
            /** <summary> Type de la mesure </summary> **/                                                           public required string type;
        } // class ..


        public class Device {
            /** <summary> Concatenation de l'IPV4 et ID de l'appareil </summary> **/                  public required string  idAppareil  { get; set; }
            /** <summary> Nom permettant aux utilisateurs de distinguer les appareils </summary> **/  public          string? nomAppareil { get; set; } = "Nouvel appareil";
            /** <summary> Identifiant du type de mesure associé </summary> **/                        public          int?    idType      { get; set; }
            /** <summary> Indique si le programme doit enregistrer ses mesures </summary> **/         public          bool    activation  { get; set; } = true;
        } // class ..

        public class MeasureType {
            /** <summary> Identifiant auto incrémenté </summary> **/                                        public int?    idType      { get; set; }
            /** <summary> Nom permettant aux utilisateurs de distinguer les types de mesure </summary> **/  public string? nomType     { get; set; } = "Nouveau type de mesure";
            /** <summary> Unité de mesure </summary> **/                                                    public string? uniteMesure { get; set; }
            /** <summary> Plus petite valeur acceptée </summary> **/                                        public float   limiteMin   { get; set; } = 0f;
            /** <summary> Plus grande valeur acceptée </summary> **/                                        public float   limiteMax   { get; set; } = 1f;
        } // class ..
        



        /// <summary>
        /// Une classe englobant les différents types de réponses de l'API
        /// </summary>
        /// <typeparam name="T"> Le type de donnée transférée </typeparam>
        public class ApiResponse<T>  {

            /** <summary> Indique si la requête s'est terminée avec succès </summary> **/ public bool    reussite { get; set; }
            /** <summary> Les données transférées. </summary> **/                         public string? message  { get; set; }
            /** <summary> Les données transférées. </summary> **/                         public T?      donnee   { get; set; }

            public static ApiResponse<T> Error(string errorMessage) => new() { reussite = false, message = errorMessage };
            public static ApiResponse<T> Success(T data)            => new() { reussite = true,  donnee = data };
            public static ApiResponse<T> Success()                  => new() { reussite = true };

        } // class ..



    //===================
    // P R O G R A M M E
    //===================

        public class Program {
            public static void Main(string[] args) {

                string SpecialOrigin = "cors_app";
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


                //=========================
                // R E Q U Ê T E S   A P I
                //=========================

                    GetAllMeasures(app);
                    GetLastMeasure(app);
                    GetDevices(app);
                    GetMeasureTypes(app);

                    PostMeasure(app);
                    PostDevice(app);
                    PostMeasureType(app);

                    PutDevice(app);

                    DeleteDevice(app);
                    DeleteMeasureType(app);


                    static void GetAllMeasures(WebApplication app){
                     app.MapGet("/measures-{idAppareil}", (int idAppareil) => {
                        FindAllMeasures(idAppareil);
                     }); //app.MapGet ..
                    } //void ..

                    static void GetLastMeasure(WebApplication app){
                        app.MapGet("/lastmeasure-{idAppareil}", (int idAppareil) => {

                        }); //app.MapGet ..
                    } //void ..

                    static void GetDevices(WebApplication app)      => app.MapGet("devices",      () => EnsureThreadSafety(arggs => ReadDevices()));
                    static void GetMeasureTypes(WebApplication app) => app.MapGet("measuretypes", () => EnsureThreadSafety(args => ReadMeasureTypes()));

                    static void PostMeasure(WebApplication app)     => app.MapPost("/newmeasure",     (Measure     measure)     => EnsureThreadSafety(args => AddMeasure(measure)));
                    static void PostDevice(WebApplication app)      => app.MapPost("/newdevice",      (Device      device)      => EnsureThreadSafety(args => AddDevice(device)));
                    static void PostMeasureType(WebApplication app) => app.MapPost("/newmeasuretype", (MeasureType measureType) => EnsureThreadSafety(args => AddMeasureType(measureType)));

                    static void PutDevice(WebApplication app) => app.MapPut("/device", (Device device) => UpdateDevice(device));

                    static void DeleteDevice(WebApplication app)      => app.MapDelete("/device-{id}",      (string id) => EnsureThreadSafety(args => DeleteDeviceWithMeasures(id)));
                    static void DeleteMeasureType(WebApplication app) => app.MapDelete("/measuretype-{id}", (int id)    => EnsureThreadSafety(args => DeleteMeasureTypeWithDevices(id)));


                // Envoie des données aléatoires toutes les 5 secondes.
                _ = new Timer(async _ => await Simulation.Run(), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));


                app.UseCors(SpecialOrigin);
                app.Run();


            } // void ..
        

        //===============================
        // R E Q U Ê T E S   M Y . S Q L
        //===============================

            /** Objet cadenas pour prévenir les erreurs asynchrones lors des reqêtes. */ private static readonly object _Lock                             = new ();
            /** Objet cadenas pour prévenir les erreurs asynchrones lors des reqêtes. */ public static          bool     IsQuerying { get; private set; } = false;


            /// <summary>
            /// Applique la requête si aucune requête n'est actuellement gérée par l'API.
            /// </summary>
            /// <typeparam name="T">   Le type de données transférées lors de la requête. </typeparam>
            /// <param name="request"> La requête. </param>
            /// <param name="args">    Les éventuels arguments de la requête. </param>
            /// <returns>              Le résultat de la requête. </returns>
            public static ApiResponse<T> EnsureThreadSafety<T>(
                Func<object[], ApiResponse<T>> request,
                params object[] args
            ) {
                lock (Program._Lock) {
                    if (Program.IsQuerying)
                        return ApiResponse<T>.Error(ConsoleLogger.LogError("L'API est déjà en train de gérer une requête !"));

                    Program.IsQuerying = true;

                    try {

                        ApiResponse<T> output = request(args);
                        return output;

                    } finally { Program.IsQuerying = false; }
                } // lock ..
            } // ApiResponse ..


        /// <summary> Récupère toutes les mesures d'un appareil dans la base de donnée </summary>
        /// <param name="idAppareil"> L'identifiant de l'appareil recherché </param>
        static Dictionary<string, MeasuresValues> FindAllMeasures(int idAppareil){

            DBConnection instance = DBConnection.Instance();
            if (!instance.IsConnect())
                instance.Connection?.Open();

            /// Pour convertir une valeur normée en valeur originale = valeur normée * (borne max - borne min) + borne min
            string query = "SELECT GROUP_CONCAT((valeur*(type_mesure.limite_max-type_mesure.limite_min)+type_mesure.limite_min)) as mesures, GROUP_CONCAT(instant) as instants, nom_type, unite_mesure FROM mesure JOIN appareil ON mesure.id_appareil = appareil.id_appareil JOIN type_mesure ON appareil.id_type = type_mesure.id_type GROUP BY appareil.id_appareil";
           
            Dictionary<string, MeasuresValues> values = new Dictionary<string, MeasuresValues>();


            try {
                using var command = new MySqlCommand(query, instance.Connection);
                using var reader = command.ExecuteReader();
                while (reader.Read()){
                    values.Add(reader.GetString("unite_mesure"), new(){valeurs = reader.GetString("mesures"), instants = reader.GetString("instants"), type = reader.GetString("nom_type")});
                }
                reader.Close();
                return values;
            } catch (MySqlException _) { ConsoleLogger.LogError("Impossible d'afficher les mesures de l'appareil " + idAppareil + " !"); }
            return values;
        }

            /// <summary>
            /// Ajoute une mesure dans la base de donnée si l'appareil associé est déjà enregistré.
            /// </summary>
            /// <param name="measure"> Une mesure générique. </param>
            /// <returns> Une réponse API générique. </returns>
            static ApiResponse<Any> AddMeasure(Measure measure) {

                DBConnection instance = DBConnection.Instance();
                if (!instance.IsConnect())
                    instance.Connection?.Open();


                // On vérifie que l'identifiant de l'appareil associé est valide.
                if (measure.idAppareil.ToDeviceBinaryID() is long binaryID) {

                    string dateTime   = DateTimeOffset.FromUnixTimeSeconds(measure.instant).LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    string checkQuery = "SELECT COUNT(*) FROM `appareil` WHERE `id_appareil` = @id_appareil AND `activation`";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, instance.Connection)) {

                        checkCommand.Parameters.AddWithValue("@id_appareil", binaryID);

                        if (Convert.ToInt32(checkCommand.ExecuteScalar()) == 0)
                            return ApiResponse<Any>.Error(ConsoleLogger.LogError("Impossible d'ajouter la mesure du " + dateTime + " ; aucun appareil en activation ne correspond !"));

                    } // using ..
                    

                    // On fait la moyenne des 4 précédentes mesures avant celle donnée.
                    string averageQuery = "SELECT AVG(subquery.valeur) FROM (SELECT valeur FROM `mesure` WHERE `instant` <= @instant ORDER BY `instant` DESC LIMIT 4) AS `subquery`";

                    float average;
                    using (MySqlCommand averageCommand = new MySqlCommand(averageQuery, instance.Connection)) {

                        averageCommand.Parameters.AddWithValue("@instant", dateTime);
                        object result = averageCommand.ExecuteScalar();
                        average = result != DBNull.Value ? Convert.ToSingle(result) : measure.valeur;

                    } // using ..


                    // Puis on ajoute cette valeur dans le tableau.
                    string query = "INSERT INTO `mesure`(`valeur`, `instant`, `id_appareil`) VALUES (@valeur, @instant, @id_appareil)";
                    if (measure.valeur < 0f || measure.valeur > 1f)
                        ConsoleLogger.LogWarning("La mesure du " + dateTime + " n'est pas normalisée entre 0 et 1 ! Cela peut causer des problèmes lors de la lecture.");

                    try {

                        using MySqlCommand command = new MySqlCommand(query, instance.Connection);
                        command.Parameters.AddWithValue("@valeur",      (average + measure.valeur) * 0.5f);
                        command.Parameters.AddWithValue("@instant",     dateTime);
                        command.Parameters.AddWithValue("@id_appareil", binaryID);
                        command.ExecuteNonQuery();
                        ConsoleLogger.LogInfo("Ajout de la mesure du " + dateTime + ".");

                    } catch { return ApiResponse<Any>.Error(ConsoleLogger.LogError("Impossible d'ajouter la mesure du " + dateTime + " !")); }
                    return ApiResponse<Any>.Success();
                } else return ApiResponse<Any>.Error(ConsoleLogger.LogError(measure.idAppareil + " n'est pas un identifiant sous le format IPV4-ID !"));
            } // void ..


            /// <summary>
            /// Ajoute un appareil dans la base de donnée si le type de mesure associé est déjà enregistré.
            /// </summary>
            /// <param name="device"> Un appareil de mesure. </param>
            /// <returns> Une réponse API générique. </returns>
            static ApiResponse<Any> AddDevice(Device device) {

                DBConnection instance = DBConnection.Instance();
                if (!instance.IsConnect())
                    instance.Connection?.Open();

                string query = "INSERT INTO `appareil`(`id_appareil`, `nom_appareil`, `id_type`, `activation`) VALUES (@id_appareil, @nom_appareil, @id_type, @activation)";
                try {

                    using MySqlCommand command = new MySqlCommand(query, instance.Connection);
                    command.Parameters.AddWithValue("@id_appareil",  device.idAppareil.ToDeviceBinaryID());
                    command.Parameters.AddWithValue("@nom_appareil", device.nomAppareil);
                    command.Parameters.AddWithValue("@id_type",      device.idType);
                    command.Parameters.AddWithValue("@activation",   device.activation);
                    command.ExecuteNonQuery();
                    ConsoleLogger.LogInfo("Ajout de " + device.nomAppareil + " à la liste des appareils.");

                } catch { return ApiResponse<Any>.Error(ConsoleLogger.LogError("Impossible d'ajouter " + device.nomAppareil + " à la liste des appareils !")); }
                return ApiResponse<Any>.Success();
            } // void ..


            /// <summary>
            /// Ajoute un type de mesure dans la base de donnée.
            /// </summary>
            /// <param name="measureType"> Un type de mesure. </param>
            /// <returns> Une réponse API générique. </returns>
            static ApiResponse<Any> AddMeasureType(MeasureType measureType) {

                DBConnection instance = DBConnection.Instance();
                if (!instance.IsConnect())
                    instance.Connection?.Open();

                string query = "INSERT INTO `type_mesure`(`nom_type`, `unite_mesure`, `limite_min`, `limite_max`) VALUES (@nom_type, @unite_mesure, @limite_min, @limite_max)";
                try {

                    using MySqlCommand command = new MySqlCommand(query, instance.Connection);
                    command.Parameters.AddWithValue("@nom_type",     measureType.nomType);
                    command.Parameters.AddWithValue("@unite_mesure", measureType.uniteMesure);
                    command.Parameters.AddWithValue("@limite_min",   measureType.limiteMin);
                    command.Parameters.AddWithValue("@limite_max",   measureType.limiteMax);
                    command.ExecuteNonQuery();
                    ConsoleLogger.LogInfo("Ajout de " + measureType.nomType + " à la liste des types de mesure.");

                } catch { return ApiResponse<Any>.Error(ConsoleLogger.LogError("Impossible d'ajouter " + measureType.nomType + " à la liste des types de mesure !")); }
                return ApiResponse<Any>.Success();
            } // void ..


            /// <summary>
            /// Récupère les appareils dans la base de données.
            /// </summary>
            /// <returns> La liste des tous les appareils de mesure. </returns>
            static ApiResponse<List<Device>> ReadDevices() {

                DBConnection instance = DBConnection.Instance();
                if (!instance.IsConnect())
                    instance.Connection?.Open();

                    string query = "SELECT * FROM `appareil`";
                    try {

                        using MySqlCommand command = new (query, instance.Connection);
                        MySqlDataReader reader     = command.ExecuteReader();
                        List<Device> devices       = new();
                        
                        while (reader.Read())
                            devices.Add(new() {
                                idAppareil  = reader.GetInt64("id_appareil").ToDeviceStringID(),
                                nomAppareil = reader.GetString("nom_appareil"),
                                activation  = reader.GetBoolean("activation"),
                                idType      = reader.GetInt32("id_type"),
                            }); // ..

                        reader.Close();
                        ConsoleLogger.LogInfo("Lecture de la liste des appareils avec succès.");

                        return ApiResponse<List<Device>>.Success(devices);
                    } catch { return ApiResponse<List<Device>>.Error(ConsoleLogger.LogError("Impossible de lire la liste des appareils !")); }
            } // List<Device> ..


            /// <summary>
            /// Récupère les types de mesure dans la base de données.
            /// </summary>
            /// <returns> La liste des tous les types de mesure. </returns>
            static ApiResponse<List<MeasureType>> ReadMeasureTypes() {

                DBConnection instance = DBConnection.Instance();
                if (!instance.IsConnect())
                    instance.Connection?.Open();

                    string query = "SELECT * FROM `type_mesure`";
                    try {

                        using MySqlCommand command     = new (query, instance.Connection);
                        MySqlDataReader reader         = command.ExecuteReader();
                        List<MeasureType> measureTypes = new();
                        
                        while (reader.Read())
                            measureTypes.Add(new() {
                                idType      = reader.GetInt32("id_type"),
                                nomType     = reader.GetString("nom_type"),
                                uniteMesure = reader.GetString("unite_mesure"),
                                limiteMin   = reader.GetFloat("limite_min"),
                                limiteMax   = reader.GetFloat("limite_max"),
                            }); // ..

                        reader.Close();
                        ConsoleLogger.LogInfo("Lecture de la liste des types de mesure avec succès.");

                        return ApiResponse<List<MeasureType>>.Success(measureTypes);
                    } catch { return ApiResponse<List<MeasureType>>.Error(ConsoleLogger.LogError("Impossible de lire la liste des types de mesure !")); }
            } // List<MeasureType> ..


            /// <summary>
            /// Mets à jour un appareil dans la base de donnée et l'ajoute s'il n'existe pas.
            /// </summary>
            /// <param name="device"> L'appareil mis à jour. </param>
            /// <returns> Une réponse API générique. </returns>
            static ApiResponse<Any> UpdateDevice(Device device) {

                DBConnection instance = DBConnection.Instance();
                if (!instance.IsConnect())
                    instance.Connection?.Open();


                // On vérifie que l'identifiant de l'appareil associé est valide.
                if (device.idAppareil.ToDeviceBinaryID() is long binaryID) {

                    string checkQuery = "SELECT COUNT(*) FROM `appareil` WHERE `id_appareil` = @id_appareil";
                    using (MySqlCommand checkCommand = new MySqlCommand(checkQuery, instance.Connection)) {

                        checkCommand.Parameters.AddWithValue("@id_appareil", binaryID);

                        if (Convert.ToInt32(checkCommand.ExecuteScalar()) == 0) {

                            ConsoleLogger.LogWarning("Aucun appareil trouvé avec l'identifiant " + device.idAppareil + " pour la mise à jour !");
                            AddDevice(device);
                            return ApiResponse<Any>.Success();

                        } // if ..
                    } // using ..


                    // On adapte la requête au valeur indiquées dans le PUT afin de ne changer que ce qui nous intéresse.
                    string query = "UPDATE `appareil` SET"
                    + (device.nomAppareil is not null ? " `nom_appareil` = @nom_appareil, " : "")
                    + (device.idType      is int    ?   " `id_type` = @id_type, "           : "")
                    + " `activation` = @activation WHERE `id_appareil` = @id_appareil";

                    try {

                        using MySqlCommand command = new MySqlCommand(query, instance.Connection);
                        command.Parameters.AddWithValue("@id_appareil",  binaryID);
                        if (device.nomAppareil is string nomAppareil) command.Parameters.AddWithValue("@nom_appareil", nomAppareil);
                        if (device.idType      is int    idType)      command.Parameters.AddWithValue("@id_type",      idType);
                        command.Parameters.AddWithValue("@activation",   device.activation);
                        command.ExecuteNonQuery();
                        ConsoleLogger.LogInfo("Aucune interruption lors de la modificationde l'appareil à l'identifiant " + device.idAppareil + ".");

                    } catch { return ApiResponse<Any>.Error(ConsoleLogger.LogError("Interruption lors de la modificationde l'appareil à l'identifiant " + device.idAppareil + " !")); }
                    return ApiResponse<Any>.Success();
                } else return ApiResponse<Any>.Error(ConsoleLogger.LogError(device.idAppareil + " n'est pas un identifiant sous le format IPV4-ID !"));
            } // void ..


            /// <summary>
            /// Supprime un appareil dans la base de donnée et toutes les mesures associées.
            /// </summary>
            /// <param name="id"> L'identifiant d'un appareil de mesure. </param>
            /// <returns> Une réponse API générique. </returns>
            static ApiResponse<Any> DeleteDeviceWithMeasures(string id) {

                DBConnection instance = DBConnection.Instance();
                if (!instance.IsConnect())
                    instance.Connection?.Open();

                string queryMeasures = "DELETE FROM `mesure` WHERE `id_appareil` = @id_appareil";
                try {
                    using MySqlCommand command = new MySqlCommand(queryMeasures, instance.Connection);
                    command.Parameters.AddWithValue("@id_appareil",  id);
                    command.ExecuteNonQuery();
                    ConsoleLogger.LogInfo("Suppression des mesures liées à l'identifiant " + id + ".");
                } catch { return ApiResponse<Any>.Error(ConsoleLogger.LogError("Impossible de supprimer les mesures liées à l'identifiant " + id + " !")); }


                string queryDevice = "DELETE FROM `appareil` WHERE `id_appareil` = @id_appareil";
                try {
                    using MySqlCommand command = new MySqlCommand(queryDevice, instance.Connection);
                    command.Parameters.AddWithValue("@id_appareil",  id);
                    command.ExecuteNonQuery();
                    ConsoleLogger.LogInfo("Suppression de l'appareil à l'identifiant " + id + ".");
                } catch { return ApiResponse<Any>.Error(ConsoleLogger.LogError("Impossible de supprimer l'appareil à l'identifiant " + id + " !")); }
                return ApiResponse<Any>.Success();
            } // void ..


            /// <summary>
            /// Supprime un type de mesure dans la base de donnée et touts les appareils et mesures associés.
            /// </summary>
            /// <param name="id"> L'identifiant d'un type de mesure. </param>
            /// <returns> Une réponse API générique. </returns>
            static ApiResponse<Any> DeleteMeasureTypeWithDevices(int id) {

                DBConnection instance = DBConnection.Instance();
                if (!instance.IsConnect())
                    instance.Connection?.Open();


                // Afin d'éviter la présence de clés étrangères pointant vers une référence nulle,
                // on supprime d'abord toutes les mesures associées à un type de mesure...
                string queryMeasure = "DELETE mesure FROM mesure JOIN appareil ON mesure.id_appareil = appareil.id_appareil WHERE appareil.id_type = @id_type";
                try {

                    using MySqlCommand command = new MySqlCommand(queryMeasure, instance.Connection);
                    command.Parameters.AddWithValue("@id_type", id);
                    command.ExecuteNonQuery();
                    ConsoleLogger.LogInfo("Suppression des mesures liées aux appareils de type " + id + ".");

                } catch { return ApiResponse<Any>.Error(ConsoleLogger.LogError("Impossible de supprimer les mesures liées aux appareils de type " + id + " !")); }


                // ... puis les appareils ...
                string queryDevice = "DELETE FROM `appareil` WHERE `id_type` = @id_type";
                try {

                    using MySqlCommand command = new MySqlCommand(queryDevice, instance.Connection);
                    command.Parameters.AddWithValue("@id_type",  id);
                    command.ExecuteNonQuery();
                    ConsoleLogger.LogInfo("Suppression de l'appareil au type de mesure " + id + ".");

                } catch { return ApiResponse<Any>.Error(ConsoleLogger.LogError("Impossible de supprimer l'appareil au type de mesure " + id + " !")); }


                // ... et enfin les types de mesure elles-même.
                string queryMeasureType = "DELETE FROM `type_mesure` WHERE `id_type` = @id_type";
                try {

                    using MySqlCommand command = new MySqlCommand(queryMeasureType, instance.Connection);
                    command.Parameters.AddWithValue("@id_type",  id);
                    command.ExecuteNonQuery();
                    ConsoleLogger.LogInfo("Suppression du type de mesure à l'identifiant " + id + ".");

                } catch { return ApiResponse<Any>.Error(ConsoleLogger.LogError("Impossible de supprimer le type de mesure à l'identifiant " + id + " !")); }
                return ApiResponse<Any>.Success();
            } // void ..
    } // class ..
} // namespace ..