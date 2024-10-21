using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class LogOnUsers
    {
        public string IPAdress { get; set; }
        public string UserName { get; set; }
        public string userId { get; set; }
        public string Name { get; set; }
        public bool newStatus { get; set; }
        public string AkharinOn { get; set; }
        public string sessionId { get; set; }
    }
}