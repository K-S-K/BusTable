using BusTable.Service.Settings;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

var importSourceSettings = builder.Configuration.GetSection("ImportSource").Get<ImportSourceSettings>();

// Add services to the container.
builder.Services.AddSingleton<BusTable.Service.Services.LanguageValidationService>();
builder.Services.AddSingleton<BusTable.Service.Services.DataTransferProviderService>();
builder.Services.AddSingleton<BusTable.Service.Services.RouteService>();
builder.Services.AddSingleton<BusTable.Service.Services.StopService>();
builder.Services.AddSingleton<BusTable.Service.Services.ImportService>(sp =>
                              new BusTable.Service.Services.ImportService(importSourceSettings));

builder.Services.AddControllers();

// https://github.com/dotnet/aspnet-api-versioning/wiki/Versioning-by-Header
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ApiVersionReader = new HeaderApiVersionReader("BusTable-API-Version");
    options.AssumeDefaultVersionWhenUnspecified = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
