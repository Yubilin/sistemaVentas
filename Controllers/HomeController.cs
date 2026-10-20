using Microsoft.AspNetCore.Mvc;
using sistemaVentas.Models;

namespace sistemaVentas.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var modelo = new DashboardViewModel();
        return View(modelo);
    }

    public IActionResult Error()
    {
        return Content("Ocurrió un error. Intenta nuevamente.");
    }
}
