using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class Mokatebat
    {
        public IEnumerable<NewFMS.WCF_Daramad.OBJ_LetterMinut> LetterMinut { get; set; }
        public IEnumerable<NewFMS.WCF_Daramad.OBJ_Mokatebat> Mokatebe { get; set; }
        public IEnumerable<NewFMS.WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> ElamAvarez { get; set; }
        public IEnumerable<NewFMS.Models.ParamGrid> ParamGrid { get; set; }
    }
}