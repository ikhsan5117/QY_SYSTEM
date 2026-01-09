using System;
using System.Collections.Generic;

namespace AplikasiCheckDimensi.Models.Generated;

public partial class Produk
{
    public int Id { get; set; }

    public string NamaProduk { get; set; } = null!;

    public string Operator { get; set; } = null!;

    public DateTime TanggalInput { get; set; }

    public virtual ICollection<StandarDimensi> StandarDimensis { get; set; } = new List<StandarDimensi>();
}
