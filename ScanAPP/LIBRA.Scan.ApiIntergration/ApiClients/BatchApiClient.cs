using LIBRA.Scan.ApiIntergration.ApiConstracts;
using LIBRA.Scan.ApiIntergration.Ultilities;
using LIBRA.Scan.Common.Manager;
using LIBRA.Scan.Entities.Dtos;
using LIBRA.Scan.Entities.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LIBRA.Scan.Entities.ViewModels;
using Azure.Core;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.Respones;

namespace LIBRA.Scan.ApiIntergration.ApiClients
{
    public class BatchApiClient : BaseApiClient, IBatchApiClient
    {
        public string _address = SettingUrl.GetAddress();

        public BatchApiClient() : base() { }

        public async Task<RequestResponse> Create(BatchCreateRequest request)
        {
            var body = await AddAsync<RequestResponse, BatchCreateRequest>($"/api/batch/create", request);
            return body;
        }

        public async Task<List<BatchVM>> GetAll()
        {
            string sessions = SessionManager.Token;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_address);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
                var response = await client.GetAsync($"/api/batch/getall");
                var body = await response.Content.ReadAsStringAsync();
                RequestResponse result = JsonConvert.DeserializeObject<RequestResponse>(await response.Content.ReadAsStringAsync());
                List<BatchVM> content = (List<BatchVM>)JsonConvert.DeserializeObject(result.Content, typeof(List<BatchVM>));
                return content;
            }
        }
    }
}
