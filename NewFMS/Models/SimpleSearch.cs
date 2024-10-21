using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class SimpleSearch
    {
        public int fldDocumentNum { get; set; }
        public string fldDescriptionDocu { get; set; }
        public string fldSh_Artikl { get; set; }
        public string fldTarikhDocument { get; set; }
        public string fldCode { get; set; }
        public long fldBedehkar { get; set; }
        public long fldBestankar { get; set; }
        public string fldfish_check { get; set; }
        public string fldfish_checkType { get; set; }

        public int fldId { get; set; }
        public byte fldType { get; set; }
        public string fldTypeName { get; set; }
        public bool fldIsArchive { get; set; }
        public int? fldAtfNum { get; set; }
        public Nullable<int> ShomareRoozane { get; set; }
        public string fldArchiveNum { get; set; }
        public Nullable<int> fldShomareFaree { get; set; }
        public string fldNameModule { get; set; }
        public Nullable<int> fldMainDocNum { get; set; }
        public string fldTypeSanadName { get; set; }


    }
}