using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Regnbuelinja.Models
{
    //Disse felten kommer fra klienten. Derfor det mer ryddig å ha denne klassen i tillegg. Vi får ikke Id og totalpris fra klient
    //Man vil alltid representere den datamodellen som faktisk er, i dette tilfellet er det input fra klienten. 
    public class BestillingInput
    {
        public string Startpunkt { get; set; }
        public string Endepunkt { get; set; }
        public DateTime AvreiseDato { get; set; }
        public Nullable<DateTime> HjemreiseDato { get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
    }
}
