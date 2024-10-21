using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class AfradTahtePooshesh
    {
        public IEnumerable<NewFMS.WCF_Personeli.OBJ_PersonalInfo> Personal { get; set; }
        public IEnumerable<NewFMS.WCF_Personeli.OBJ_AfradTahtePooshesh> Afrad { get; set; }
    }
}