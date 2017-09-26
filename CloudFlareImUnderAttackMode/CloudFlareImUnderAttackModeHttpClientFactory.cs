using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudFlareImUnderAttackMode
{
    class CloudFlareImUnderAttackModeHttpClientFactory
    {
        public void Init(Uri baseUri)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler cookieHandler = new HttpClientHandler { CookieContainer = cookies };
            var httpClient = new HttpClient(cookieHandler);

            Task<HttpResponseMessage> asyncResponse = httpClient.GetAsync(baseUri);
            HttpResponseMessage respone = asyncResponse.Result;

            var html = respone.Content.ReadAsStringAsync().Result;

            System.Threading.Thread.Sleep(4000);

            DecodeChallengeQuestion decodeChallengeQuestion = new DecodeChallengeQuestion();
            var clearanceUrl = decodeChallengeQuestion.GetClearanceUrl(html);

            Task<HttpResponseMessage> asyncClearanceResponse = httpClient.GetAsync(clearanceUrl);
            HttpResponseMessage clearanceResponse = asyncClearanceResponse.Result;

            clearanceResponse.EnsureSuccessStatusCode();
        }
    }
}
