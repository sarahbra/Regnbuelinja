using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.Models
{
    public class Ferder
    {
        public int FId { get; set; }
        public int BId { get; set; }
        public int RId { get; set; }
        //string eller DateTime - Frontend decide, ved string trengs regular expression
        public DateTime AvreiseTid { get; set; }
        public DateTime AnkomstTid { get; set; }
    }
}
