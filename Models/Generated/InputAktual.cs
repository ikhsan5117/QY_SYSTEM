using System;
using System.Collections.Generic;

namespace AplikasiCheckDimensi.Models.Generated;

public partial class InputAktual
{
    public int Id { get; set; }

    public int StandarDimensiId { get; set; }

    public string? NilaiDimensiA { get; set; }

    public string? NilaiDimensiB { get; set; }

    public string? NilaiSudut { get; set; }

    public DateTime TanggalInput { get; set; }

    public string? CatatanOperator { get; set; }

    public decimal? NilaiInnerDiameter { get; set; }

    public decimal? NilaiOuterDiameter { get; set; }

    public string? NilaiPanjang { get; set; }

    public string? NilaiRadius { get; set; }

    public string? NilaiThickness { get; set; }

    public string? NilaiTinggi { get; set; }

    public virtual StandarDimensi StandarDimensi { get; set; } = null!;
}
