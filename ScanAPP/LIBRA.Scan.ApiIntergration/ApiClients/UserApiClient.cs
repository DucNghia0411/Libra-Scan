using LIBRA.Scan.ApiIntergration.ApiConstracts;
using LIBRA.Scan.ApiIntergration.Ultilities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.Respones;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.ApiIntergration.ApiClients
{
    public class UserApiClient : IUserApiClient
    {
        public string _address = SettingUrl.GetAddress();

        public UserApiClient() { }

        public async Task<RequestResponse> Authenticate(AuthRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                client.BaseAddress = new Uri(_address);
                var response = await client.PostAsync("/api/auth/login", httpContent);
                var result = JsonConvert.DeserializeObject<RequestResponse>(await response.Content.ReadAsStringAsync());
                return result;
            }
        }
    }
}
