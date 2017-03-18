using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Models
{
    namespace IMTService
    {
        public class Address
        {
            public String addressLine { get; set; }
            public String cityName { get; set; }
            public String postalCode { get; set; }
        }
       public class AddBeneficiaryRequest : Credentials
        {
            [Required]
            public String uniqueRequestNo { get; set; }
            public String appID { get; set; }
            public String customerID { get; set; }
            public String beneficiaryMobileNo { get; set; }
            public String beneficiaryName { get; set; }
            public Address beneficiaryAddress { get; set; }
            public AddBeneficiaryRequest()
            {
                this.beneficiaryAddress = new Address();
            }
        }
        public class AddBeneficiaryResult
        {
            public String version;
            public String uniqueResponseNo;
        }
    }
}