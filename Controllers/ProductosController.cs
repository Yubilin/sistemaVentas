
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    // Cargar categorías en el desplegable
    private async Task CargarCategorias()
    {
        ViewBag.CategoriaId = new SelectList(await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync(),
        nameof(Categoria.Id),
        nameof(Categoria.Nombre)
        );
    }

    // GET: Productos
    public async Task<IActionResult> Index()
    {
        var productos = await _context.Producto
            .Include(p => p.Categoria)
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        return View(productos);
    }

    // GET: Productos/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarCategorias();
        return View(new Producto());
    }

    // POST: Productos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Producto producto)
    {
        if (!ModelState.IsValid)
        {
            await CargarCategorias();
            return View(producto);
        }

        _context.Producto.Add(producto);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Productos/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var producto = await _context.Producto.FindAsync(id);

        if (producto == null)
            return NotFound();

        await CargarCategorias();

        return View(producto);
    }

    // POST: Productos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Producto producto)
    {
        if (id != producto.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await CargarCategorias();
            return View(producto);
        }

        _context.Producto.Update(producto);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: Productos/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var producto = await _context.Producto
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (producto == null)
            return NotFound();

        return View(producto);
    }

    // POST: Productos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var producto = await _context.Producto.FindAsync(id);

        if (producto != null)
        {
            _context.Producto.Remove(producto);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
