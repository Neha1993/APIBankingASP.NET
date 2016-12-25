using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Models
{
    public class Credentials
    {
        public String user { get; set; }
        public String password { get; set; }
        public String client_key { get; set; }
        public String client_secret { get; set; }
        public bool withClientCertificate { get; set; }


        public Credentials()
        {
            // default values, subject to change
            this.user = "testclient";
            this.password = "";
            this.client_key = "5bbc3c5c-6225-4935-8146-523b5883097a";
            this.client_secret = "bP7eY0fA7tW7nX7yE6oY8qD7tF3yL3fE4uK0pJ7cP3kE0mV8rF";
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public APIBanking.Environment buildEnvironment()
        {
            if (withClientCertificate == false)
            {
                return new APIBanking.Environments.YBL.UAT(
                           this.user,
                           this.password,
                           this.client_key,
                           this.client_secret,
                           null);
            }
            else
            {
                //We have read the certificate into the string from the file using password then from string we have converted it into a byte[] 
                string pkcs12RawData = "308201B930820122A00302010202045769126C300D06092A864886F70D01010505003021311F301D0603550403131674656370726F636573732E79657362616E6B2E636F6D301E170D3136303632313130303934385A170D3137303632313130303934385A3021311F301D0603550403131674656370726F636573732E79657362616E6B2E636F6D30819F300D06092A864886F70D010101050003818D003081890281810086DA74F8E6412C428DC4E2815D2C4DE313BD39079CE7365580D62FFFCE16BEC25B4DBC9766AB206EED1C1D21ACB51F6EEE256EC50FEC8AE7E589372DDB538E5B36F20DFB1C09DF9E3C26720EC3567AEE4788F6EE55461648E4DF235C1476A2FB884F1C1997DCBA0D521025DB0A8598A67FC3DE357E5B8A6CAF630FB9CDE1E0370203010001300D06092A864886F70D0101050500038181005271B237D77AFAB070319B4371B535FC0A76EB9C5C598FD337E8459DF0880BA3E7686A663F64E1F26630B85490117BF612CF3C3B186183E067FBB2A86BEDAC2CEF9A564803323DD95F162D5A0D46C3B7E556F3562C4B6CA2286C9283F046C5D34ED54979963C6AF68C9FED76FE8322225331A04A46E0E030646DED3C1F62E74C";

                return new APIBanking.Environments.YBL.UAT(
                  this.user,
                  this.password,
                  this.client_key,
                  this.client_secret,
                  StringToByteArray(pkcs12RawData));
              }
        }
    }
}