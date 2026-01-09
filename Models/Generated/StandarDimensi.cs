using System;
using System.Collections.Generic;

namespace AplikasiCheckDimensi.Models.Generated;

public partial class StandarDimensi
{
    public int Id { get; set; }

    public int ProdukId { get; set; }

    public string NamaDimensi { get; set; } = null!;

    public string? DimensiAMin { get; set; }

    public string? DimensiAMax { get; set; }

    public string? DimensiBMin { get; set; }

    public string? DimensiBMax { get; set; }

    public string? SudutMin { get; set; }

    public string? SudutMax { get; set; }

    public decimal? InnerDiameterMax { get; set; }

    public decimal? InnerDiameterMin { get; set; }

    public decimal? OuterDiameterMax { get; set; }

    public decimal? OuterDiameterMin { get; set; }

    public string? PanjangMax { get; set; }

    public string? PanjangMin { get; set; }

    public string? RadiusMax { get; set; }

    public string? RadiusMin { get; set; }

    public string? ThicknessMax { get; set; }

    public string? ThicknessMin { get; set; }

    public string? TinggiMax { get; set; }

    public string? TinggiMin { get; set; }

    public virtual ICollection<InputAktual> InputAktuals { get; set; } = new List<InputAktual>();

    public virtual Produk Produk { get; set; } = null!;
}
