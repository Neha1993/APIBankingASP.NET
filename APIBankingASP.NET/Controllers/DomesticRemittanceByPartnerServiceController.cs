using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using APIBankingASP.NET.Models.DomesticRemittanceByPartnerService;
using System.ServiceModel;
using System.ServiceModel.Security;

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
            APIBankingASP.NET.Models.Fault fault;

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
            /* 
              * the following exceptions have to be caught separately, even when you handle then the same way 
              * this is because of the way the APIBanking.Fault is created
              */
            catch (MessageSecurityException e)
            {
                fault = new APIBankingASP.NET.Models.Fault(new APIBanking.Fault(e));
            }
            catch (FaultException e)
            {
                fault = new APIBankingASP.NET.Models.Fault(new APIBanking.Fault(e));
            }
            catch (Exception e)
            {
                fault = new APIBankingASP.NET.Models.Fault(new APIBanking.Fault(e));
            }
            return View("fault", fault);
        }
    }
}