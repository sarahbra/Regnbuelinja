
using System.ComponentModel.DataAnnotations;

namespace Regnbuelinja.Models
{
    public class Bestilling
    {
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Startpunkt { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Endepunkt { get; set; }
        //Finn riktig: [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string AvreiseTid { get; set; }
        //Finn riktig: [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string HjemreiseTid { get; set; }
        [RegularExpression(@"[1-9]{1,}")]
        public int AntallVoksne { get; set; }
        [RegularExpression(@"[1-9]{1,}")]
        public int AntallBarn { get; set; }
    }
}
