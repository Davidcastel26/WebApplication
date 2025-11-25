using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Repository;
using WebApplication.Repository.IRepository;
using WebApplication.Services;
using WebApplication.Services.Interfaces;
// Alias por conflicto con tu namespace raíz "WebApplication"
using AspNetWebApp = Microsoft.AspNetCore.Builder.WebApplication;

var builder = AspNetWebApp.CreateBuilder(args);

// ================== CONFIG ==================
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
const string CorsPolicyName = "FrontendPolicy";

// ================== SERVICES (antes de Build) ==================
// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnectionString));

// Repositorios
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Servicios de dominio
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<IDepositoService, DepositoService>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Controllers (+ enums como string p/ enviar "Factura" en lugar de 1)
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// CORS (registro de la política)
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger (Swashbuckle: JSON + UI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ================== BUILD ==================
var app = builder.Build();

// ================== MIDDLEWARE ==================
if (app.Environment.IsDevelopment())
{
    // Sirve /swagger/v1/swagger.json y la UI en /swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usa la política CORS (después de Build, antes de MapControllers)
app.UseCors(CorsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();
