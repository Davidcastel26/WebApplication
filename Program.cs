using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data;
using WebApplication.Repository;
using WebApplication.Repository.IRepository;
using WebApplication.Services;
using WebApplication.Services.Interfaces;

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

// CORS
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BudgetApi", Version = "v1" });
});

// ================== BUILD ==================
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "openapi/{documentName}.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "BudgetApi v1");
        c.RoutePrefix = "swagger";
    });
}


app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();
