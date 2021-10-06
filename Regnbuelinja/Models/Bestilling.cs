
using System.ComponentModel.DataAnnotations;

namespace Regnbuelinja.Models
{
    public class Bestilling
    {
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Startpunkt { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Endepunkt { get; set; }
        public string AvreiseTid { get; set; }
        public string HjemreiseTid { get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
    }
}
