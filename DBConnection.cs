using MySql.Data.MySqlClient;

namespace Cube.Data {
    public class DBConnection {

        private DBConnection() {}

        public string? Server;
        public string? DatabaseName;
        public string? UserName;
        public string? Password;

        public MySqlConnection? Connection { get; set;}

        private static DBConnection? _instance = null;

        public static DBConnection Instance()  {
           DBConnection. _instance ??= new DBConnection();
           return _instance;
        } // DBConnection ..
    
        public bool IsConnect() {
            if (Connection == null) {

                if (String.IsNullOrEmpty(DatabaseName))
                    return false;
                
                Connection = new MySqlConnection(string.Format("Server={0}; database={1}; UID={2}; password={3}", Server, DatabaseName, UserName, Password));
                Connection.Open();

            } // if ..
    
            return true;
        } // bool ..
    
        public void Close() => this.Connection?.Close();

    } // class ..
} // namespace ..
