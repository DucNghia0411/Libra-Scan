using LIBRA.Scan.Entities.Constracts;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace LIBRA.Scan.Entities
{
    public class Api<T> : IApi<T> where T : class
    {
        private string BaseUrl = "https://localhost:55137/api/";
        private Dictionary<string, string> Headers;
        public async Task<T> Get(string UrlParam)
        {
            T result = null;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                //foreach (var header in Headers)
                //{
                //    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                //}
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string url = string.Format("{0}{1}", this.BaseUrl, UrlParam);
                HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode) 
                {
                    result = await response.Content.ReadFromJsonAsync<T>();
                }
                return result;
            }
        }
        public async Task<T> Post(string UrlParam, string Content)
        {
            T result = null;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                //foreach (var header in Headers)
                //{
                //    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                //}
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                StringContent content = new StringContent(Content, Encoding.UTF8, Application.Json); 
                string url = string.Format("{0}{1}", this.BaseUrl, UrlParam);
                HttpResponseMessage response = client.PostAsync(url, content).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<T>();
                }
                return result;
            }
        }
        public void Init(string _baseUrl, Dictionary<string, string> Headers)
        {
            this.BaseUrl = _baseUrl;
            this.Headers = Headers;
        }
    }
}
