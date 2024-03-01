using System.Data.SqlClient;
using System.Net;
using Npgsql;
using server_asp_api.Models;

namespace server_asp_api.Services;

public class PostgreDatabaseManager : IDatabaseManager
{
    private string _baseConnection { get; } = "Host=localhost;Username=postgres;Password=admin;";
    /// <summary>
    /// Метод который создаёт БД и пользователь вызывая готовые методы
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public override async Task<ResultModel> Register(string username, string password)
    {
        var createDbModel = await CreateDatabase(username);
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
        var createUser = await CreateUser(username, password);
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

    /// <summary>
    /// Метод создаёт БД
    /// </summary>
    /// <param name="databaseName"></param>
    /// <returns></returns>
    protected override async Task<BoolMethodResult> CreateDatabase(string username)
    {
        try
        {
            string sqlCommandExists = $@"SELECT 1 FROM pg_database WHERE datname = '{username}'";
            var dbCheck = await GetCommandResult(_baseConnection, sqlCommandExists);

            BoolMethodResult result;
            if (dbCheck != null && dbCheck.Count > 0)
            {
                result = BoolMethodResult.GetSuccessResult("База данных создана");
                return result;
            }
        
            string sqlCommand = $@"CREATE DATABASE ""{username}""";
            var dbCreateResult = await SendCommand(_baseConnection, sqlCommand);

            if (!dbCreateResult.IsSuccess)
            {
                result = BoolMethodResult.GetBadRequest($"База не создана: {dbCreateResult.Message}");
                return result;
            }

            result = BoolMethodResult.GetSuccessResult("База данных создана");
            return result;
        }
        catch (Exception e)
        {
            return BoolMethodResult.GetBadRequest(e.Message);
        }
    }

    /// <summary>
    /// Метод создаёт пользователя
    /// </summary>
    /// <param name="databaseName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    protected override async Task<BoolMethodResult> CreateUser(string databaseName, string password)
    {
        try
        {
            string sqlCommandExists = $@"SELECT 1 FROM pg_roles WHERE rolname = '{databaseName}';";
            var userCheck = await GetCommandResult(_baseConnection, sqlCommandExists);
            BoolMethodResult result;
            if (userCheck != null && userCheck.Count > 0)
            {
                result = BoolMethodResult.GetBadRequest("Пользователь уже добавлен");
                return result;
            }
            
            string sqlCommand = $"CREATE USER \"{databaseName}\" WITH PASSWORD '{password}';" +
                                $"GRANT CONNECT ON DATABASE \"{databaseName}\" TO \"{databaseName}\";" +
                                $"GRANT USAGE ON SCHEMA public TO \"{databaseName}\";" +
                                $"GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO \"{databaseName}\";";

            var createUserResult = await SendCommand(_baseConnection+$"Database={databaseName}", sqlCommand);
            if (!createUserResult.IsSuccess)
            {
                result = BoolMethodResult.GetBadRequest($"Пользователь не создан: {createUserResult.Message}");
                return result;
            }

            result = BoolMethodResult.GetSuccessResult("Пользователь создан");
            return result;
        }
        catch (Exception e)
        {
            return BoolMethodResult.GetBadRequest(e.Message);
        }
    }

    /// <summary>
    /// Метод отправляет запрос в БД (для запросов без результата)
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    protected override async Task<BoolMethodResult> SendCommand(string connectionString, string command)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;

                    cmd.CommandText = command;
                    await cmd.ExecuteReaderAsync();
                }
            }

            return BoolMethodResult.GetSuccessResult(null);
        }
        catch (Exception e)
        {
            return BoolMethodResult.GetBadRequest(null);
        }
    }

    /// <summary>
    /// Метод отправляет запрос в БД (для запросов направленных на получение результата)
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    private async Task<List<string>>? GetCommandResult(string connectionString, string command)
    {
        try
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;

                    cmd.CommandText = command;
                    List<string> results = new List<string>();
                    await using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        string row = "";
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row += (reader.IsDBNull(i) ? "NULL" : reader.GetValue(i).ToString() + " ");
                        }
                        results.Add(row);
                    }

                    return results;
                }
            }
        }
        catch (Exception e)
        {
            return null;
        }
    }
}