using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using sistemaVentas.Data;

#nullable disable

namespace sistemaVentas.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261024023000_QuitarUsuarios")]
public partial class QuitarUsuarios : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TABLE IF EXISTS \"Usuarios\";");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}