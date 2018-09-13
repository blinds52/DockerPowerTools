using System.Net.Http;

namespace DockerPowerTools.Common
{
    public static class HttpClientOwner
    {
        public static readonly HttpClient Client = new HttpClient();
    }
}