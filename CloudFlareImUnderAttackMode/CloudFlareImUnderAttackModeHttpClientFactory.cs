using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudFlareImUnderAttackMode
{
    public class CloudFlareImUnderAttackModeHttpClientFactory
    {
        public HttpClient Create(Uri baseUri)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler cookieHandler = new HttpClientHandler { CookieContainer = cookies };
            var httpClient = new HttpClient(cookieHandler);

            Task<HttpResponseMessage> asyncResponse = httpClient.GetAsync(baseUri);
            HttpResponseMessage respone = asyncResponse.Result;

            var html = respone.Content.ReadAsStringAsync().Result;

            if (!respone.IsSuccessStatusCode)
            {
                GetClearanceCookie(httpClient, html);
            }

            return httpClient;
        }

        public void GetClearanceCookie(HttpClient client, string html)
        {
            System.Threading.Thread.Sleep(4000);

            DecodeChallengeQuestion decodeChallengeQuestion = new DecodeChallengeQuestion();
            var clearanceUrl = decodeChallengeQuestion.GetClearanceUrl(html);

            Task<HttpResponseMessage> asyncClearanceResponse = client.GetAsync(clearanceUrl);
            HttpResponseMessage clearanceResponse = asyncClearanceResponse.Result;

            clearanceResponse.EnsureSuccessStatusCode();
        }
    }
}
