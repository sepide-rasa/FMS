using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class KosorateParametri_Personal
    {
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_PayPersonalInfo> Personal { get; set; }
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_KosorateParametri_Personal> Kosorat { get; set; }
    }
    public class FishDetails
    {
        public string fldTitle { get; set; }
        public string fldGroupedTitle { get; set; }
        public Nullable<int> fldBed { get; set; }
        public Nullable<int> fldBes { get; set; }
        public int fldMashmolBime { get; set; }
        public int fldMashmolMaliyat { get; set; }
        public short fldYearPardakht { get; set; }
        public string fldMonthPardakhtName { get; set; }
        public byte fldMonthPardakht { get; set; }
        public short fldYear { get; set; }
        public string fldMonthName { get; set; }
        public byte fldMonth { get; set; }
        public byte fldKarkard { get; set; }
        public Nullable<int> fldHokmId { get; set; }
        public int fldtype { get; set; }
        public int fldCalcType { get; set; }
        public string fldCalcTypeName { get; set; }
        public int fldOrder { get; set; }
        public string fldMaliyatMashmool { get; set; }
        public string fldBimeMashmool { get; set; }
        public int fldMablaghHokm { get; set; }
        public int fldGrouped { get; set; }
    }
}