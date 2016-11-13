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
 
            req.partnerCode = "smb1";
            req.customerID = "857552";
            req.accountNo = "001587700000054";

            return View(req);
        }

        [HttpPost]
        public ActionResult getBalance(GetBalanceRequest request)
        {
            
            APIBanking.Environment env = request.buildEnvironment(); 

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
            catch (Exception ex)
            {
                APIBankingASP.NET.Models.Fault fault = new APIBankingASP.NET.Models.Fault(ex);
                return View("fault", fault);
            }
        }
    }
}