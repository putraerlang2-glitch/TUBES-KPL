using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ObatAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "obat",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nama = table.Column<string>(type: "VARCHAR(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kategori = table.Column<string>(type: "VARCHAR(100)", nullable: true, defaultValue: "Tablet")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    stok = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    harga = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0.00m),
                    expired_date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    status = table.Column<string>(type: "VARCHAR(50)", nullable: true, defaultValue: "Available")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_obat", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transaksi",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    no_struk = table.Column<string>(type: "VARCHAR(50)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tanggal_transaksi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    persentase_diskon = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    nominal_diskon = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    persentase_pajak = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    nominal_pajak = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    total_akhir = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    uang_bayar = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    uang_kembalian = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaksi", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "transaksi_detail",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    transaksi_id = table.Column<int>(type: "int", nullable: false),
                    obat_id = table.Column<int>(type: "int", nullable: false),
                    jumlah = table.Column<int>(type: "int", nullable: false),
                    harga_satuan = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transaksi_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_transaksi_detail_obat_obat_id",
                        column: x => x.obat_id,
                        principalTable: "obat",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_transaksi_detail_transaksi_transaksi_id",
                        column: x => x.transaksi_id,
                        principalTable: "transaksi",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_created_at",
                table: "obat",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_expired_date",
                table: "obat",
                column: "expired_date");

            migrationBuilder.CreateIndex(
                name: "idx_status",
                table: "obat",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_transaksi_detail_obat_id",
                table: "transaksi_detail",
                column: "obat_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaksi_detail_transaksi_id",
                table: "transaksi_detail",
                column: "transaksi_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transaksi_detail");

            migrationBuilder.DropTable(
                name: "transaksi");

            migrationBuilder.DropTable(
                name: "obat");
        }
    }
}
