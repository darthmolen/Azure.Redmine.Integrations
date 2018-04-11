using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Azure.Redmine.Integrations.Business
{
    public static class HttpClientInstance
    {

        private static string _url;
        private static readonly HttpClient _instance = new HttpClient();
        private static bool _initialized = false;
        static HttpClientInstance()
        {

        }

        public static void Init(string url, List<KeyValuePair<string,string>> headers)
        {
            if (!_initialized)
            {
                _url = url;
                Instance.BaseAddress = new Uri(_url);

                if (headers != null)
                {
                    foreach (var kvp in headers)
                    {
                        Instance.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
                    }
                }

                _initialized = true;
            }
        }

        public static HttpClient Instance => _instance;

    }
}
