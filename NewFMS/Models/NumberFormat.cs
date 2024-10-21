using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class NumberFormat
    {
        public IEnumerable<NewFMS.WCF_Automation.OBJ_Secretariat_OrganizationUnit> Secretariat { get; set; }
        public IEnumerable<NewFMS.WCF_Automation.OBJ_NumberingFormat> NumberingFormat { get; set; } 
    }
}