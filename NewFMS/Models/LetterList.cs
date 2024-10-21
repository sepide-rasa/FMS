using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class LetterList
    {
        public IEnumerable<WCF_Automation.OBJ_Inbox> Inbox { get; set; }
        public IEnumerable<WCF_Automation.OBJ_Sent> Sent { get; set; }
        public IEnumerable<WCF_Automation.OBJ_SavedLetter> SavedLetter { get; set; }
        public IEnumerable<WCF_Automation.OBJ_SavedMessage> SavedMessage { get; set; }
        public IEnumerable<WCF_Automation.OBJ_Trash> Deleted { get; set; }
        
            
            
            
            
            
    }
}