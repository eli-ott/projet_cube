using MySql.Data.MySqlClient;

namespace Cube.Data {
    public class DBConnection {

        //=====================
        // D E F I N I T I O N
        //=====================

            /** <summary> Nom du serveur. </summary> **/                   public string? Server;
            /** <summary> Nom de la base de donnée. </summary> **/         public string? DatabaseName;
            /** <summary> Nom de l'administrateur. </summary> **/          public string? UserName;
            /** <summary> Mot de passe de l'administrateur. </summary> **/ public string? Password;

            /** <summary> Reference de la connexion au serveur. </summary> **/ public MySqlConnection? Connection { get; set;}

            /** <summary> Instance de connexion au serveur. </summary> **/ private static DBConnection? _instance = null;


        //=============================
        // I M P L E M E N T A T I O N
        //=============================

            /// <summary>
            /// Permet d'accéder à l'instance de connexion et de la créer si nécessaire.
            /// </summary>
            /// <returns> L'instance de connexion. </returns>
            public static DBConnection Instance()  {
                DBConnection. _instance ??= new DBConnection();
                return DBConnection._instance;
            } // DBConnection ..

    
            /// <summary>
            /// Indique si la connexion est établie.
            /// </summary>
            /// <returns> `true` quand la connexion est établie. </returns>
            public bool IsConnect() {
                if (this.Connection == null) {

                    if (string.IsNullOrEmpty(this.DatabaseName))
                        return false;
                    
                    this.Connection = new MySqlConnection(string.Format("Server={0}; database={1}; UID={2}; password={3}", Server, DatabaseName, UserName, Password));
                    this.Connection.Open();

                    ConsoleLogger.LogInfo("Connexion avec succès avec la base de donnée.");

                } // if ..
        
                return true;
                
            } // bool ..

    
            /// <summary>
            /// Permet de fermer la connexion.
            /// </summary>
            public void Close() {
                this.Connection?.Close();
                ConsoleLogger.LogWarning("Connexion avec la base de donnée interrompue avec succès.");
            } // void ..
    } // class ..
} // namespace ..
