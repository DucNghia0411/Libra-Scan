using Azure.Core;
using LIBRA.Scan.ApiIntergration.ApiConstracts;
using LIBRA.Scan.ApiIntergration.Ultilities;
using LIBRA.Scan.Common.Manager;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.Respones;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.ApiIntergration.ApiClients
{
    public class RoleApiClient : BaseApiClient, IRoleApiClient
    {
        public new string _address = SettingUrl.GetAddress();

        public RoleApiClient() : base() { }

        public async Task<List<string>> GetRolesByUserAsync(GetRolesByUserRequest request)
        {
            string token = SessionManager.Token;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_address);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync("/api/roles/getrolesbyuser", httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = JsonConvert.DeserializeObject<RequestResponse>(await response.Content.ReadAsStringAsync());

                        if (result != null && result.Content != null)
                        {
                            try
                            {
                                var content = JsonConvert.DeserializeObject<List<string>>(result.Content);

                                if (content != null)
                                    return content;
                            }
                            catch (JsonReaderException)
                            {
                                //add log
                                throw;
                            }
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    //add log
                    throw;
                }
            }

            return new List<string>();
        }
    }
}
