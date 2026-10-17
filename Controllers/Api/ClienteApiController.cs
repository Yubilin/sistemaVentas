using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistemaVentas.Data; //los subdirectorios se ingresan con punto
using sistemaVentas.Models;
using SQLitePCL;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace SistemaVentas.Controllers.Api;

//este api controller valida un Json automaticamente 400 (error de servidor),nos sirve para devolver una lista y devolver
// en caso de funcionar dara status 200
public class ClienteApiController: ControllerBase{
    private readonly AppDbContext _context;
    public ClienteApiController(AppDbContext context)
    {
        _context = context;
    }
    //GET api/clientes 
    //async crear una tarea de solicitud para traer los datos formato de lista (Id,Nombre) toda la lista 
    public async Task<ActionResult<List<Cliente>>> GetAll()
    {
        return await _context.Clientes.OrderBy(c=>c.Nombre).ToListAsync();//await es esperar una respuesta del servidor 
    }
    //GET api/Clientes/1
    public async Task<ActionResult<Cliente>> GetById(int Id)
    {
        var clientes = await _context.Clientes.FindAsync(Id);//await es esperar una respuesta del servidor 
        if  (clientes == null)
            return NotFound();
        return clientes;
    }
    //----------------------------------CREAR----------------------------------------------
    //Post api/Ctaegorias/
    //Body:{"nombre":"limpieza"}
    public async Task<ActionResult<Cliente>> Create(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById),new{id=cliente.Id},cliente);
    }
    //----------------------------------ACTUALIAR-------------------------------------------
    //metodo PUT /clientes/3
    //body {"id:3","nombre:bebidas"}
    public async Task<ActionResult> update(int ID,Cliente cliente)
    {
        if(ID != cliente.Id)
            return BadRequest("El Id de la URL no encontrado o no coincide con el cuerpo."); 
        if(!await _context.Clientes.AnyAsync(c=> c.Id == ID))
            return NotFound();
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
        return NoContent();        
    }
    //DELETE api/cliente/4
    //[HTTPDELETE ("id")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            return NotFound();

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}