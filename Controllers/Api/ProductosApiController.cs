
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistemaVentas.Data;
using sistemaVentas.Models;
namespace sistemaVentas.Controllers.Api

{
    public class ProductosApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ProductosApi
        // Obtener todos los productos
        [HttpGet]
        public async Task<ActionResult<List<Producto>>> GetAll()
        {
            return await _context.Producto.OrderBy(p => p.Nombre).ToListAsync();
        }

        // GET: api/ProductosApi/1
        // Obtener un producto por su ID
        public async Task<ActionResult<Producto>> GetById(int id)
        {
            var producto = await _context.Producto.FindAsync(id);

            if (producto == null)
                return NotFound();

            return producto;
        }

        // POST: api/ProductosApi
        // Crear un producto
        public async Task<ActionResult<Producto>> Create(Producto producto)
        {
            _context.Producto.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),new { id = producto.Id },producto);
        }

        // PUT: api/ProductosApi/1
        // Actualizar un producto
        public async Task<IActionResult> Update(int id, Producto producto)
        {
            if (id != producto.Id)
                return BadRequest(
                    "El ID de la URL no coincide con el ID del producto."
                );

            var existe = await _context.Producto
                .AnyAsync(p => p.Id == id);

            if (!existe)
                return NotFound();

            _context.Producto.Update(producto);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ProductosApi/1
        // Eliminar un producto
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.Producto.FindAsync(id);

            if (producto == null)
                return NotFound();

            _context.Producto.Remove(producto);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
