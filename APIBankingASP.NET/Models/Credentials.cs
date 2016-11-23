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

        public Credentials()
        {
            // default values, subject to change
            this.user = "testclient";
            this.password = "";
            this.client_key = "5bbc3c5c-6225-4935-8146-523b5883097a";
            this.client_secret = "bP7eY0fA7tW7nX7yE6oY8qD7tF3yL3fE4uK0pJ7cP3kE0mV8rF";
 
        }

        public APIBanking.Environment buildEnvironment()
        {
            return new APIBanking.Environments.YBL.UAT(
              this.user,
              this.password,
              this.client_key,
              this.client_secret,
              null);
        }
    }
}