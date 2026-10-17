using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistemaVentas.Data; //los subdirectorios se ingresan con punto
using sistemaVentas.Models;
using SQLitePCL;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace sistemaVentas.Controllers.Api;
[ApiController]
[Route("api/categorias")]
//este api controller valida un Json automaticamente 400 (error de servidor),nos sirve para devolver una lista y devolver
// en caso de funcionar dara status 200
public class CategoriasApiController: ControllerBase{
    private readonly AppDbContext _context;
    public CategoriasApiController(AppDbContext context)
    {
        _context = context;
    }
    //GET api/categorias 
    //async crear una tarea de solicitud para traer los datos formato de lista (Id,Nombre) toda la lista 
    [HttpGet]
    public async Task<ActionResult<List<Categoria>>> GetAll()
    {
        return await _context.Categorias.OrderBy(c=>c.Id).ToListAsync();//await es esperar una respuesta del servidor 
    }
    //GET api/Categorias/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Categoria>> GetById(int Id)
    {
        var categoria = await _context.Categorias.FindAsync(Id);//await es esperar una respuesta del servidor 
        if  (categoria == null)
            return NotFound();
        return categoria;
    }
    //----------------------------------CREAR----------------------------------------------
    //Post api/Categorias/
    //Body:{"nombre":"limpieza"}
    [HttpPost]
    public async Task<ActionResult<Categoria>> Create(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById),new{id=categoria.Id},categoria);
    }
    //----------------------------------ACTUALIZAR-------------------------------------------
    //metodo PUT /categorias/3
    //body {"id:3","nombre:bebidas"}
    [HttpPut("{id}")]
    public async Task<ActionResult> update(int ID,Categoria categoria)
    {
        if(ID != categoria.Id)
            return BadRequest("El Id de la URL no encontrado o no coincide con el cuerpo."); 
        if(!await _context.Categorias.AnyAsync(c=> c.Id == ID))
            return NotFound();
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync();
        return NoContent();        
    }
    //DELETE api/categoria/4
    //[HTTPDELETE ("id")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null)
            return NotFound();
        if (await _context.Producto.AnyAsync(p=>p.CategoriaId == id))
            return Conflict("No se puede eliminar: la categoria tiene productos");
        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}