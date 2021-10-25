using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.Models
{
    public class Ruter
    {
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Avreisehavn { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Ankomsthavn { get; set; }
        public double Pris { get; set; }
    }
}
