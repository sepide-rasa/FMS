using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class Occasions
    {
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_Monasebat> Monasebat { get; set; }
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_MonasebatMablagh> Mablagh { get; set; }
    }
}