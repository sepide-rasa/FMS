using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class AfradTahtePoosheshTakmili
    {
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_PayPersonalInfo> Personal { get; set; }
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_AfradeTahtePoshesheBimeTakmily> Afrad { get; set; }
    }
}