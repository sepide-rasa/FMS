using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models.Mohasebat
{
    public class PersonalInfo
    {
        public bool fldBimeOmr { get; set; }
        public bool fldBimeTakmili { get; set; }
        public bool fldMazad30Sal { get; set; }
        public bool fldPasAndaz { get; set; }
        public bool fldSanavatPayanKhedmat { get; set; }
        public bool fldHamsarKarmand { get; set; }
        public bool fldMoafDarman { get; set; }
        public bool fldMoafAsBime { get; set; }
        public bool fldMoafAsMaliyat { get; set; }
        public byte fldStatus { get; set; }


        public int fldEsargariId { get; set; }

        public int fldJensiyat { get; set; }
    }
}