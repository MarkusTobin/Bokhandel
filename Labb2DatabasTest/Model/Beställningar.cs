using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

public partial class Beställningar
{
    public int BeställningId { get; set; }

    public int KundId { get; set; }

    public DateOnly Datum { get; set; }

    public string? CurrentStatus { get; set; }

    public decimal TotalPris { get; set; }

    public int ButikId { get; set; }

    public virtual ICollection<BeställningsDetaljer> BeställningsDetaljers { get; set; } = new List<BeställningsDetaljer>();

    public virtual Butiker Butik { get; set; } = null!;

    public virtual Kunder Kund { get; set; } = null!;
}
