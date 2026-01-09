using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AplikasiCheckDimensi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaProduk = table.Column<string>(type: "TEXT", nullable: false),
                    PartNo = table.Column<string>(type: "TEXT", nullable: true),
                    PartCode = table.Column<string>(type: "TEXT", nullable: true),
                    CT = table.Column<string>(type: "TEXT", nullable: true),
                    Plant = table.Column<string>(type: "TEXT", nullable: true),
                    IdentifikasiItem = table.Column<string>(type: "TEXT", nullable: true),
                    Operator = table.Column<string>(type: "TEXT", nullable: false),
                    TanggalInput = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TypeBox = table.Column<string>(type: "TEXT", nullable: true),
                    QtyPerBox = table.Column<int>(type: "INTEGER", nullable: true),
                    GambarPacking = table.Column<string>(type: "TEXT", nullable: true),
                    StandarCF = table.Column<string>(type: "TEXT", nullable: true),
                    GambarCF = table.Column<string>(type: "TEXT", nullable: true),
                    VideoPath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produk", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    NamaLengkap = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Plant = table.Column<string>(type: "TEXT", nullable: false),
                    Grup = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    TanggalDibuat = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StandarDimensi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProdukId = table.Column<int>(type: "INTEGER", nullable: false),
                    NamaDimensi = table.Column<string>(type: "TEXT", nullable: false),
                    InnerDiameter_SisiA_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    InnerDiameter_SisiA_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    OuterDiameter_SisiA_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    OuterDiameter_SisiA_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    Thickness_SisiA_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    Thickness_SisiA_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    InnerDiameter_SisiB_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    InnerDiameter_SisiB_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    OuterDiameter_SisiB_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    OuterDiameter_SisiB_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    Thickness_SisiB_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    Thickness_SisiB_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    Panjang_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    Panjang_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    Tinggi_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    Tinggi_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    Radius_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    Radius_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    InnerDiameter_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    InnerDiameter_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    OuterDiameter_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    OuterDiameter_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    Thickness_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    Thickness_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    DimensiA_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    DimensiA_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    DimensiB_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    DimensiB_Max = table.Column<decimal>(type: "TEXT", nullable: true),
                    Sudut_Min = table.Column<decimal>(type: "TEXT", nullable: true),
                    Sudut_Max = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandarDimensi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandarDimensi_Produk_ProdukId",
                        column: x => x.ProdukId,
                        principalTable: "Produk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InputAktual",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StandarDimensiId = table.Column<int>(type: "INTEGER", nullable: false),
                    NilaiInnerDiameter_SisiA = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiInnerDiameter_SisiAX = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiInnerDiameter_SisiAY = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiOuterDiameter_SisiA = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiOuterDiameter_SisiAX = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiOuterDiameter_SisiAY = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiThickness_SisiA = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiThickness_SisiAX = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiThickness_SisiAY = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiInnerDiameter_SisiB = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiInnerDiameter_SisiBX = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiInnerDiameter_SisiBY = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiOuterDiameter_SisiB = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiOuterDiameter_SisiBX = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiOuterDiameter_SisiBY = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiThickness_SisiB = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiThickness_SisiBX = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiThickness_SisiBY = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiInnerDiameter = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiOuterDiameter = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiThickness = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiPanjang = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiTinggi = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiRadius = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiDimensiA = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiDimensiB = table.Column<decimal>(type: "TEXT", nullable: true),
                    NilaiSudut = table.Column<decimal>(type: "TEXT", nullable: true),
                    TanggalInput = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CatatanOperator = table.Column<string>(type: "TEXT", nullable: true),
                    NamaPIC = table.Column<string>(type: "TEXT", nullable: true),
                    Plant = table.Column<string>(type: "TEXT", nullable: true),
                    Grup = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputAktual", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InputAktual_StandarDimensi_StandarDimensiId",
                        column: x => x.StandarDimensiId,
                        principalTable: "StandarDimensi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InputAktual_StandarDimensiId",
                table: "InputAktual",
                column: "StandarDimensiId");

            migrationBuilder.CreateIndex(
                name: "IX_StandarDimensi_ProdukId",
                table: "StandarDimensi",
                column: "ProdukId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InputAktual");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "StandarDimensi");

            migrationBuilder.DropTable(
                name: "Produk");
        }
    }
}
