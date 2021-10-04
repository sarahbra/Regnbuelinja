using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Regnbuelinja.Models
{
    //Disse felten kommer fra klienten. Derfor det mer ryddig å ha denne klassen i tillegg. Vi får ikke Id og totalpris fra klient
    //Man vil alltid representere den datamodellen som faktisk er, i dette tilfellet er det input fra klienten. 
    public class BestillingInput
    {
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Startpunkt { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string Endepunkt { get; set; }
        public DateTime AvreiseTid { get; set; }
        public Nullable<DateTime> HjemreiseTid { get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
    }
}
