var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/add", () => "Ajout d'une nouvelle info");

app.Run();
