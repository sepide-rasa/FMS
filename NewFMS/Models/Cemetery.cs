using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class Cemetery
    {
        public IEnumerable<NewFMS.WCF_Deceased.OBJ_VadiSalam> VadiSalam { get; set; }
        public IEnumerable<NewFMS.WCF_Deceased.OBJ_Ghete> Ghete { get; set; }
    }
}