using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class DocDetails
    {
        public int fldId { get; set; }
        public int fldCodingId { get; set; }
        public string fldDescription { get; set; }
        public Nullable<int> fldCaseId { get; set; }
        public int fldDocument_HedearId { get; set; }
        public long fldBedehkar { get; set; }
        public long fldBestankar { get; set; }
        public Nullable<int> fldCenterCoId { get; set; }
        public Nullable<int> fldCaseTypeId { get; set; }
        public int fldSourceId { get; set; }
        public short fldOrder { get; set; }
    }
    
    public class Document
    {
        public IEnumerable<NewFMS.WCF_Accounting.OBJ_DocumentRecord_Header> Header { get; set; }
        public IEnumerable<NewFMS.WCF_Accounting.OBJ_DocumentRecord_Details> Detail { get; set; }
    }
    public class Coding_DaramadCode
    {
        public int fldId { get; set; }
        public string fldDaramadCode { get; set; }
    }
    public class Coding_Anticipation
    {
        public long Mablagh { get; set; }        
        public int BudgetTypeId { get; set; }
        public Nullable<int> MotammamId { get; set; }
        public string Name { get; set; }
    }
    public class MablaghPerCoding
    {
        public int fldAccCodingId { get; set; }
        public List<Coding_Anticipation> Mablagh { get; set; }
    }
    public class Tas_him
    {
        public int fldCodingAcc_DetailsId { get; set; }
        public string fldTitle { get; set; }
        public string fldCode { get; set; }
        public string fldDaramadCode { get; set; }
        public Nullable<decimal> fldPercentHazine { get; set; }
        public Nullable<decimal> fldPercentTamallok { get; set; }
        public bool fldHazine { get; set; }
        public bool fldSarmaye { get; set; }
        public bool fldMali { get; set; }
        public string fldHozeMasraf { get; set; }
        public List<string> fldHozeMamooriyat { get; set; }
    }
    public enum BudgetType
    {
        Amalkard1=4,
        Mosavab1=2,
        Pishnahadi=1,
        Afzayesh=9,
        Kahesh=10
    }    
}