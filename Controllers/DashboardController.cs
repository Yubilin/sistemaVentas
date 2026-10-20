using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistemaVentas.Data;
using sistemaVentas.Models;

namespace sistemaVentas.Controllers;

public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int dias = 7)
    {
        // Solo permitimos 7, 15 o 30 días.
        if (dias != 7 && dias != 15 && dias != 30)
            dias = 7;

        var hoy = DateTime.Today;
        var manana = hoy.AddDays(1);
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        var inicioAnio = new DateTime(hoy.Year, 1, 1);

        var ventas = _context.Ventas.AsNoTracking();

        var modelo = new DashboardViewModel
        {
            DiasSeleccionados = dias,

            VentasHoy = await ventas
                .Where(v => v.Fecha >= hoy && v.Fecha < manana)
                .SumAsync(v => (decimal?)v.Total) ?? 0m,

            VentasMes = await ventas
                .Where(v => v.Fecha >= inicioMes && v.Fecha < manana)
                .SumAsync(v => (decimal?)v.Total) ?? 0m,

            VentasAnio = await ventas
                .Where(v => v.Fecha >= inicioAnio && v.Fecha < manana)
                .SumAsync(v => (decimal?)v.Total) ?? 0m,

            TotalClientes = await _context.Clientes
                .CountAsync(),

            ProductosStockBajo = await _context.Producto
                .CountAsync(p =>
                    p.Stock.HasValue &&
                    p.StockMinimo.HasValue &&
                    p.Stock <= p.StockMinimo &&
                    p.Stock > 0),

            ProductosAgotados = await _context.Producto
                .CountAsync(p => p.Stock.HasValue && p.Stock <= 0)
        };

        // Ticket promedio del mes: total vendido / número de ventas.
        var ventasMesQuery = ventas.Where(v =>
            v.Fecha >= inicioMes && v.Fecha < manana);

        var cantidadVentasMes = await ventasMesQuery.CountAsync();

        modelo.TicketPromedio = cantidadVentasMes > 0
            ? modelo.VentasMes / cantidadVentasMes
            : 0m;

        // Obtener las ventas del período seleccionado.
        var inicioGrafico = hoy.AddDays(-(dias - 1));

        var resumenDiario = await ventas
            .Where(v => v.Fecha >= inicioGrafico && v.Fecha < manana)
            .GroupBy(v => v.Fecha.Date)
            .Select(g => new
            {
                Fecha = g.Key,
                Total = g.Sum(v => v.Total)
            })
            .ToListAsync();

        // Incluimos también los días sin ventas.
        for (int i = 0; i < dias; i++)
        {
            var fecha = inicioGrafico.AddDays(i);
            var registro = resumenDiario
                .FirstOrDefault(r => r.Fecha == fecha);

            modelo.EtiquetasDias.Add(fecha.ToString("dd/MM"));
            modelo.VentasPorDia.Add(registro?.Total ?? 0m);
        }

        // Alertas de inventario.
        if (modelo.ProductosAgotados > 0)
            modelo.Alertas.Add(
                $"Hay {modelo.ProductosAgotados} productos agotados.");

        if (modelo.ProductosStockBajo > 0)
            modelo.Alertas.Add(
                $"Hay {modelo.ProductosStockBajo} productos con stock bajo.");

        if (modelo.Alertas.Count == 0)
            modelo.Alertas.Add("No hay alertas de inventario.");

        return View(modelo);
    }
}