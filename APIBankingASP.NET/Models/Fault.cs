using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace APIBankingASP.NET.Models
{
    public class Fault
    {
        public String code { get; }
        public String subCode { get; }
        public String message { get; }
        public String messageInserts { get; }
        public String responseText { get; }

        public Fault(APIBanking.Fault ex)
        {

            // for information on faultCode, refer documentation 
            this.code = ex.Code;

            // faultSubCode is for information only, do not use in your application, this is subject to change without notice
            this.subCode = ex.SubCode;

            // an english message, you can choose to show this to your users
            this.message = ex.Message;

            // additional diagnostic information (may be asked by tech-support)
            this.messageInserts = ex.MessageInserts;

            // in some cases the response is also parsed and made available (may be asked by support)
            this.responseText = ex.responseText;
        }
    }
}