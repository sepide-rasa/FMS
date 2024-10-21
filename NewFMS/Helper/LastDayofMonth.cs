using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Helper
{
    public class LastDayofMonth
    {
        public static string GetLastDay(int Year, int Month)
        {
            if (Month == 1 || Month == 2 || Month == 3 || Month == 4 || Month == 5 || Month == 6)
            {
                return "31";
            }
            else if (Month == 7 || Month == 8 || Month == 9 || Month == 10 || Month == 11)
            {
                return "30";
            }
            else
            {
                var kabise = MyLib.Shamsi.Iskabise(Year);
                if (kabise == true)
                {
                    return "30";
                }
                else{
                    return "29";
                }
            }
        }
    }
    public class BimeCls
    {
        public int TedadKol { get; set; }
        public int TedadMard { get; set; }
        public int TedadZan { get; set; }
        public decimal J_Mazaya { get; set; }
        public decimal j_motalebat { get; set; }
        public decimal J_Bime_P { get; set; }
        public decimal J_Bime_K { get; set; }
        public decimal j_bime_bikari { get; set; }
        public string Masir { get; set; }
        public string Masir1 { get; set; }
    }
}