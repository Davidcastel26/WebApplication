using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FondoMonetario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoFondo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroCuenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FondoMonetario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoGasto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoGasto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deposito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FondoMonetarioId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposito", x => x.Id);
                    table.CheckConstraint("CK_Deposito_MontoPositivo", "[Monto] > 0");
                    table.ForeignKey(
                        name: "FK_Deposito_FondoMonetario_FondoMonetarioId",
                        column: x => x.FondoMonetarioId,
                        principalTable: "FondoMonetario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GastoEncabezado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FondoMonetarioId = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NombreComercio = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GastoEncabezado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GastoEncabezado_FondoMonetario_FondoMonetarioId",
                        column: x => x.FondoMonetarioId,
                        principalTable: "FondoMonetario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Presupuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    TipoGastoId = table.Column<int>(type: "int", nullable: false),
                    MontoPresupuestado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuesto", x => x.Id);
                    table.CheckConstraint("CK_Presupuesto_Mes", "[Mes] BETWEEN 1 AND 12");
                    table.CheckConstraint("CK_Presupuesto_MontoPositivo", "[MontoPresupuestado] >= 0");
                    table.ForeignKey(
                        name: "FK_Presupuesto_TipoGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TipoGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Presupuesto_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GastoDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GastoEncabezadoId = table.Column<int>(type: "int", nullable: false),
                    TipoGastoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GastoDetalle", x => x.Id);
                    table.CheckConstraint("CK_GastoDetalle_MontoPositivo", "[Monto] > 0");
                    table.ForeignKey(
                        name: "FK_GastoDetalle_GastoEncabezado_GastoEncabezadoId",
                        column: x => x.GastoEncabezadoId,
                        principalTable: "GastoEncabezado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GastoDetalle_TipoGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TipoGasto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deposito_FondoMonetarioId",
                table: "Deposito",
                column: "FondoMonetarioId");

            migrationBuilder.CreateIndex(
                name: "IX_FondoMonetario_Nombre",
                table: "FondoMonetario",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_GastoDetalle_GastoEncabezadoId",
                table: "GastoDetalle",
                column: "GastoEncabezadoId");

            migrationBuilder.CreateIndex(
                name: "IX_GastoDetalle_TipoGastoId",
                table: "GastoDetalle",
                column: "TipoGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_GastoEncabezado_FondoMonetarioId",
                table: "GastoEncabezado",
                column: "FondoMonetarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuesto_Anio_Mes_TipoGastoId_UsuarioId",
                table: "Presupuesto",
                columns: new[] { "Anio", "Mes", "TipoGastoId", "UsuarioId" },
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuesto_TipoGastoId",
                table: "Presupuesto",
                column: "TipoGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuesto_UsuarioId",
                table: "Presupuesto",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoGasto_Codigo",
                table: "TipoGasto",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deposito");

            migrationBuilder.DropTable(
                name: "GastoDetalle");

            migrationBuilder.DropTable(
                name: "Presupuesto");

            migrationBuilder.DropTable(
                name: "GastoEncabezado");

            migrationBuilder.DropTable(
                name: "TipoGasto");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "FondoMonetario");
        }
    }
}
