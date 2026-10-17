using Microsoft.AspNetCore.Mvc;

namespace sistemaVentas.Models;

public class Categoria
{
    //Atributos
    public int Id { get; set; }
    public string? Nombre { get; set; } = string.Empty;
    //Metodos
    public List<Producto> Productos { get; set ;} = new();
}