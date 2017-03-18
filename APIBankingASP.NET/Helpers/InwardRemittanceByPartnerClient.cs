using APIBanking;
using APIBankingASP.NET.com.quantiguous.inw;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Helpers
{
    public class InwardRemittanceByPartnerClient : SoapClient
    {
        public static readonly String VERSION = "1";
        public static readonly String SERVICE_NAME = "InwardRemittanceByPartnerService";

        private static InwardRemittanceByPartnerServiceClient createClient(APIBanking.Environment env)
        {
            InwardRemittanceByPartnerServiceClient client;

            client = new InwardRemittanceByPartnerServiceClient(getBinding(env), env.getEndpointAddress(SERVICE_NAME));

            if (env.needsHTTPBasicAuth())
            {
                client.ClientCredentials.UserName.UserName = env.getUser();
                client.ClientCredentials.UserName.Password = env.getPassword();
            }

            return client;
        }

        public static remitResponse remit(APIBanking.Environment env, remit request)
        {
            InwardRemittanceByPartnerServiceClient client = createClient(env);

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

                remitResponse response = client.remit(request);

                return response;
            }
        }
    }
}