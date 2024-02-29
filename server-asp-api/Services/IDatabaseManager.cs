using Npgsql;
using server_asp_api.Models;

namespace server_asp_api.Services;

public abstract class IDatabaseManager
{
    public abstract Task<ResultModel> Register(string username, string password);
    protected abstract Task<BoolMethodResult> CreateDatabase(string userName);
    protected abstract Task<BoolMethodResult> CreateUser(string database, string password);

    protected abstract Task<BoolMethodResult> SendCommand(string connectionString, string command);

}