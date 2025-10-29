using DotNetEnv;
using censudex_clients_service.src.Data;
using Microsoft.EntityFrameworkCore;
using censudex_clients_service.src.Repositories;
using censudex_clients_service.src.Extensions;

Env.Load();
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("No DB_CONNECTION_STRING variable found.");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseNpgsql(connectionString)); 
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.InitializeDatabase();
app.Run();
