using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Models
{
    public class Credentials
    {
        public String user { get; set; }
        public String password { get; set; }
        public String client_key { get; set; }
        public String client_secret { get; set; }
    }
}