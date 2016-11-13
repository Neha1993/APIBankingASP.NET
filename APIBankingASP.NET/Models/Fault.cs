using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Models
{
    public class Fault
    {
        public String httpStatus { get; set; }
        public String code { get; set; }
        public String subCode { get; set; }
        public String reason { get; set; }
    }
}