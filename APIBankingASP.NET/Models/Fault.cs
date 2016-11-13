using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace APIBankingASP.NET.Models
{
    public class Fault
    {
        public String httpStatus { get; set; }
        public String code { get; set; }
        public String subCode { get; set; }
        public String reason { get; set; }

        public Fault(Exception ex)
        {
            if (ex is TimeoutException)
            {       
                this.httpStatus = "504";
                this.reason = ex.Message;
            }
            else if (ex is FaultException)
            {

                // soap faults come with status 500
                this.httpStatus = "500";

                // for information on faultCode, refer documentation 
                this.code = APIBanking.SoapClient.formatFaultCode(((FaultException)ex).Code.SubCode);

                // faultSubCode is for information only, do not use in your application, this is subject to change without notice
                if (((FaultException)ex).Code.SubCode.SubCode != null)
                {
                    this.subCode = APIBanking.SoapClient.formatFaultCode(((FaultException)ex).Code.SubCode.SubCode);
                }

                // an english message, you can choose to show this to your users
                this.reason = ((FaultException)ex).Reason.ToString();
            }
            else if (ex is CommunicationException)
            {
                this.httpStatus = "503";
                this.reason = ex.Message;
            }
            else
            {
                this.httpStatus = "502";
                this.reason = ex.Message;
            }
        }
    }
}