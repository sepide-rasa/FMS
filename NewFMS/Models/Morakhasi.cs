using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class Morakhasi
    {
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_PayPersonalInfo> Personal { get; set; }
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_Morakhasi> PayMorakhasi { get; set; }
    }
}