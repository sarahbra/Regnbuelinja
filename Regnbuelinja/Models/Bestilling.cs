using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.Models
{
    public class Bestilling
    {
        public int Id { get; set; }
        public int KId { get; set; }
        public bool Betalt { get; set; }
    }
}
