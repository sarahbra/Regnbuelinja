using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.Models
{
    public class Bruker
    {
        [RegularExpression(@"^[a-zA-ZæøåÆØÅ. \-]{2,30}$")]
        public string Brukernavn { get; set; }
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-zA-ZæøåÆØÅ])([a-zA-ZæøåÆØÅ0-9]+){6,}$")]
        public string Passord { get; set; }
    }
}
