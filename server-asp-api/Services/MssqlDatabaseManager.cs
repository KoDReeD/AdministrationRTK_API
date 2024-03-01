using System.Data.SqlClient;
using System.Net;
using server_asp_api.Models;

namespace server_asp_api.Services;

public class MssqlDatabaseManager : IDatabaseManager
{
    protected override async Task<BoolMethodResult> CreateDatabase(string username, string connectionString)
    {
        try
        {
            string sqlCommand = $"IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{username}') CREATE DATABASE \"{username}\";";
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
            string sqlCommand =
                $"IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = '{database}') " +
                $"BEGIN " +
                $"CREATE LOGIN \"{database}\" WITH PASSWORD = '{password}'; " +
                $"USE \"{database}\"; " +
                $"CREATE USER \"{database}\" FOR LOGIN \"{database}\"; " +
                $"GRANT CONNECT TO \"{database}\"; " +
                $"EXEC sp_addrolemember 'db_datareader', '{database}'; " +
                $"EXEC sp_addrolemember 'db_datawriter', '{database}'; " +
                $"EXEC sp_addrolemember 'db_ddladmin', '{database}'; " +
                $"END";



                BoolMethodResult result;
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
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand())
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
}