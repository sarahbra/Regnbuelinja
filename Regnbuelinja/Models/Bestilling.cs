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
        public string Dato { get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
        // Denne må parses til en Boolean 
        public string TurRetur { get; set; }
    }

    public class Bestilling
    {
        public int Id { get; set; }
        public string Startpunkt { get; set; }
        public string Endepunkt { get; set; }
        //Denne bør være typet til en DateTime som igjen blir parset fra BestillingInput som er en string (som kommer fra JSON)
        public string Dato { get; set; }
        public int AntallVoksne { get; set; }
        public int AntallBarn { get; set; }
        public string Båtnavn { get; set; }
        public double TotalPris { get; set; }
    }
}
