using RestSharp;
using ParameterType = RestSharp.ParameterType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Paypal_Sandbox
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        public static string PayPalClientId { get; } = EnvironmentVariable.GetOrThrow("PayPalClientId");
        public static string PayPalSecret { get; } = EnvironmentVariable.GetOrThrow("PayPalSecret");


        private static readonly RestClient RestClient = new RestClient(Program.BaseAddress);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessToken(CreateBasicAuthToken());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private static string CreateBasicAuthToken()
        {
            var credentials = Encoding.GetEncoding("ISO-8859-1").GetBytes(PayPalClientId + ":" + PayPalSecret);
            var authHeader = Convert.ToBase64String(credentials);

            return "Basic " + authHeader;
        }

        private static async Task<string> GetAccessToken(string authToken)
        {
            var request = new RestRequest("v1/oauth2/token");
            request.AddHeader("Authorization", authToken);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials", ParameterType.RequestBody);

            var response = await RestClient.ExecuteAsync<Response>(request, Method.Post);

            return response.Data.access_token;
        }

        private class Response
        {
            public string access_token { get; set; }
        }
    }
}
