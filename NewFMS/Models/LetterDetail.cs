using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class LetterDetail
    {
        public IEnumerable<WCF_Automation.OBJ_LetterAttachment> Attach { get; set; }
        public IEnumerable<WCF_Automation.OBJ_HistoryLetter> History { get; set; }
        public IEnumerable<WCF_Automation.OBJ_Letter> Lett { get; set; }
        public IEnumerable<WCF_Automation.OBJ_LetterFollow> Follow { get; set; }
    }
}