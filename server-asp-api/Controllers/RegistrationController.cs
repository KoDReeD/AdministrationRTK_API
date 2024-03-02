using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using server_asp_api.Models;
using server_asp_api.Models.Gitea;
using server_asp_api.Services;

namespace server_asp_api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class RegistrationController : ControllerBase
{
    [HttpPost]
    public async void RegistrationOnGitea(GiteaRegistrationModel data)
    {
        using HttpClient httpClient = new HttpClient();
        
        string json = JsonConvert.SerializeObject(data);
        StringContent content = new StringContent(json);
        var url = "host/api/v1/admin/users";
        
        using var response = await httpClient.PostAsync(url, content);
        
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
        }
        else
        {
            
        }
    }
    
    //TODO: пофиксить отображение всех бд в MSsql и ролей (если можно)

    [HttpPost]
    public async Task<IActionResult> Postgre(string username, string password)
    {
        var connectionString = "Host=localhost;Username=postgres;Password=admin;";
        PostgreDatabaseManager manager = new PostgreDatabaseManager();
        var result = await manager.Register(username,password, connectionString);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Mssql(string username, string password)
    {
        var connectionString = "Server=localhost;User Id=admin;Password=admin;Trusted_Connection=True;";
        MssqlDatabaseManager manager = new MssqlDatabaseManager();
        var result = await manager.Register(username,password, connectionString);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Mysql(string username, string password)
    {
        var connectionString = "server=localhost;user=root;port=3306;password=admin;Allow User Variables=true;";
        MysqlDatabaseNanager manager = new MysqlDatabaseNanager();
        var result = await manager.Register(username,password, connectionString);
        return Ok(result);
    }
}