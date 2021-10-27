using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.Models
{
    public class Ferder
    {
        public int FId { get; set; }
        public int BId { get; set; }
        public int RId { get; set; }
        //Finn riktig regExp
        public string AvreiseTid { get; set; }
        public string AnkomstTid { get; set; }
    }
}
