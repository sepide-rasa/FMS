using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class LetterHistory
    {
        public IEnumerable<NewFMS.WCF_Automation.OBJ_Letter> Letter { get; set; }
        public IEnumerable<NewFMS.WCF_Automation.OBJ_HistoryLetter> HistoryLetter { get; set; } 
    }
}