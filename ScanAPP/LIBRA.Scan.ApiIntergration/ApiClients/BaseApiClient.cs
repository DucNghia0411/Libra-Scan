using LIBRA.Scan.ApiIntergration.Ultilities;
using LIBRA.Scan.Common.Manager;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.ApiIntergration.ApiClients
{
    public class BaseApiClient
    {
        public string _address = SettingUrl.GetAddress();

        protected BaseApiClient() { }

        protected async Task<T> GetAsync<T>(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_address);
                HttpResponseMessage response;
                string body;
                response = await client.GetAsync(url);
                body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    T myDeserializedObjList = (T)JsonConvert.DeserializeObject(body, typeof(T));
                    return myDeserializedObjList;
                }
                return JsonConvert.DeserializeObject<T>(body);
            }
        }

        public async Task<List<T>> GetListAsync<T>(string url, bool requiredLogin = false)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var data = (List<T>)JsonConvert.DeserializeObject(body, typeof(List<T>));
                    return data;
                }
                throw new Exception(body);
            }
        }

        protected async Task<TResponse> AddAsync<TResponse, T>(string url, T data)
        {
            string sessions = SessionManager.Token;

            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(data);
                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(_address);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                HttpResponseMessage response;
                string body;
                try
                {
                    response = await client.PostAsync(url, httpContent);
                }
                catch (Exception)
                {
                    Object bodyOB = new { ErrorCode = 1, Content = "" };
                    body = JsonConvert.SerializeObject(bodyOB);
                    return JsonConvert.DeserializeObject<TResponse>(body);
                }

                body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TResponse myDeserializedObjList = (TResponse)JsonConvert.DeserializeObject(body,
                    typeof(TResponse));

                    return myDeserializedObjList;
                }
                return JsonConvert.DeserializeObject<TResponse>(body);
            }
        }

        public async Task<bool> DeleteAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_address);

                var response = await client.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        protected async Task<TResponse> AddWithFileAsync<TResponse, T>(string url, T data, IFormFile file)
        {
            string sessions = SessionManager.Token;

            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(data);
                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(_address);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                HttpResponseMessage response;
                string body;

                using (var memoryStream = new MemoryStream())
                {
                    try
                    {
                        await file.CopyToAsync(memoryStream);
                        var form = new MultipartFormDataContent();
                        var fileContent = new ByteArrayContent(memoryStream.ToArray());
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                        form.Add(httpContent);
                        form.Add(fileContent, "file", file.FileName);
                        response = await client.PostAsync(url, form);
                    }
                    catch (Exception)
                    {
                        Object bodyOB = new { ErrorCode = 1, Content = "" };
                        body = JsonConvert.SerializeObject(bodyOB);
                        return JsonConvert.DeserializeObject<TResponse>(body);
                    }
                }

                body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TResponse myDeserializedObjList = (TResponse)JsonConvert.DeserializeObject(body,
                    typeof(TResponse));

                    return myDeserializedObjList;
                }
                return JsonConvert.DeserializeObject<TResponse>(body);
            }
        }

    }
}
