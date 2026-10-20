
namespace sistemaVentas.Models
{
    public class DashboardViewModel
    {
        public decimal VentasHoy { get; set; }
        public decimal VentasMes { get; set; }
        public decimal VentasAnio { get; set; }
        public decimal TicketPromedio { get; set; }

        public int TotalClientes { get; set; }
        public int ProductosAgotados { get; set; }
        public int ProductosStockBajo { get; set; }

        public List<VentaCategoriaItem> VentasPorCategoria { get; set; } = new();
        public List<ProductoVendidoItem> ProductosMasVendidos { get; set; } = new();
        public List<UltimaVentaItem> UltimasVentas { get; set; } = new();
        public int DiasSeleccionados { get; set; } = 7;
        public List<string> Alertas { get; set; } = new();
            public List<string> EtiquetasDias { get; set; } = new();
    public List<decimal> VentasPorDia { get; set; } = new();
    }

    public class VentaCategoriaItem
    {
        public string Categoria { get; set; } = "";
        public decimal Total { get; set; }
    }

    public class ProductoVendidoItem
    {
        public string Producto { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }

    public class UltimaVentaItem
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = "";
        public decimal Total { get; set; }
    }
}
