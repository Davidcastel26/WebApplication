using Microsoft.EntityFrameworkCore;

using WebApplication.Repository;
using WebApplication.Repository.IRepository;
using WebApplication.Services;
using WebApplication.Services.Interfaces;

using AspNetWebApp = Microsoft.AspNetCore.Builder.WebApplication;

var builder = AspNetWebApp.CreateBuilder(args);

// DB conneciton
var dbConectionString = builder.Configuration.GetConnectionString("ConexionSql");

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConectionString));

// Repositorios
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Servicios
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<IDepositoService, DepositoService>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/openapi/v1.json", "BudgetApi v1");
    });
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
