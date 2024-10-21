using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class SodoorFish
    {
        public IEnumerable<NewFMS.WCF_Daramad.OBJ_ElamAvarez> ElamAvarez { get; set; }
        public IEnumerable<NewFMS.WCF_Daramad.OBJ_SodoorFish> SodurFish { get; set; }
        public IEnumerable<NewFMS.WCF_Daramad.OBJ_CodhayeDaramadiElamAvarez> CodhayeDaramad { get; set; }
    }
}