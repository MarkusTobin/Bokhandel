using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

public partial class Kunder
{
    public int Id { get; set; }

    public string Namn { get; set; } = null!;

    public string? Epost { get; set; }

    public string? Telefonnummer { get; set; }

    public virtual ICollection<Beställningar> Beställningars { get; set; } = new List<Beställningar>();

    public virtual ICollection<Recensioner> Recensioners { get; set; } = new List<Recensioner>();
}
