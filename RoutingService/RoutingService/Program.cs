using SoapCore; // AJOUT SOAP
using RoutingService.Services; // Pour trouver tes classes

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- AJOUT SOAP 1 : On déclare le service ---
builder.Services.AddSingleton<IRoutingSoapService, RoutingSoapService>();
// ------------------------------------------

// CORS (Ton code existant)
builder.Services.AddCors(options => { /* ... */ });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

// --- AJOUT SOAP 2 : On expose l'URL du WSDL ---
// Le service sera accessible à : http://localhost:5132/RoutingService.asmx
app.UseSoapEndpoint<IRoutingSoapService>("/RoutingService.asmx", new SoapEncoderOptions());
// ----------------------------------------------

app.Run();