using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class CalcAvarez
    {
        public int ID { get; set; }
        public int Maliat { get; set; }
        public int Avarez { get; set; }
        public int Mablagh { get; set; }
        public int Status { get; set; }
        public string CodeDaramadId { get; set; }
        public string ElamAvarezId { get; set; }
        public string TitleCodeDaramad { get; set; }
    }
}