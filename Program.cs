
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using sistemaVentas.Data;
var envFile = Path.Combine(
    Directory.GetCurrentDirectory(), ".env");

if (File.Exists(envFile))
{
    foreach (var linea in File.ReadAllLines(envFile))
    {
        var l = linea.Trim();

        if (l.Length == 0 || l.StartsWith('#') || !l.Contains('='))
            continue;

        var partes = l.Split('=', 2);

        Environment.SetEnvironmentVariable(
            partes[0].Trim(),
            partes[1].Trim().Trim('"'));
    }
}

CultureInfo.DefaultThreadCurrentCulture =
    CultureInfo.InvariantCulture;

CultureInfo.DefaultThreadCurrentUICulture =
    CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Credenciales del login
var loginEmail =
    Environment.GetEnvironmentVariable("LOGIN_EMAIL")?.Trim();

var loginPassword =
    Environment.GetEnvironmentVariable("LOGIN_PASSWORD");

if (string.IsNullOrWhiteSpace(loginEmail) ||
    string.IsNullOrWhiteSpace(loginPassword))
{
    throw new InvalidOperationException(
        "Configura LOGIN_EMAIL y LOGIN_PASSWORD.");
}

// Credenciales de Turso
var tursoUrl =
    Environment.GetEnvironmentVariable("TURSO_DATABASE_URL")?.Trim();

var tursoToken =
    Environment.GetEnvironmentVariable("TURSO_AUTH_TOKEN")?.Trim();

if (string.IsNullOrWhiteSpace(tursoUrl) ||
    string.IsNullOrWhiteSpace(tursoToken))
{
    throw new InvalidOperationException(
        "Configura TURSO_DATABASE_URL y TURSO_AUTH_TOKEN.");
}

if (!Uri.TryCreate(tursoUrl, UriKind.Absolute, out var uri) ||
    (uri.Scheme != Uri.UriSchemeHttps &&
     uri.Scheme != Uri.UriSchemeHttp))
{
    throw new InvalidOperationException(
        "La URL de conexión debe comenzar con https://.");
}

builder.Services.AddControllersWithViews();

// Autenticación mediante cookies
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
var connectionString =
    Microsoft.EntityFrameworkCore.LibSql.Connection
        .LibSqlConnectionStringBuilder.Create(tursoUrl, tursoToken);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLibSql(connectionString));

// Puerto para Railway
var port = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    try
    {
        Console.WriteLine("=== PRUEBA DE TURSO ===");

        var conectado = await db.Database.CanConnectAsync();

        Console.WriteLine($"Conexión: {conectado}");

        if (conectado)
        {
            var pendientes = await db.Database
                .GetPendingMigrationsAsync();

            Console.WriteLine(
                $"Migraciones pendientes: {pendientes.Count()}");

            foreach (var migracion in pendientes)
            {
                Console.WriteLine($"Pendiente: {migracion}");
            }
        }

        Console.WriteLine("=== FIN DE LA PRUEBA ===");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex}");
    }
}

// Aplicar migraciones a la base de datos configurada
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider
//        .GetRequiredService<AppDbContext>();

//    db.Database.Migrate();
//}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
