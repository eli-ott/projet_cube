using MySql.Data;
using MySql.Data.MySqlClient;

namespace Cube {
    public class Program {
        public static void Main(string[] args) {
            
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            int x = 0;
            app.MapGet("/", () => { x++; return "sd"; });

            app.Run();

        } // void ..
    } // class ..
} // namespace ..
