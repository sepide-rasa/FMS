using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class KarkardMahane
    {
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_KarKardeMahane> _KarKardeMahane { get; set; }
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_PayPersonalInfo> PersonalInfo { get; set; }
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_KarkardMahane_Detail> KarKardDetail { get; set; }
    }

    public class HokmDetails
    {
        public IEnumerable<NewFMS.WCF_Personeli.OBJ_PersonalHokm> Hokm { get; set; }
        public IEnumerable<NewFMS.WCF_Personeli.OBJ_Hokm_Item> Hokm_Items { get; set; }
    }
}