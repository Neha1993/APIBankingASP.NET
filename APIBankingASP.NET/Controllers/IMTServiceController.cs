using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using APIBankingASP.NET.Models.IMTService;
using APIBankingASP.NET.Helpers;
using System.ServiceModel;
using System.ServiceModel.Security;
using APIBankingASP.NET.com.quantiguous.imt;

namespace APIBankingASP.NET.Controllers
{
    public class IMTServiceController : Controller
    {
        public ActionResult addBeneficiary()
        {
            AddBeneficiaryRequest req = new AddBeneficiaryRequest();

            req.uniqueRequestNo = Guid.NewGuid().ToString().Replace("-", "");
            req.appID = "16725";
            req.customerID = "5062";

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;

            req.beneficiaryMobileNo = secondsSinceEpoch.ToString();


            req.beneficiaryName = "Quantiguous Solutions";
            req.beneficiaryAddress.addressLine = "Malad";
            req.beneficiaryAddress.cityName = "Mumbai";
            req.beneficiaryAddress.postalCode = "400064";
            return View(req);
        }

        [HttpPost]
        public ActionResult addBeneficiary(AddBeneficiaryRequest request)
        {

            if (TryValidateModel(request) == false)
            {
                return View(request);
            }

            APIBankingASP.NET.Models.Fault fault;
            APIBanking.Environment env = request.buildEnvironment();

            com.quantiguous.imt.addBeneficiary apiReq = new com.quantiguous.imt.addBeneficiary();
            com.quantiguous.imt.addBeneficiaryResponse apiRep;

            // the order of assignment does not matter, WCF serialises correctly
            // mandatory parameters first

            apiReq.uniqueRequestNo = request.uniqueRequestNo;
            apiReq.appID = request.appID;
            apiReq.customerID = request.customerID;
            apiReq.beneficiaryName = request.beneficiaryName;
            apiReq.beneficiaryMobileNo = request.beneficiaryMobileNo;
            apiReq.beneficiaryAddress = getAddress(request.beneficiaryAddress);

            try
            {
                apiRep = IMTClient.addBeneficiary(env, apiReq);

                AddBeneficiaryResult result = new AddBeneficiaryResult();

                result.version = apiRep.version;
                result.uniqueResponseNo = apiRep.uniqueResponseNo;

                return View("addBeneficiaryResult", result);

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
        private com.quantiguous.imt.beneficiaryAddressType getAddress(Address inAddress)
        {
            com.quantiguous.imt.beneficiaryAddressType address = new com.quantiguous.imt.beneficiaryAddressType();
            address.addressLine = inAddress.addressLine;
            address.cityName = inAddress.cityName;
            address.postalCode = inAddress.postalCode;

            return address;
        }

    }
}
