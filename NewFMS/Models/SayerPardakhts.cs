﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFMS.Models
{
    public class SayerPardakhts
    {
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_PayPersonalInfo> Personal { get; set; }
        public IEnumerable<NewFMS.WCF_PayRoll.OBJ_SayerPardakhts> Pardakht { get; set; }
    }
}