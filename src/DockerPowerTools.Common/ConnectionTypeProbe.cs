using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DockerPowerTools.Common
{
    public static class ConnectionTypeProbe
    {
        public static async Task<ConnectionType> ProbeAsync(string partialUrl)
        {
            string httpUrl = $"http://{partialUrl}";
            string httpsUrl = $"https://{partialUrl}";

            if (await ProbeSingleAsync(httpsUrl))
                return ConnectionType.Https;

            if (await ProbeSingleAsync(httpUrl))
                return ConnectionType.Http;

            return ConnectionType.None;
        }

        static async Task<bool> ProbeSingleAsync(string uri)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                using (await HttpClientOwner.Client.SendAsync(request))
                {
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("  " + ex.GetType().Name);

                return false;
            }
        }
    }
}