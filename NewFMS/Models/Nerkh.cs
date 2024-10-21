using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class Nerkh
    {
        public IEnumerable<NewFMS.WCF_Daramad.OBJ_ParametreSabet_Nerkh> ParametreSabetNerkh { get; set; }
        public IEnumerable<NewFMS.WCF_Daramad.OBJ_ParametreSabet> ParametreSabet { get; set; }
    }
}