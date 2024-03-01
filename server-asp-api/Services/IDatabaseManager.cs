using System.Net;
using Npgsql;
using server_asp_api.Models;

namespace server_asp_api.Services;

public abstract class IDatabaseManager
{
    public async Task<ResultModel> Register(string username, string password, string connectionString)
    {
        var createDbModel = await CreateDatabase(username, connectionString);
        if (!createDbModel.IsSuccess)
        {
            ResultModel result = new ResultModel()
            {
                Data = null,
                Message = createDbModel.Message,
                StatusCode = HttpStatusCode.BadRequest
            };
            return result;
        }
        var createUser = await CreateUser(username, password, connectionString);
        if (!createUser.IsSuccess)
        {
            ResultModel result = new ResultModel()
            {
                Data = null,
                Message = createUser.Message,
                StatusCode = HttpStatusCode.BadRequest
            };
            return result;
        }

        ResultModel resultModel = new ResultModel()
        {
            Data = null,
            Message = "Успешно",
            StatusCode = HttpStatusCode.Created
        };
        return resultModel;
    }
    protected abstract Task<BoolMethodResult> CreateDatabase(string userName, string connectionString);
    protected abstract Task<BoolMethodResult> CreateUser(string database, string password, string connectionString);

    protected abstract Task<BoolMethodResult> SendCommand(string connectionString, string command);

}