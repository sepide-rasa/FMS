using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class AhkamKari
    {
        public IEnumerable<NewFMS.WCF_Common.OBJ_Ashkhas> Ashkhas { get; set; }
        public IEnumerable<NewFMS.WCF_Automation.OBJ_Commision> Ahkam { get; set; } 
    }
}