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
        [RegularExpression(@"/^([1-9]|([012][0-9])|(3[01])).([0]{0,1}[1-9]|1[012]).\d\d\d\d [012]{0,1}[0-9]:[0-6][0-9]$/")]
        public string AvreiseTid { get; set; }
        [RegularExpression(@"/^([1-9]|([012][0-9])|(3[01])).([0]{0,1}[1-9]|1[012]).\d\d\d\d [012]{0,1}[0-9]:[0-6][0-9]$/")]
        public string AnkomstTid { get; set; }
    }
}
