using SoapCore;
using RoutingService.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Ajouter les services au conteneur
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CONFIGURATION CORS (INDISPENSABLE POUR LE FRONTEND) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// --- CONFIGURATION SOAP (POUR LE CLIENT JAVA) ---
builder.Services.AddSingleton<IRoutingSoapService, RoutingSoapService>();

var app = builder.Build();

// 2. Configurer le pipeline de requêtes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- ACTIVER CORS (DOIT ÊTRE PLACÉ AVANT MapControllers) ---
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// --- ENDPOINT SOAP ---
app.UseSoapEndpoint<IRoutingSoapService>("/RoutingService.asmx", new SoapEncoderOptions());

app.Run();