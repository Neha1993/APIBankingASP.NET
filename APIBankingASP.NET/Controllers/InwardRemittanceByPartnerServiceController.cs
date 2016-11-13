using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using APIBankingASP.NET.Models.InwardRemittanceByPartnerService;
using APIBankingASP.NET.Helpers;
using System.ServiceModel;
using APIBanking;

namespace APIBankingASP.NET.Controllers
{
    public class InwardRemittanceByPartnerServiceController : Controller
    {
        public ActionResult remit()
        {
            RemitRequest req = new RemitRequest();

            req.uniqueRequestNo = Guid.NewGuid().ToString().Replace("-", "");
            req.partnerCode = "TRANSFERWI";
            req.remitterType = PartyType.I;
            req.remitterName = "Quantiguous";
            req.remitterAddress.address1 = "Mumbai";
            req.remitterContact.mobileNo = "919191919191";
            req.remitterIdentities = new Identity[1];
            req.remitterIdentities[0] = new Identity("UCN","1234","India",DateTime.Today,DateTime.Today.AddMonths(10));

            req.beneficiaryType = PartyType.C;
            req.beneficiaryName = "API Banking Services";
            req.beneficiaryAddress.address1 = "Mumbai";
            req.beneficiaryAccountNo = "026291800001191";
            req.beneficiaryIFSC = "YESB0000262";
            req.transferType = TransferType.ANY;
            req.transferAmount = 100;
            req.remitterToBeneficiaryInfo = "onboarding";

            // purpose codes are shared via a document, be sure you classify payments correctly
            req.purposeCode = "PC06";

            return View(req);
        }

        [HttpPost]
        public ActionResult remit(RemitRequest request)
        {

            if (TryValidateModel(request) == false)
            {
                return View(request);
            }

            APIBanking.Environment env = request.buildEnvironment();

            com.quantiguous.inw.remit apiReq = new com.quantiguous.inw.remit();
            com.quantiguous.inw.remitResponse apiRep;

            // the order of assignment does not matter, WCF serialises correctly
            // mandatory parameters first

            apiReq.uniqueRequestNo = request.uniqueRequestNo;
            apiReq.partnerCode = request.partnerCode;

            apiReq.remitterType = getRemitterType(request.remitterType);
            apiReq.remitterName = getName(request.remitterName);
            apiReq.remitterAddress = getAddress(request.remitterAddress);

            apiReq.beneficiaryType = getBeneficiaryType(request.beneficiaryType);
            apiReq.beneficiaryName = getName(request.beneficiaryName);
            apiReq.beneficiaryAddress = getAddress(request.beneficiaryAddress);
            apiReq.beneficiaryAccountNo = request.beneficiaryAccountNo;
            apiReq.beneficiaryIFSC = request.beneficiaryIFSC;

            apiReq.transferType = getTransferType(request.transferType);
            apiReq.transferCurrencyCode = com.quantiguous.inw.currencyCodeType.INR; // only INR is supported
            apiReq.transferAmount = (float)request.transferAmount;  // this can lead to round-off errors

            apiReq.purposeCode = request.purposeCode;
            apiReq.remitterToBeneficiaryInfo = request.remitterToBeneficiaryInfo;

            // identity information, for AML checks
            apiReq.remitterIdentities = getIdentities(request.remitterIdentities);
            apiReq.beneficiaryIdentities = getIdentities(request.beneficiaryIdentities);

            // contact information to send an email/sms
            apiReq.remitterContact = getContact(request.remitterContact);
            apiReq.beneficiaryContact = getContact(request.beneficiaryContact);



            try
            {
                apiRep = InwardRemittanceByPartnerClient.remit(env, apiReq);

                RemitResult result = new RemitResult();

                result.version = apiRep.version;
                result.uniqueResponseNo = apiRep.uniqueResponseNo;
                result.attemptNo = Decimal.Parse(apiRep.attemptNo);
                result.transferType = getTransferType(apiRep.transferType);
                result.lowBalanceAlert = apiRep.lowBalanceAlert;

                result.transferStatus.statusCode = getStatusCode(apiRep.transactionStatus.statusCode);
                result.transferStatus.subStatusCode = apiRep.transactionStatus.subStatusCode;
                result.transferStatus.bankReferenceNo = apiRep.transactionStatus.bankReferenceNo;

                return View("remitResult", result);

            }
            catch (Exception ex)
            {
                APIBankingASP.NET.Models.Fault fault = new APIBankingASP.NET.Models.Fault(ex);
                return View("fault", fault);
            }
        }

        private com.quantiguous.inw.remitterType getRemitterType(PartyType partyType)
        {
            if (partyType == PartyType.I)
                return com.quantiguous.inw.remitterType.I;
            else
                return com.quantiguous.inw.remitterType.C;
        }

        private com.quantiguous.inw.beneficiaryType getBeneficiaryType(PartyType partyType)
        {
            if (partyType == PartyType.I)
                return com.quantiguous.inw.beneficiaryType.I;
            else
                return com.quantiguous.inw.beneficiaryType.C;
        }

        private com.quantiguous.inw.nameType getName(String inName)
        {
            com.quantiguous.inw.nameType name = new com.quantiguous.inw.nameType();
            name.Item = inName;
            return name;
        }

        private com.quantiguous.inw.AddressType getAddress(Address inAddress)
        {
            com.quantiguous.inw.AddressType address = new com.quantiguous.inw.AddressType();
            address.address1 = inAddress.address1;
            address.address2 = inAddress.address2;
            address.address3 = inAddress.address3;
            address.city = inAddress.city;
            address.country = inAddress.country;
            address.postalCode = inAddress.postalCode;

            return address;
        }

        private com.quantiguous.inw.contactType getContact(Contact inContact)
        {
            com.quantiguous.inw.contactType contact = new com.quantiguous.inw.contactType();
            contact.mobileNo = inContact.mobileNo;
            contact.emailID = inContact.emailID;

            return contact;
        }

        private com.quantiguous.inw.IdentityType[] getIdentities(Identity[] inIdentities)
        {
            com.quantiguous.inw.IdentityType[] identities;
            
            if ( inIdentities != null && inIdentities.Length > 0 )
            {
                identities = new com.quantiguous.inw.IdentityType[inIdentities.Length];

                for (int i=0; i<inIdentities.Length; i++)
                {
                    identities[i] = new com.quantiguous.inw.IdentityType();

                    // there are 5 elements in this object
                    identities[i].ItemsElementName = new com.quantiguous.inw.ItemsChoiceType[5];
                    identities[i].Items = new object[5];

                    // the order below should be exactly the way it is specified in the schema
                    int j = 0;
                    identities[i].ItemsElementName[j] = com.quantiguous.inw.ItemsChoiceType.idType;
                    identities[i].Items[j++] = inIdentities[i].idType;

                    identities[i].ItemsElementName[j] = com.quantiguous.inw.ItemsChoiceType.idNumber;
                    identities[i].Items[j++] = inIdentities[i].idNumber;

                    identities[i].ItemsElementName[j] = com.quantiguous.inw.ItemsChoiceType.idCountry;
                    identities[i].Items[j++] = getISO3166CountryCode(inIdentities[i].idCountry);

                    identities[i].ItemsElementName[j] = com.quantiguous.inw.ItemsChoiceType.issueDate;
                    identities[i].Items[j++] = inIdentities[i].issueDate;

                    identities[i].ItemsElementName[j] = com.quantiguous.inw.ItemsChoiceType.expiryDate;
                    identities[i].Items[j++] = inIdentities[i].expiryDate;
                }
            }
            else
            {
                // the identities element is mandatory, even when no identity is passed
                identities = new com.quantiguous.inw.IdentityType[0]; ;
            }

            return identities;
        }

        // country codes are as per the aplha-2 code specified here http://en.wikipedia.org/wiki/ISO_3166-1
        // this should be completed as per your conventions
        private String getISO3166CountryCode(String countryName)
        {
            switch (countryName)
            {
                case "India" :
                    return "IN";
            }
            // some default
            return "US";
        }

        private com.quantiguous.inw.transferTypeType getTransferType(TransferType transferType)
        {
            switch (transferType)
            {
                case TransferType.ANY:
                   return com.quantiguous.inw.transferTypeType.ANY;               
                case TransferType.NEFT:
                    return com.quantiguous.inw.transferTypeType.NEFT;
                case TransferType.IMPS:
                    return com.quantiguous.inw.transferTypeType.IMPS;
                case TransferType.FT:
                    return com.quantiguous.inw.transferTypeType.FT;
            }

            // some default
            return com.quantiguous.inw.transferTypeType.ANY;
        }

        private TransferType? getTransferType(com.quantiguous.inw.transferTypeType? transferType)
        {
            switch (transferType)
            {
                case com.quantiguous.inw.transferTypeType.ANY:
                    return TransferType.ANY;
                case com.quantiguous.inw.transferTypeType.NEFT:
                    return TransferType.NEFT;
                case com.quantiguous.inw.transferTypeType.IMPS:
                    return TransferType.IMPS;
                case com.quantiguous.inw.transferTypeType.FT:
                    return TransferType.FT;
            }

            // transferType isn't always going to come, handle that situation
            return null;
        }

        private LastTransferStatus getStatusCode(com.quantiguous.inw.transactionStatusTypeStatusCode statusCode)
        {
            switch (statusCode)
            {
                // not yet processed, will be done within 1 hour
                case com.quantiguous.inw.transactionStatusTypeStatusCode.IN_PROCESS:
                    return LastTransferStatus.IN_PROCESS;
                // a failure, reason for failure is in the subStatusCode
                case com.quantiguous.inw.transactionStatusTypeStatusCode.FAILED:
                    return LastTransferStatus.FAILED;
                // completed, money debited and credite
                case com.quantiguous.inw.transactionStatusTypeStatusCode.COMPLETED:
                    return LastTransferStatus.COMPLETED;
                // money debited, and sent to beneficiary bank, can still get returned
                case com.quantiguous.inw.transactionStatusTypeStatusCode.SENT_TO_BENEFICIARY:
                    return LastTransferStatus.SENT_TO_BENEFICIARY;
                // not yet processed, will be done next working day
                case com.quantiguous.inw.transactionStatusTypeStatusCode.SCHEDULED_FOR_NEXT_WORKDAY:
                    return LastTransferStatus.SCHEDULED_FOR_NEXT_WORKDAY;
                // beneficiary bank returned the money, credited back to your account
                case com.quantiguous.inw.transactionStatusTypeStatusCode.RETURNED_FROM_BENEFICIARY:
                    return LastTransferStatus.RETURNED_FROM_BENEFICIARY;
            }

            // some default, when in doubt, do not send the money again
            return LastTransferStatus.IN_PROCESS;
        }
    }
}
