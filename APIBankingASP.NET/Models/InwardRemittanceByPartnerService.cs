using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Models
{
    namespace InwardRemittanceByPartnerService
    {
        public class Address
        {
            public String address1 { get; set; }
            public String address2 { get; set; }
            public String address3 { get; set; }
            public String postalCode { get; set; }
            public String city { get; set; }
            public String stateOrProvince { get; set; }
            public String country { get; set; } 
        }
        public class Contact
        {
            public String mobileNo { get; set; }
            public String emailID { get; set; }
        }
        public class Identity
        {
            public String idType { get; set; }
            public String idNumber { get; set; } 
            public String idCountry { get; set; } 
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime issueDate { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime expiryDate { get; set; }

            public Identity(String idType, String idNumber, String idCountry, DateTime issueDate, DateTime expiryDate)
            {
                this.idType = idType;
                this.idNumber = idNumber;
                this.idCountry = idCountry;
                this.issueDate = issueDate;
                this.expiryDate = expiryDate;
            }

            public Identity()
            {

            }
        }
        public enum PartyType
        {
            I,
            C
        }
        public enum TransferType
        {
            FT,
            IMPS,
            NEFT,
            ANY
        }
        public enum LastTransferStatus
        {
            IN_PROCESS,
            COMPLETED,
            FAILED,
            SENT_TO_BENEFICIARY,
            RETURNED_FROM_BENEFICIARY,
            SCHEDULED_FOR_NEXT_WORKDAY
        }
        public class TransferStatus
        {
            public LastTransferStatus statusCode;
            public String subStatusCode;
            public String bankReferenceNo;
        }
        public class RemitRequest : Credentials
        {
            [Required]
            public String uniqueRequestNo { get; set; }
            public String partnerCode { get; set; }
            public PartyType remitterType { get; set; }
            public String remitterName { get; set; }
            public Address remitterAddress { get; set; }
            public Contact remitterContact { get; set; }
            public Identity[] remitterIdentities { get; set; }
            public PartyType beneficiaryType { get; set; }
            public String beneficiaryName { get; set; }
            public Address beneficiaryAddress { get; set; }
            public Contact beneficiaryContact { get; set; }
            public Identity[] beneficiaryIdentities { get; set; }
            public String beneficiaryAccountNo { get; set; }
            public String beneficiaryIFSC { get; set; }
            public TransferType transferType { get; set; }
            public String transferCurrencyCode { get;  }
            public Decimal transferAmount { get; set; }
            public String remitterToBeneficiaryInfo { get; set; }
            public String purposeCode { get; set; }

            public RemitRequest()
            {
                // only 1 value is allowed
                this.transferCurrencyCode = "INR";
                this.remitterAddress = new Address();
                this.remitterContact = new Contact();

                this.beneficiaryAddress = new Address();
                this.beneficiaryContact = new Contact();
            }
        }
        public class RemitResult
        {
            public String version;
            public String uniqueResponseNo;
            public Decimal attemptNo;
            public TransferType? transferType;
            public Boolean? lowBalanceAlert;
            public TransferStatus transferStatus; 

            public RemitResult()
            {
                this.transferStatus = new TransferStatus();
            }
        }
    }
}