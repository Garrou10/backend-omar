using BackendApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders; // NY: Krävs för att peka ut filer

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// --- NY KOD FÖR ATT SERVA BILDER KORREKT ---
// 1. Skapa den fysiska sökvägen till wwwroot/uploads
var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

// 2. Om mappen inte finns ännu (t.ex. vid nystart), skapa den
if (!Directory.Exists(uploadsFolder))
{
    Directory.CreateDirectory(uploadsFolder);
}

// 3. Tvinga servern att koppla "/uploads" i webbläsaren till den fysiska mappen
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsFolder),
    RequestPath = "/uploads"
});
// -------------------------------------------

app.MapControllers();

app.Run();