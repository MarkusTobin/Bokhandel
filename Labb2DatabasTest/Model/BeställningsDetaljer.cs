using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

public partial class BeställningsDetaljer
{
    public int BeställningsId { get; set; }

    public string Isbn13 { get; set; } = null!;

    public int Antal { get; set; }

    public virtual Beställningar Beställnings { get; set; } = null!;

    public virtual Böcker Isbn13Navigation { get; set; } = null!;
}
