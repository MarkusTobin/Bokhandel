using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

public partial class Anställda
{
    public int AnställdId { get; set; }

    public string Namn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public int Lön { get; set; }

    public int ButikId { get; set; }

    public virtual Butiker Butik { get; set; } = null!;
}
