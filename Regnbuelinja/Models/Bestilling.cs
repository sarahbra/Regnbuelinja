using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.Models
{
    public class Bestilling
    {
        public int Id { get; set; }
        public string Startpunkt { get; set; }
        public string Endepunkt { get; set; }
        public string Dato { get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
        public string Båtnavn { get; set; }
        public double TotalPris { get; set; } 
    }
}
