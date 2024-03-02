using System.Data.SqlClient;
using MySqlConnector;
using server_asp_api.Models;

namespace server_asp_api.Services;

public class MysqlDatabaseNanager : IDatabaseManager
{
    protected override async Task<BoolMethodResult> CreateDatabase(string username, string connectionString)
    {
        try
        {
            string sqlCommand = $"CREATE DATABASE IF NOT EXISTS `{username}`";
            var dbCreateResult = await SendCommand(connectionString, sqlCommand);

            BoolMethodResult result;
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

    protected override async Task<BoolMethodResult> CreateUser(string database, string password, string connectionString)
    {
        try
        {
            string sqlCommandExists = $@"SELECT User FROM mysql.user WHERE User = '{database}';";
            var userCheck = await GetCommandResult(connectionString, sqlCommandExists);
            BoolMethodResult result;
            if (userCheck != null && userCheck.Count > 0)
            {
                result = BoolMethodResult.GetBadRequest("Пользователь уже добавлен");
                return result;
            }


            string sqlCommand =
                $"CREATE USER `{database}`@'localhost' IDENTIFIED BY '{password}';" +
                $"GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, ALTER, DROP ON `{database}`.* TO '{database}'@'localhost';" +
                $"FLUSH PRIVILEGES;";

                var createUserResult = await SendCommand(connectionString+$"Database={database}", sqlCommand);
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

    protected override async Task<BoolMethodResult> SendCommand(string connectionString, string command)
    {
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = connection;

                    cmd.CommandText = command;
                    await cmd.ExecuteNonQueryAsync();
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
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new MySqlCommand())
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