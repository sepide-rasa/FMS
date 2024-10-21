using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class SodorCheck
    {
        public IEnumerable<NewFMS.WCF_Chek.OBJ_DasteCheck> DasteCheck { get; set; }
        public IEnumerable<NewFMS.WCF_Chek.OBJ_SodorCheck> Sodor{ get; set; }
    }
}