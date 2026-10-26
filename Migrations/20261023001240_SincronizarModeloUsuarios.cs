using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using sistemaVentas.Data;

#nullable disable

namespace sistemaVentas.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261023001240_SincronizarModeloUsuarios")]
public partial class SincronizarModeloUsuarios : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}