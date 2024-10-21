using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class RadifShomare
    {
        public IEnumerable<NewFMS.WCF_Deceased.OBJ_Radif> radif { get; set; }
        public IEnumerable<NewFMS.WCF_Deceased.OBJ_Shomare> shomare { get; set; }
    }
}