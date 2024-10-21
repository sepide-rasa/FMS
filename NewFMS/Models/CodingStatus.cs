using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class CodingStatus
    {
        public IEnumerable<NewFMS.WCF_Accounting.OBJ_RptCodingStatus> RptCoding { get; set; }
        public IEnumerable<NewFMS.WCF_Accounting.OBJ_Coding_Details> Coding_Details { get; set; }
    }
}