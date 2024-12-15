using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

public partial class Författare
{
    public int FörfattareId { get; set; }

    public string Förnamn { get; set; } = null!;

    public string Efternamn { get; set; } = null!;

    public DateOnly Födelsedatum { get; set; }

    public virtual ICollection<Böcker> Isbn13 { get; set; } = new List<Böcker>();

    public string FullName
    {
        get { return $"{Förnamn} {Efternamn}"; }
    }
}
