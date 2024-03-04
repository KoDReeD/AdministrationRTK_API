using System.Net;
using System.Text;
using Newtonsoft.Json;
using server_asp_api.Models;
using server_asp_api.Models.Gitea;

namespace server_asp_api.Services;

public class GiteaService
{
    public async Task<ResultModel> RegisterUser(GiteaRegistrationModel data)
    {
        string adminUsername = "";
        string adminPassword = "";
        string base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(adminUsername + ":" + adminPassword));
        var url = "http://localhost:3031/api/v1/admin/users";

        var jsonData = JsonConvert.SerializeObject(data);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Credentials);
            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<GiteaUserObject>(responseContent);
                var responseData = new ResultModel()
                {
                    Data = obj,
                    Message = "успех",
                    StatusCode = HttpStatusCode.BadRequest
                };
                return responseData;
            }
            else
            {
                string responseContent = response.Content.ReadAsStringAsync().Result;
                GiteaBadResponse dataJson = JsonConvert.DeserializeObject<GiteaBadResponse>(responseContent);
                var responseData = new ResultModel()
                {
                    Data = dataJson,
                    Message = "ошибка",
                    StatusCode = HttpStatusCode.BadRequest
                };
                return responseData;
            }
        }
    }
}