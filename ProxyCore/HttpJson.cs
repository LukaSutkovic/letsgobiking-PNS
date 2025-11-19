using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProxyCore
{
    public static class HttpJson
    {
        public static async Task<T> GetAsync<T>(HttpClient http, string relative)
        {
            using (var resp = await http.GetAsync(relative).ConfigureAwait(false))
            {
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
