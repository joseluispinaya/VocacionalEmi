using Capa.Web.Data;
using Capa.Web.Helpers;
using Capa.Web.Repositories.Implementations;
using Capa.Web.Repositories.Intefaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("LocalConnection");

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<SeedDb>();
builder.Services.AddScoped<IFileStorage, FileStorage>();

builder.Services.AddScoped<IUnidadesEduRepository, UnidadesEduRepository>();
builder.Services.AddScoped<IEstudiantesRepository, EstudiantesRepository>();

var app = builder.Build();

await SeedDataAsync(app);

// Convertimos el método local a asíncrono
async Task SeedDataAsync(WebApplication app)
{
    // Forma simplificada de crear el scope
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var service = services.GetRequiredService<SeedDb>();
        // Usamos await real, no .Wait()
        await service.SeedAsync();
    }
    catch
    {
        throw;
        // Opcional: Loguear el error para saber qué pasó si falla el seed throw;
        //var logger = services.GetRequiredService<ILogger<Program>>();
        //logger.LogError(ex, "Ocurrió un error al ejecutar el SeedDb.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
