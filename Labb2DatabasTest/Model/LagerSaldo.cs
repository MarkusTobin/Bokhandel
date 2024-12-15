using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

public partial class LagerSaldo
{
    public int ButikId { get; set; }

    public string Isbn13 { get; set; } = null!;

    public int AntalBöckerKvar { get; set; }

    public virtual Butiker Butik { get; set; } = null!;
    public virtual Böcker Böckers { get; set; } = null!;
}
