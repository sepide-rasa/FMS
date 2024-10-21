using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class RemittanceDetails
    {

        public string fldDesc { get; set; }
        public int fldKalaId { get; set; }
        public int fldMaxTon { get; set; }
        public bool fldControlLimit { get; set; }
    }
}