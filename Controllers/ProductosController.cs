using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistemaVentas.Data;
using sistemaVentas.Models;

namespace sistemaVentas.Controllers;

public class ProductosController : Controller
{
    private readonly AppDbContext _context;

    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Categorias
    public async Task<IActionResult> Index()
    {
        return View(await _context.Producto.OrderBy(c => c.Nombre).ToListAsync());
    }

    // GET: /Categorias/Create
    public IActionResult Create()
    {
        return View(new Producto());
    }

    // POST: /Categorias/Create
    [HttpPost]
    public async Task<IActionResult> Create(Producto producto)
    {
        if (!ModelState.IsValid) return View(producto);

        _context.Producto.Add(producto);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Categorias/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var producto = await _context.Producto.FindAsync(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    // POST: /Categorias/Edit/5
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Producto producto)
    {
        if (id != producto.Id) return NotFound();
        if (!ModelState.IsValid) return View(producto);

        _context.Producto.Update(producto);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Categorias/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var producto = await _context.Producto.FindAsync(id);
        if (producto == null) return NotFound();
        return View(producto);
    }

    // POST: /Categorias/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var producto = await _context.Producto.FindAsync(id);
        if (producto != null)
        {
            // No se puede eliminar una categoría que tiene productos
            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
