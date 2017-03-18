using APIBanking;
using APIBankingASP.NET.com.quantiguous.imt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Helpers
{
    public class IMTClient : SoapClient
    {
        public static readonly String VERSION = "1";
        public static readonly String SERVICE_NAME = "IMTService";

        private static MTServiceClient createClient(APIBanking.Environment env)
        {
            MTServiceClient client;

            client = new MTServiceClient(getBinding(env), env.getEndpointAddress(SERVICE_NAME));

            if (env.needsHTTPBasicAuth())
            {
                client.ClientCredentials.UserName.UserName = env.getUser();
                client.ClientCredentials.UserName.Password = env.getPassword();
            }

            return client;
        }

        public static addBeneficiaryResponse addBeneficiary(APIBanking.Environment env, addBeneficiary request)
        {
            MTServiceClient client = createClient(env);

            request.version = VERSION;

            using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)client.InnerChannel))
            {
                System.Net.ServicePointManager.SecurityProtocol = env.getSecurityProtocol();

                System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.UserAgent = App.USER_AGENT;


                IDictionaryEnumerator headers = env.getHeaders().GetEnumerator();
                while (headers.MoveNext())
                {
                    if (headers.Key != null && headers.Value != null)
                    {
                        System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add(headers.Key.ToString(), headers.Value.ToString());
                    }
                }

                addBeneficiaryResponse response = client.addBeneficiary(request);

                return response;
            }
        }
    }
}