namespace server_asp_api.Models.Gitea;

public class GiteaRegistrationModel
{
    public DateTimeOffset? Created_At { get; set; }
    public string Email { get; set; }
    public string? Full_Name { get; set; }
    public string? Login_Name { get; set; }
    public bool Must_Change_Password { get; set; } = false;
    public string Password { get; set; }
    public bool Restricted { get; set; }
    public bool Send_Notify { get; set; }
    public int Source_Id { get; set; }
    public string Username { get; set; }
    public string? Visibility { get; set; }
}