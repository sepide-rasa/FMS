using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models.Mohasebat
{
    public class MoteghayerhayeHoghoghi
    {
        public int fldId { get; set; }
        public string fldTarikhEjra { get; set; }
        public string fldTarikhSodur { get; set; }
        public int fldAnvaeEstekhdamId { get; set; }
        public int fldTypeBimeId { get; set; }
        public int fldZaribEzafeKar { get; set; }
        public double fldSaatKari { get; set; }
        public double fldDarsadBimePersonal { get; set; }
        public double fldDarsadbimeKarfarma { get; set; }
        public double fldDarsadBimeBikari { get; set; }
        public double fldDarsadBimeJanbazan { get; set; }
        public double fldHaghDarmanKarmand { get; set; }
        public double fldHaghDarmanKarfarma { get; set; }
        public double fldHaghDarmanDolat { get; set; }
        public int fldHaghDarmanMazad { get; set; }
        public int fldHaghDarmanTahteTakaffol { get; set; }
        public double fldDarsadBimeMashagheleZiyanAvar { get; set; }
        public int fldMaxHaghDarman { get; set; }
        public int fldZaribHoghoghiSal { get; set; }
        public bool fldHoghogh { get; set; }
        public bool fldFoghShoghl { get; set; }
        public bool fldTafavotTatbigh { get; set; }
        public bool fldFoghVizhe { get; set; }
        public bool fldHaghJazb { get; set; }
        public bool fldTadil { get; set; }
        public bool fldBarJastegi { get; set; }
        public bool fldSanavat { get; set; }
        public bool fldFoghTalash { get; set; }
        public List<MoteghayerhayeHoghoghi_Detail> Items = new List<MoteghayerhayeHoghoghi_Detail>();

        
    }

    public class MoteghayerhayeHoghoghi_Detail
    {
        public int fldId { get; set; }
        public int fldMoteghayerhayeHoghoghiId { get; set; }
        public int fldItemEstekhdamId { get; set; }
        public int fldItemHoghughiId { get; set; }

    }

}