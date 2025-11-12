using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        static int Main()
        {
            try { return MainAsync().GetAwaiter().GetResult(); }
            catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
        }

        static async Task<int> MainAsync()
        {
            var origin = "43.6167,7.0530"; // Sophia
            var dest = "43.7034,7.2663"; // Nice

            var qs = $"route?origin={Uri.EscapeDataString(origin)}&dest={Uri.EscapeDataString(dest)}";
            using (var http = new HttpClient { BaseAddress = new Uri("http://localhost:9002/api/") })
            {
                var resp = await http.GetAsync(qs);
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                Console.WriteLine(json);
            }
            return 0;
        }
    }
}
