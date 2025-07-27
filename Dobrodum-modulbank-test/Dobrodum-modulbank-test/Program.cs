using Dobrodum_modulbank_test;
var builder = WebApplication.CreateBuilder(args);
var appContext = new ApplicationSQliteContext();

if (Convert.ToUInt32(builder.Configuration["fieldSize"]) < Convert.ToUInt32(builder.Configuration["winningLenght"]))
    throw new ArgumentException("Размер поля не может быть меньше длинны выигрышной линии");


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(appContext);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Эндпоинт для проверки переменных
    app.MapGet("/env", (IConfiguration config) =>
    {
        return Results.Json(
            new Dictionary<string, string> 
            {
                { "fieldSizeConfig", config["fieldSize"] }
            }
        );
    });
}

app.MapControllers();

app.Run();

