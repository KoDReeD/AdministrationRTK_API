﻿using System.Data.SqlClient;
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
            string sqlCommandExists = $@"SELECT * FROM sys.server_principals WHERE name = '{database}';";
            var userCheck = await GetCommandResult(connectionString, sqlCommandExists);
            BoolMethodResult result;
            if (userCheck != null && userCheck.Count > 0)
            {
                result = BoolMethodResult.GetBadRequest("Пользователь уже добавлен");
                return result;
            }

            string sqlCommand =
                $"CREATE LOGIN [{database}] WITH PASSWORD = '{password}'; " +
                $"USE [{database}]; " +
                $"CREATE USER [{database}] FOR LOGIN [{database}]; " +
                $"GRANT CONNECT TO [{database}]; " +
                $"GRANT ALTER ON SCHEMA :: dbo TO [{database}];" +
                $"GRANT SELECT,UPDATE,INSERT,DELETE ON SCHEMA::dbo TO [{database}];;" +
                $"GRANT CREATE TABLE TO [{database}];" +
                $"GRANT CREATE VIEW TO [{database}];" +
                $"GRANT CREATE PROCEDURE TO [{database}];" +
                $"GRANT EXECUTE TO [{database}];";
                // $"GRANT ALTER ON SCHEMA::dbo TO {database};" +
                // $"ALTER ROLE db_datareader ADD MEMBER {database} WITH DEFAULT_SCHEMA = dbo; " +
                // $"ALTER ROLE db_datawriter ADD MEMBER {database} WITH DEFAULT_SCHEMA = dbo;" +
                // $"ALTER ROLE db_ddladmin ADD MEMBER {database} WITH DEFAULT_SCHEMA = dbo;";

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
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var cmd = new SqlCommand())
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