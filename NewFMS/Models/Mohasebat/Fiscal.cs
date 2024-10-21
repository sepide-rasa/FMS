using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models.Mohasebat
{
    public class Fiscal
    {
        public int fldId { get; set; }
        public string fldTarikhEjra { get; set; }
        public string fldTarikhSodur { get; set; }

        public List<Fiscal_Detail> Items_Detail = new List<Fiscal_Detail>();
        public List<FiscalTitle> Items_Title = new List<FiscalTitle>();

    }

    public class Fiscal_Detail
    {
        public int fldId { get; set; }
        public int fldFiscalHeaderId { get; set; }
        public int fldAmountFrom { get; set; }
        public int fldAmountTo { get; set; }
        public double fldPercentTaxOnWorkers { get; set; }
        public double fldTaxationOfEmployees { get; set; }
        public int fldTax { get; set; }
       
    }

    public class FiscalTitle
    {
        public int fldId { get; set; }
        public int fldFiscalHeaderId { get; set; }
        public int fldItemEstekhdamId { get; set; }
        public int fldAnvaEstekhdamId { get; set; }
        public int fldItemHoghughiId { get; set; }
     
    }
}