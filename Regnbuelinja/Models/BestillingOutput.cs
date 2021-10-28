
using System.ComponentModel.DataAnnotations;

namespace Regnbuelinja.Models
{
    public class BestillingOutput
    {
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Startpunkt { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Endepunkt { get; set; }
        //TODO: FIKS DETTE [RegularExpression(@"(\d{4})-(\d{2})-(\d{2})( (\d{2}):(\d{2}):(\d{2}))?")]
        public string AvreiseTid { get; set; }
        //TODO: FIKS DETTE [RegularExpression(@"(\d{4})-(\d{2})-(\d{2})( (\d{2}):(\d{2}):(\d{2}))?")]
        public string HjemreiseTid { get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
    }
}
