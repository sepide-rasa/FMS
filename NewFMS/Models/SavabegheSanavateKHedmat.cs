using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class SavabegheSanavateKHedmat
    {
        public IEnumerable<NewFMS.WCF_Personeli.OBJ_SavabegheSanavateKHedmat> _SavabegheSanavateKHedmat { get; set; }
        public IEnumerable<NewFMS.WCF_Personeli.OBJ_PersonalInfo> PersonalInfo { get; set; }
    }
}