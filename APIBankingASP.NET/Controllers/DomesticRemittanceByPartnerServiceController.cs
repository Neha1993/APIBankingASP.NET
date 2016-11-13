using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using APIBankingASP.NET.Models.DomesticRemittanceByPartnerService;
using System.ServiceModel;

namespace APIBankingASP.NET.Controllers
{
    public class DomesticRemittanceByPartnerServiceController : Controller
    {
        public ActionResult getBalance()
        {
            GetBalanceRequest req = new GetBalanceRequest();
            req.user = "testclient";
            req.password = "test@123";
            req.client_key = "5bbc3c5c-6225-4935-8146-523b5883097a";
            req.client_secret = "bP7eY0fA7tW7nX7yE6oY8qD7tF3yL3fE4uK0pJ7cP3kE0mV8rF";

            req.partnerCode = "smb1";
            req.customerID = "857552";
            req.accountNo = "001587700000054";

            return View(req);
        }
 
        [HttpPost]
        public ActionResult getBalance(GetBalanceRequest request)
        {
            APIBankingASP.NET.Models.Fault fault = new APIBankingASP.NET.Models.Fault();

            APIBanking.Environment env =
                new APIBanking.Environments.YBL.UAT(
                    request.user,
                    request.password,
                    request.client_key,
                    request.client_secret,
                    null);

            com.quantiguous.smb.getBalance apiReq = new com.quantiguous.smb.getBalance();
            com.quantiguous.smb.getBalanceResponse apiRep;

            apiReq.partnerCode = request.partnerCode;
            apiReq.customerID = request.customerID;
            apiReq.accountNo = request.accountNo;

            try
            {
                apiRep = DomesticRemittanceByPartnerClient.getBalance(env, apiReq);

                GetBalanceResult result = new GetBalanceResult();
                result.version = apiRep.version;
                result.accountCurrencyCode = apiRep.accountCurrencyCode.ToString();
                result.accountBalanceAmount = apiRep.accountBalanceAmount;
                result.lowBalanceAlert = apiRep.lowBalanceAlert;
                return View("getBalanceResult", result);

            }
            catch (TimeoutException ex)
            {
 
                fault.httpStatus = "504";
                fault.reason = ex.Message;
            }
            catch (FaultException ex)
            {

                // soap faults come with status 500
                fault.httpStatus = "500";

                // for information on faultCode, refer documentation 
                fault.code = APIBanking.SoapClient.formatFaultCode(ex.Code.SubCode);

                // faultSubCode is for information only, do not use in your application, this is subject to change without notice
                if ( ex.Code.SubCode.SubCode != null ) { 
                    fault.subCode = APIBanking.SoapClient.formatFaultCode(ex.Code.SubCode.SubCode);
                }

                // an english message, you can choose to show this to your users
                fault.reason = ex.Reason.ToString();
            }

            
            catch (CommunicationException ex)
            {
                fault.httpStatus = "503";
                fault.reason = ex.Message;
            }

            catch (Exception ex)
            {
 
                fault.httpStatus = "502";
                fault.reason = ex.ToString();
            }

            return View("fault", fault);


        }

        [HttpPost]
        public ActionResult remit()
        {
            return View();
        }
    }
}