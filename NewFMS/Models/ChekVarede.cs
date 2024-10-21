using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class ChekVarede
    {
        public IEnumerable<NewFMS.WCF_Chek.OBJ_CheckHayeVarede> Varede { get; set; }
        public IEnumerable<NewFMS.WCF_Chek.OBJ_AghsatCheckAmani> Aghsat { get; set; }
    }
}