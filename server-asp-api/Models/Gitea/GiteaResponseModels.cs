namespace server_asp_api.Models.Gitea;

public class GiteaBadResponse
{
    public string Message { get; set; }
    public string Url { get; set; }
}
    
public class GiteaUserObject
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string LoginName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public string Language { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime Created { get; set; }
    public bool Restricted { get; set; }
    public bool Active { get; set; }
    public bool ProhibitLogin { get; set; }
    public string Location { get; set; }
    public string Website { get; set; }
    public string Description { get; set; }
    public string Visibility { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public int StarredReposCount { get; set; }
    public string Username { get; set; }
}