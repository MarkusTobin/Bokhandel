using System;
using System.Collections.Generic;

namespace Labb2DatabasTest.Model;

    public partial class InfoPerButik
    {

        public string Butiksnamn { get; set; } = null!;
        public string Stad { get; set; } = null!;
        public int AntalOlikaBoktitlar { get; set; }
        public int TotaltAntalBöcker { get; set; }
        public int TotaltLagervärde { get; set; }
        public int AntalAnställda { get; set; }
        public int TotalLön { get; set; }
    }
