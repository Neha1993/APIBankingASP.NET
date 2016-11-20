using APIBankingASP.NET.Helpers;
using APIBankingASP.NET.Models.FundsTransferByCustomerService2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Mvc;

namespace APIBankingASP.NET.Controllers
{
    public class FundsTransferByCustomerService2Controller : Controller
    {

        private TransferRequest getTransferRequest (String appID, String customerID, String debitAccountNo, String purposeCode)
        {
            TransferRequest req = new TransferRequest();
            req.uniqueRequestNo = Guid.NewGuid().ToString().Replace("-", "");
            req.appID = appID;
            req.customerID = customerID;
            req.debitAccountNo = debitAccountNo;
            req.transferType = TransferType.NEFT;
            req.transferAmount = 100;
            req.rmtrToBeneInfo = "OnBoarding";
            req.purposeCode = purposeCode;
            return req;
        }
        // GET: FundsTransferByCustomerService with beneficiary detail
        public ActionResult transferWithBeneDetail()
        {
            TransferRequest req;
            req = getTransferRequest("299915", "299915", "000380800000781", null);
            req.beneficiaryName = "Quantiguous Solutions";
            req.beneficiaryAddress.beneficiaryAddress1 = "Wilston Road";
            req.beneficiaryAddress.beneficiaryCountry = "IN";
            req.beneficiaryContact.beneficiaryEmailID = "hello@quantiguous.com";
            req.beneficiaryContact.beneficiaryMobileNo = "9561234523";
            req.beneficiaryAccountNo = "026291800001191";
            req.beneficiaryIFSCCode = "HDFC0000001";
            req.beneficiaryMobileForMMID = "9869581569";
            req.beneficiaryMMID = "9532870";

            // transfer with bene detail view
             return View("transferWithBeneDetail", req);
            
        }

        // GET: FundsTransferByCustomerService with beneficiary code
        public ActionResult transferWithBeneCode()
        {
            TransferRequest req;
            req = getTransferRequest("26528", "26528", "000183200000030", "DOSI");
            req.beneficiaryCode = "ESECURE";

            // transfer with bene code
            return View("transferWithBeneCode", req);
        }


        [HttpPost]
        public ActionResult transfer(TransferRequest request)
        {
            if (TryValidateModel(request) == false)
            {
                return View(request);
            }

            APIBankingASP.NET.Models.Fault fault;
            APIBanking.Environment env = request.buildEnvironment();

            com.quantiguous.ft2.transfer apiReq = new com.quantiguous.ft2.transfer();
            com.quantiguous.ft2.transferResponse apiRep;

            apiReq.uniqueRequestNo = request.uniqueRequestNo;
            apiReq.purposeCode = request.purposeCode;
            apiReq.customerID = request.customerID;
            apiReq.debitAccountNo = request.debitAccountNo;

            //we have an optional element here, pass either beneficiary code or beneficiary detail
            if (request.beneficiaryCode == null)
            {
                com.quantiguous.ft2.beneficiaryType a = new com.quantiguous.ft2.beneficiaryType();
                a.Item = getBeneficiaryDetail(request);
                apiReq.beneficiary = a;
            }
            else
            {
                apiReq.beneficiary = getBeneficiaryCode(request.beneficiaryCode);
            }

            apiReq.transferType = getTransferType(request.transferType);
            apiReq.transferCurrencyCode = com.quantiguous.ft2.currencyCodeType.INR;
            apiReq.transferAmount = (float)request.transferAmount;
            apiReq.remitterToBeneficiaryInfo = request.rmtrToBeneInfo;


            try
            {
                apiRep = FundsTransferByCustomerClient.transfer(env, apiReq);

                TransferResult result = new TransferResult();

                result.version = apiRep.version;
                result.uniqueResponseNo = apiRep.uniqueResponseNo;
                result.attemptNo = int.Parse(apiRep.attemptNo);
                result.transferType = getTransferType(apiRep.transferType);
                result.lowBalanceAlert = apiRep.lowBalanceAlert;

                result.transferStatus.statusCode = getStatusCode(apiRep.transactionStatus.statusCode);
                result.transferStatus.subStatusCode = apiRep.transactionStatus.subStatusCode;
                result.transferStatus.bankReferenceNo = apiRep.transactionStatus.bankReferenceNo;
                result.requestReferenceNo = apiRep.requestReferenceNo;
                return View("transferResult", result);

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

        private LastTransferStatus getStatusCode(com.quantiguous.ft2.transactionStatusTypeStatusCode statusCode)
        {
            switch (statusCode)
            {
                // not yet processed, will be done within 1 hour
                case com.quantiguous.ft2.transactionStatusTypeStatusCode.IN_PROCESS:
                    return LastTransferStatus.IN_PROCESS;
                // a failure, reason for failure is in the subStatusCode
                case com.quantiguous.ft2.transactionStatusTypeStatusCode.FAILED:
                    return LastTransferStatus.FAILED;
                // completed, money debited and credite
                case com.quantiguous.ft2.transactionStatusTypeStatusCode.COMPLETED:
                    return LastTransferStatus.COMPLETED;
                // money debited, and sent to beneficiary bank, can still get returned
                case com.quantiguous.ft2.transactionStatusTypeStatusCode.SENT_TO_BENEFICIARY:
                    return LastTransferStatus.SENT_TO_BENEFICIARY;
                // not yet processed, will be done next working day
                case com.quantiguous.ft2.transactionStatusTypeStatusCode.SCHEDULED_FOR_NEXT_WORKDAY:
                    return LastTransferStatus.SCHEDULED_FOR_NEXT_WORKDAY;
                // beneficiary bank returned the money, credited back to your account
                case com.quantiguous.ft2.transactionStatusTypeStatusCode.RETURNED_FROM_BENEFICIARY:
                    return LastTransferStatus.RETURNED_FROM_BENEFICIARY;
            }

            // some default, when in doubt, do not send the money again
            return LastTransferStatus.IN_PROCESS;
        }
   

        private TransferType? getTransferType(com.quantiguous.ft2.transferTypeType? transferType)
        {
            switch (transferType)
            {
                case com.quantiguous.ft2.transferTypeType.ANY:
                    return TransferType.ANY;
                case com.quantiguous.ft2.transferTypeType.NEFT:
                    return TransferType.NEFT;
                case com.quantiguous.ft2.transferTypeType.IMPS:
                    return TransferType.IMPS;
                case com.quantiguous.ft2.transferTypeType.FT:
                    return TransferType.FT;
            }

            // transferType isn't always going to come, handle that situation
            return null;
        }

        private com.quantiguous.ft2.transferTypeType getTransferType(TransferType transferType)
        {
            switch (transferType)
            {
                case TransferType.ANY:
                    return com.quantiguous.ft2.transferTypeType.ANY;
                case TransferType.NEFT:
                    return com.quantiguous.ft2.transferTypeType.NEFT;
                case TransferType.IMPS:
                    return com.quantiguous.ft2.transferTypeType.IMPS;
                case TransferType.FT:
                    return com.quantiguous.ft2.transferTypeType.FT;
            }

            // some default
            return com.quantiguous.ft2.transferTypeType.ANY;
        }

        private com.quantiguous.ft2.beneficiaryDetailType getBeneficiaryDetail(TransferRequest request)
        {
            com.quantiguous.ft2.beneficiaryDetailType detail = new com.quantiguous.ft2.beneficiaryDetailType();

            com.quantiguous.ft2.AddressType addr = new com.quantiguous.ft2.AddressType();
            com.quantiguous.ft2.contactType contact = new com.quantiguous.ft2.contactType();
            com.quantiguous.ft2.nameType name = new com.quantiguous.ft2.nameType();

            name.Item = request.beneficiaryName;

            detail.beneficiaryName = name;
            addr.address1 = request.beneficiaryAddress.beneficiaryAddress1;
            addr.address2 = request.beneficiaryAddress.beneficiaryAddress2;
            addr.address3 = request.beneficiaryAddress.beneficiaryAddress3;
            addr.postalCode = request.beneficiaryAddress.beneficiaryPostalCode;
            addr.city = request.beneficiaryAddress.beneficiaryCity;
            addr.stateOrProvince = request.beneficiaryAddress.beneficiaryStateOrProvince;
            addr.country = request.beneficiaryAddress.beneficiaryCountry;

            contact.mobileNo = request.beneficiaryContact.beneficiaryMobileNo;
            contact.emailID = request.beneficiaryContact.beneficiaryEmailID;


            detail.beneficiaryAddress = addr;
            detail.beneficiaryContact = contact;
            detail.beneficiaryAccountNo = request.beneficiaryAccountNo;
            detail.beneficiaryIFSC = request.beneficiaryIFSCCode;
            detail.beneficiaryMMID = request.beneficiaryMMID;
            detail.beneficiaryMobileNo = request.beneficiaryMobileForMMID;
            return detail;
        }

        private com.quantiguous.ft2.beneficiaryType getBeneficiaryCode(String beneficiaryCode)
        {
            com.quantiguous.ft2.beneficiaryType type = new com.quantiguous.ft2.beneficiaryType();
            type.Item = beneficiaryCode;
            return type;
        }
    }
}
