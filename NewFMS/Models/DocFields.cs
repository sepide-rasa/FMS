using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class DocFields
    {
        public string Name { get; set; }
        public string EnName { get; set; }
        public byte Type { get; set; }
    }
    public class BankTemplate_Details
    {
        public int Id { get; set; }
        public int BankId { get; set; }
    }
    public class MapBankBill
    {
        public IEnumerable<NewFMS.WCF_Accounting.OBJ_BankBillDetails> BankBill { get; set; }
        public IEnumerable<NewFMS.WCF_Accounting.OBJ_DocumentRecord_Details> DocDetails { get; set; }
    }
}