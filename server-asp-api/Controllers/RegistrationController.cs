using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using server_asp_api.Models;
using server_asp_api.Models.Gitea;
using server_asp_api.Services;

namespace server_asp_api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class RegistrationController : ControllerBase
{
    [HttpPost]
    public async Task<ResultModel> RegistrationOnGitea(GiteaRegistrationModel data)
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

    

    //TODO: пофиксить отображение всех бд в MSsql и ролей (если можно)

    [HttpPost]
    public async Task<IActionResult> Postgre(string username, string password)
    {
        var connectionString = "Host=localhost;Username=postgres;Password=admin;";
        PostgreDatabaseManager manager = new PostgreDatabaseManager();
        var result = await manager.Register(username, password, connectionString);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Mssql(string username, string password)
    {
        var connectionString = "Server=localhost;User Id=admin;Password=admin;Trusted_Connection=True;";
        MssqlDatabaseManager manager = new MssqlDatabaseManager();
        var result = await manager.Register(username, password, connectionString);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Mysql(string username, string password)
    {
        var connectionString = "server=localhost;user=root;port=3306;password=admin;Allow User Variables=true;";
        MysqlDatabaseNanager manager = new MysqlDatabaseNanager();
        var result = await manager.Register(username, password, connectionString);
        return Ok(result);
    }
}