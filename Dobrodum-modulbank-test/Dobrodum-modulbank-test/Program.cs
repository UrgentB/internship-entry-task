using Dobrodum_modulbank_test.Services;

var builder = WebApplication.CreateBuilder(args);
var appContext = new ApplicationSQliteContext();

if (Convert.ToUInt32(builder.Configuration["fieldSize"]) < Convert.ToUInt32(builder.Configuration["winningLenght"]))
    throw new ArgumentException("������ ���� �� ����� ���� ������ ������ ���������� �����");


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

    // �������� ��� �������� ����������
    app.MapGet("/health", () => "��������");
}

app.MapControllers();

app.Run();

