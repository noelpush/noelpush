using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;

namespace NoelPush.Services
{
    public static class RequestService
    {
        public static async Task<string> SendRequest(string url, Dictionary<string, string> values)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var parameters = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync(url, parameters);
                    var contents = await response.Content.ReadAsStringAsync();
                    return contents;
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e.Message);
            }

            return string.Empty;
        }
    }
}
