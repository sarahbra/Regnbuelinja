using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.Models
{
    public class Baater
    {
        public int Id { get; set; }
        [RegularExpression(@"^[a-zA-ZæøåÆØÅ ]{2,30}$")]
        public string Navn { get; set; }
    }
}
