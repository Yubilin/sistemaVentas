using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using sistemaVentas.Data;
using sistemaVentas.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaVentas.Controllers;

public class VentasController : Controller
{
    private readonly AppDbContext _context;

    public VentasController(AppDbContext context)
    {
        _context = context;
    }

// Cargar los datos necesarios para la vista de ventas
    private async Task CargarDatosFormulario()
    {
        ViewBag.Clientes = await _context.Clientes
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        ViewBag.Productos = await _context.Producto
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }
    public async Task<IActionResult> Index()
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalles).ThenInclude(d => d.Producto)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();
        return View(ventas);
    }

// Mostrar el formulario de nueva venta
public async Task<IActionResult> Create()
{
    await CargarDatosFormulario();

    return View(new Venta());
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Venta venta)
{
    if (venta.ClienteId <= 0 ||
        !await _context.Clientes.AnyAsync(cliente => cliente.Id == venta.ClienteId))
    {
        ModelState.AddModelError(nameof(Venta.ClienteId), "Selecciona un cliente registrado.");
    }

    if (!ModelState.IsValid)
    {
        await CargarDatosFormulario();
        return View(venta);
    }

    venta.CalcularTotal();
    venta.CalcularCambio();
    venta.ActualizarStock();

    _context.Ventas.Add(venta);
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}

    public async Task<IActionResult> Details(int id)
    {
        var venta = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(v => v.Id == id);
            
        if (venta == null) return NotFound();
        return View(venta);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var venta = await _context.Ventas
            .Include(v => v.Cliente)
            .FirstOrDefaultAsync(v => v.Id == id);
            
        if (venta == null) return NotFound();
        return View(venta);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var venta = await _context.Ventas.FindAsync(id);
        if (venta != null)
        {
            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}