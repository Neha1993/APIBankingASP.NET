using APIBanking;
using APIBankingASP.NET.com.quantiguous.ft2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIBankingASP.NET.Helpers
{
    public class FundsTransferByCustomerClient : SoapClient
    {
        public static readonly String VERSION = "1";
        public static readonly String SERVICE_NAME = "fundsTransferByCustomerService2";

        private static fundsTransferByCustomerService2Client createClient(APIBanking.Environment env)
        {
            fundsTransferByCustomerService2Client client;

            client = new fundsTransferByCustomerService2Client(getBinding(env), env.getEndpointAddress(SERVICE_NAME));

            if (env.needsClientCertificate())
            {
                client.ClientCredentials.ClientCertificate.Certificate = env.getClientCertificate();
            }

            if (env.needsHTTPBasicAuth())
            {
                client.ClientCredentials.UserName.UserName = env.getUser();
                client.ClientCredentials.UserName.Password = env.getPassword();
            }

            return client;
        }

        public static transferResponse transfer(APIBanking.Environment env, transfer request)
        {
            fundsTransferByCustomerService2Client client = createClient(env);

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

                transferResponse response = client.transfer(request);

                return response;
            }
        }

        public static getBalanceResponse getBalance(APIBanking.Environment env, getBalance request)
        {
            fundsTransferByCustomerService2Client client = createClient(env);

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

                getBalanceResponse response = client.getBalance(request);

                return response;
            }
        }
    }
}