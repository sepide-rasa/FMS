using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models.Mohasebat
{
     public class Hokm
    {
        public int HokmId { get; set; }
        public string TarikhEjra { get; set; }
        public string TarikhSodur { get; set; }
        public int NoeEstekhdam { get; set; }
        public int NoeTaahol { get; set; }
        public int TedadFarzand { get; set; }
        public int Days { get; set; }
        public int TedadAfradTakafol { get; set; }

        public List<HokmItems> Items = new List<HokmItems>();

        
    }

   public class HokmItems
    {
        public int ItemEstekhdamId { get; set; }
        public int ItemHoghughiId { get; set; }
        public int Mablagh { get; set; }
    }


}