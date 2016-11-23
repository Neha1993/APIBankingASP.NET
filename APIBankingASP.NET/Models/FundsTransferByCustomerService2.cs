using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Models
{
    namespace FundsTransferByCustomerService2
    {
        public class BeneficiaryAddress
        {
            public String address1 { get; set; }
            public String address2 { get; set; }
            public String address3 { get; set; }
            public String postalCode { get; set; }
            public String city { get; set; }
            public String stateOrProvince { get; set; }
            public String country { get; set; }
        }
            
        public class BeneficiaryContact
        {
            public String mobileNo { get; set; }
            public String emailID { get; set; }
        }

        public enum TransferType
        {
            FT,
            IMPS,
            NEFT,
            ANY,
            RTGS
        }

        public enum LastTransferStatus
        {
            IN_PROCESS,
            COMPLETED,
            FAILED,
            SENT_TO_BENEFICIARY,
            RETURNED_FROM_BENEFICIARY,
            SCHEDULED_FOR_NEXT_WORKDAY,
            ONHOLD
        }

     

        public class TransactionStatus
        {
            public LastTransferStatus statusCode;
            public String subStatusCode;
            public String bankReferenceNo;
            public String beneReferenceNo;
            public String reason;
        }

        public class TransferRequest : Credentials
        {
            [Required]
            public String uniqueRequestNo { get; set; }
            public String appID { get; set; }
            public String purposeCode { get; set; }
            public String customerID { get; set; }
            public String debitAccountNo { get; set; }
            public String beneficiaryCode { get; set; }

            public String beneficiaryName { get; set; }
            public BeneficiaryAddress beneficiaryAddress { get; set; }
            public BeneficiaryContact beneficiaryContact { get; set; }
            public String beneficiaryAccountNo { get; set; }
            public String beneficiaryIFSCCode { get; set; }
            public String beneficiaryMobileForMMID { get; set; }
            public String beneficiaryMMID { get; set; }

            public String transferCurrencyCode { get; }
            public TransferType transferType { get; set; }
            public Decimal transferAmount { get; set; }
            public String rmtrToBeneInfo { get; set; }

            public TransferRequest()
            {
                this.transferCurrencyCode = "INR";
                this.beneficiaryAddress = new BeneficiaryAddress();
                this.beneficiaryContact = new BeneficiaryContact();
            }
        }

        public class TransferResult
        {
            public String version;
            public String uniqueResponseNo;
            public Decimal attemptNo;
            public TransferType? transferType;
            public Boolean? lowBalanceAlert;
            public TransactionStatus transferStatus;
            public String nameWithBeneBank;
            public String requestReferenceNo;

            public TransferResult()
            {
                this.transferStatus = new TransactionStatus();
            }
        }
    }
}