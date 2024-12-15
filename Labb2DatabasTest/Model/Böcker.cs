using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

public partial class Böcker
{
    public string Isbn13 { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Språk { get; set; } = null!;

    public decimal Pris { get; set; }

    public DateOnly Utgivardatum { get; set; }

    public virtual ICollection<BeställningsDetaljer> BeställningsDetaljers { get; set; } = new List<BeställningsDetaljer>();

    public virtual ICollection<Recensioner> Recensioners { get; set; } = new List<Recensioner>();

    public virtual ICollection<Författare> Författares { get; set; } = new List<Författare>();

    public virtual ICollection<LagerSaldo> LagerSaldos { get; set; } = new List<LagerSaldo>();
}
