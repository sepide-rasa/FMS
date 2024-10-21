using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class PersonalHokm
    {
        public IEnumerable<NewFMS.WCF_Personeli.OBJ_PersonalHokm> _PersonalHokm { get; set; }
        public IEnumerable<NewFMS.WCF_Personeli.OBJ_PersonalInfo> PersonalInfo { get; set; }
    }
}