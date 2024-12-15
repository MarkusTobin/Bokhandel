using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

public partial class Butiker
{
    public int Id { get; set; }

    public string Butiksnamn { get; set; } = null!;

    public string Adress { get; set; } = null!;

    public string Stad { get; set; } = null!;

    public string Postnummer { get; set; } = null!;

    public string Land { get; set; } = null!;

    public string? Hemsida { get; set; }

    public virtual ICollection<Anställda> Anställda { get; set; } = new List<Anställda>();

    public virtual ICollection<Beställningar> Beställningars { get; set; } = new List<Beställningar>();

    public virtual ICollection<LagerSaldo> LagerSaldos { get; set; } = new List<LagerSaldo>();
}
