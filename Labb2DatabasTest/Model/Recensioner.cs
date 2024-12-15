using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

public partial class Recensioner
{
    public int Id { get; set; }

    public int KundId { get; set; }

    public string Isbn13 { get; set; } = null!;

    public int Betyg { get; set; }

    public virtual Böcker Isbn13Navigation { get; set; } = null!;

    public virtual Kunder Kund { get; set; } = null!;
}
