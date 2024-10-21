using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models.Mohasebat
{
    public class Mohasebat:ICloneable
    {
        public int fldId { get; set; }
        public int fldPersonalId { get; set; }
        public short fldYear { get; set; }
        public byte fldMonth { get; set; }
        public byte fldKarkard { get; set; }
        public int fldShift { get; set; }
        public byte fldGheybat { get; set; }
        public byte fldTedadEzafeKar { get; set; }
        public byte fldTedadTatilKar { get; set; }
        public byte fldBaBeytute { get; set; }
        public byte fldBedunBeytute { get; set; }
        public int fldBimeOmrKarFarma { get; set; }
        public int fldBimeOmr { get; set; }
        public int fldBimeTakmilyKarFarma { get; set; }
        public int fldBimeTakmily { get; set; }
        public int fldHaghDarmanKarfFarma { get; set; }
        public int fldHaghDarmanDolat { get; set; }
        public int fldHaghDarman { get; set; }
        public int fldBimePersonal { get; set; }
        public int fldBimeKarFarma { get; set; }
        public int fldBimeBikari { get; set; }
        public int fldBimeMashaghel { get; set; }
        public int fldBimeSakht { get; set; }
        public decimal fldDarsadBimePersonal { get; set; }
        public decimal fldDarsadBimeKarFarma { get; set; }
        public decimal fldDarsadBimeBikari { get; set; }
        public decimal fldDarsadBimeSakht { get; set; }
        public byte fldTedadNobatKari { get; set; }
        public int fldMosaede { get; set; }
        public int fldNobatPardakht { get; set; }
        public int fldGhestVam { get; set; }
        public int fldPasAndaz { get; set; }
        public int fldMashmolBime { get; set; }
        public int fldMashmolBimeNaKhales { get; set; }
        public int fldMashmolMaliyat { get; set; }
        public bool fldFlag { get; set; }
        public int fldkhalesPardakhti { get; set; }
        public int fldMogharari { get; set; }
        public int fldMaliyat { get; set; }
        public int fldSayerMazaya { get; set; }
        public int fldSayerKosoorat { get; set; }
        public int fldFiscalHeaderId { get; set; }
        public int fldMoteghayerHoghoghiId { get; set; }
        public int fldHokmId { get; set; }
        public byte fldT_Asli { get; set; }
        public byte fldT_70 { get; set; }
        public byte fldT_60 { get; set; }
        public bool fldHamsareKarmand { get; set; }
        public bool fldMazad30Sal { get; set; }
        public int fldTedadBime1 { get; set; }
        public int fldTedadBime2 { get; set; }
        public int fldTedadBime3 { get; set; }
        public int? fldVamId { get; set; }
        public int fldDayCount { get; set; }
        public int fldTashilat { get; set; }
        public int fldTedad { get; set; }//تعداد مرخصی
        public short fldSalHokm { get; set; }
        public int fldMablagh { get; set; }//عیدی، مرخصی
        public int fldMablaghKosorat { get; set; } //کسورات عیدی
        public List<Mohasebat_Items> Items = new List<Mohasebat_Items>();
        public List<Mohasebat_Kosorat_MotalebatParam> Kosorat_MotalebatParam = new List<Mohasebat_Kosorat_MotalebatParam>();
        public List<Mohasebat_KosoratBank> KosoratBank = new List<Mohasebat_KosoratBank>();
        public List<Movaghat> Movaghat = new List<Movaghat>();
        public bool err { get; set; }

        public string errMsg { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class Mohasebat_Items
    {
        public int fldId { get; set; }
        public int fldMohasebatId { get; set; }
        public int fldItemEstekhdamId { get; set; }
        public int fldItemHoghoghiId { get; set; }
        public int fldMablagh { get; set; }
        public int fldSourceId { get; set; }

    }

    public class Mohasebat_Kosorat_MotalebatParam
    {
        public int fldId { get; set; }
        public int fldMohasebatId { get; set; }
        public int? fldKosoratId { get; set; }
        public int? fldMotalebatId { get; set; }
        public int fldMablagh { get; set; }
    }

    public class Mohasebat_KosoratBank
    {
        public int fldId { get; set; }
        public int fldMohasebatId { get; set; }
        public int fldKosoratBankId { get; set; }
        public int fldMablagh { get; set; }
    }

    public class Movaghat
    {
        public int fldMohasebatId { get; set; }
        public int fldHokmId { get; set; }
        public int fldYear { get; set; }
        public int fldMonth { get; set; }
        public int fldHaghDarmanKarfFarma { get; set; }
        public int fldHaghDarmanDolat { get; set; }
        public int fldHaghDarman { get; set; }
        public int fldBimePersonal { get; set; }
        public int fldBimeKarFarma { get; set; }
        public int fldBimeBikari { get; set; }
        public int fldBimeMashaghel { get; set; }
        public int fldPasAndaz { get; set; }
        public int fldMashmolBime { get; set; }
        public int fldMashmolBimeNaKhales { get; set; }
        public int fldMashmolMaliyat { get; set; }
        public int fldMaliyat { get; set; }
        public List<Moavaghat_Items> Items = new List<Moavaghat_Items>();

    }
    public class Moavaghat_Items
    {
        public int fldId { get; set; }
        public int fldMohasebatId { get; set; }
        public int fldItemEstekhdamId { get; set; }
        public int fldItemHoghoghiId { get; set; }
        public int fldMablagh { get; set; }
        public int fldSourceId { get; set; }

    }
}