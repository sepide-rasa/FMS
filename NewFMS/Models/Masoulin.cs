using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class Masoulin
    {
        public IEnumerable<NewFMS.WCF_Common.OBJ_Masoulin> Masoulin_H { get; set; }
        public IEnumerable<NewFMS.WCF_Common.OBJ_Masoulin_Detail> Masoulin_Detail { get; set; }
    }
}