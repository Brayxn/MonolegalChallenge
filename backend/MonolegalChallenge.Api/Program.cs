using MonolegalChallenge.Domain;
using MonolegalChallenge.Infrastructure;
using MonolegalChallenge.Application;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Configuración de MongoDB
var mongoConfig = builder.Configuration.GetSection("MongoDb");
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConfig["ConnectionString"]));
builder.Services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoConfig["Database"]));


// Configuración SMTP
// Bind SMTP settings from configuration (supports appsettings.json and environment variables)
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<MonolegalChallenge.Infrastructure.SmtpSettings>()
                   ?? new MonolegalChallenge.Infrastructure.SmtpSettings();
builder.Services.AddSingleton(smtpSettings);
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICorreoHistorialRepository, CorreoHistorialRepository>();
builder.Services.AddScoped<CorreoHistorialService>();
builder.Services.AddScoped<FacturaAutoDesactivacionService>(sp =>
    new FacturaAutoDesactivacionService(
        sp.GetRequiredService<IFacturaRepository>(),
        sp.GetRequiredService<IClienteRepository>(),
        sp.GetRequiredService<IEmailService>(),
        sp.GetRequiredService<CorreoHistorialService>()
    )
);
// Configuración de tiempo de espera para desactivación
var tiempoEsperaDesactivacionMin = builder.Configuration.GetValue<int?>("TiempoEsperaDesactivacionMin") ?? 2880; // 2880 min = 48h por defecto
builder.Services.AddScoped<FacturaService>(sp =>
    new FacturaService(
        sp.GetRequiredService<IFacturaRepository>(),
        sp.GetRequiredService<IClienteRepository>(),
        sp.GetRequiredService<IEmailService>(),
        sp.GetRequiredService<CorreoHistorialService>(),
        TimeSpan.FromMinutes(tiempoEsperaDesactivacionMin)
    )
);

// Configuración de intervalo de automatización
var intervaloAutomatizacionMin = builder.Configuration.GetValue<int?>("IntervaloAutomatizacionMin") ?? 1;
builder.Services.AddHostedService(sp =>
    new FacturaBackgroundService(sp, TimeSpan.FromMinutes(intervaloAutomatizacionMin))
);
builder.Services.AddScoped<ClienteService>();


// Add Swagger/OpenAPI support
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
app.UseCors();
app.MapControllers();

app.Run();
