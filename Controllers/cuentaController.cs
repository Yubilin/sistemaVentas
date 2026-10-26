
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sistemaVentas.Models;

namespace SistemaVentas.Controllers;

public class CuentaController : Controller
{
    private readonly string _loginEmail;
    private readonly string _loginPassword;

    public CuentaController(IConfiguration configuration)
    {
        _loginEmail = configuration["LOGIN_EMAIL"]?.Trim() ?? string.Empty;
        _loginPassword = configuration["LOGIN_PASSWORD"] ?? string.Empty;
    }

    // Mostrar el formulario de inicio de sesión
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    // Procesar el formulario de inicio de sesión
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        if (!string.Equals(
                NormalizarEmail(model.Email),
                NormalizarEmail(_loginEmail),
                StringComparison.Ordinal)
            || !CoincideContrasena(model.Password, _loginPassword))
        {
            ModelState.AddModelError(
                string.Empty,
                "Correo o contraseña incorrectos.");

            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "login-temporal"),
            new(ClaimTypes.Name, _loginEmail),
            new(ClaimTypes.Email, _loginEmail)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = model.Recordarme,
                ExpiresUtc = model.Recordarme
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : null,
                AllowRefresh = !model.Recordarme
            });

        if (!string.IsNullOrWhiteSpace(returnUrl)
            && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    // Página de acceso denegado
    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        return Content(
            "No tienes permiso para acceder a esta sección.");
    }

    // Cerrar sesión
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    private static string NormalizarEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private static bool CoincideContrasena(string ingresada, string configurada)
    {
        var ingresadaHash = SHA256.HashData(Encoding.UTF8.GetBytes(ingresada));
        var configuradaHash = SHA256.HashData(Encoding.UTF8.GetBytes(configurada));

        return CryptographicOperations.FixedTimeEquals(ingresadaHash, configuradaHash);
    }
}
