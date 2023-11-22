using MySql.Data;
using MySql.Data.MySqlClient;

namespace Cube {
    public class Program {
        public static void Main(string[] args) {
            
            var SpecialOrigin = "special_origin";
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(option => {
                option.AddPolicy(name: SpecialOrigin, policy => {
                    policy.WithOrigins("http://127.0.0.1:5500", "http://127.0.0.1:5023");
                });
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            int x = 0;
            app.MapGet("/", () => { x++; return "sd"; });

            app.UseCors(SpecialOrigin);

            app.Run();

        } // void ..
    } // class ..
} // namespace ..
