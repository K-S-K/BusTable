using BusTable.Service.Settings;

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
