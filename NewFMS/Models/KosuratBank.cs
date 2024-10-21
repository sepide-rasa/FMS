using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class KosuratBank
    {
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_KosuratBank> _KosuratBank { get; set; }
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_PayPersonalInfo> PersonalInfo { get; set; }
    }
}