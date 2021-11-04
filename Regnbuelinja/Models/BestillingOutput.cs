
using System.ComponentModel.DataAnnotations;

namespace Regnbuelinja.Models
{
    public class BestillingOutput
    {
        [RegularExpression(@"/(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d/")]
        public string Startpunkt { get; set; }
        [RegularExpression(@"/(0[1-9]|[12][0-9]|3[01])[- /.](0[1-9]|1[012])[- /.](19|20)\d\d/")]
        public string Endepunkt { get; set; }
        [RegularExpression(@"/(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]/")]
        public string AvreiseTid { get; set; }
        [RegularExpression(@"/(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]/")]
        public string HjemreiseTid { get; set; }

        public int AntallVoksne { get; set; }

        public int AntallBarn { get; set; }
    }
}
