using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Models
{
    namespace DomesticRemittanceByPartnerService
    { 
        public class GetBalanceRequest : Credentials
        {
            public String version { get; set; }
            public String partnerCode { get; set; }
            public String customerID { get; set; }
            public String accountNo { get; set; } 
        }
        public class GetBalanceResult
        {
            public String version { get; set; }
            public String accountCurrencyCode { get; set; }
            public Decimal accountBalanceAmount { get; set; }
            public Boolean lowBalanceAlert { get; set; }
        }
    }
}