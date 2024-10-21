using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class SaleRules
    {
        public IEnumerable<NewFMS.WCF_Weigh.OBJ_Arze> Arze { get; set; }
        public IEnumerable<NewFMS.WCF_Weigh.OBJ_Arze_Detail> Arze_Details { get; set; }
        public IEnumerable<NewFMS.Models.spr_SelectParamerValueFromArze> Params { get; set; }   
    }
}