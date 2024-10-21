using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class Letter
    {
        public long fldID { get; set; }
        public int fldYear { get; set; }
        public long fldOrderId { get; set; }
        public string fldSubject { get; set; }
        public string fldLetterNumber { get; set; }
        public string fldLetterDate { get; set; }
        public string fldCreatedDate { get; set; }
        public string fldKeywords { get; set; }
        public int fldComisionID { get; set; }
        public int fldLetterTypeID { get; set; }
        public string fldDesc { get; set; }
        public int fldAssignmentId { get; set; }
       
    }
}