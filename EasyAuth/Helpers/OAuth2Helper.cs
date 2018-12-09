using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EasyAuth.Helpers
{
    public static class OAuth2Helper
    {
        private static readonly HttpClient client = new HttpClient();


        public async static Task<OAuthModel> GetAccessTokenFromCode(string url, string redirect_url, string client_id, string client_secret, string scope_list, string code)
        {

            var values = new Dictionary<string, string>
            {
               { "client_id", client_id },
               { "client_secret", client_secret },
                {"grant_type","authorization_code"},
                {"scope",scope_list},
                {"redirect_uri", redirect_url},
                {"code",code}
            };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(url, content);

            var responseString = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("Reponse: ", responseString);

            var model = JsonConvert.DeserializeObject<OAuthModel>(responseString);

            return model;
        }


    }

    public class OAuthModel
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

    }
}
