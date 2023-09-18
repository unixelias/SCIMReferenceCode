using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.SCIM.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Repository;

public abstract class DatabaseRepositoryBasic
{
    private const string CONNECTION_STRING = "ConnectionStrings:MainDb";
    private readonly string __connectionString;

    protected DatabaseRepositoryBasic()
    {
        __connectionString = Environment.GetEnvironmentVariable(CONNECTION_STRING);
    }

    protected async Task<IEnumerable<TObject>> ListAsync<TObject>(string command, object parameters = null, int? timeout = null)
    {
        try
        {
            using var connection = new SqlConnection(__connectionString);
            return await connection.QueryAsync<TObject>(command, parameters, commandTimeout: timeout);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    protected async Task<TObject> GetAsync<TObject>(string command, object parameters, int? timeout = null)
    {
        try
        {
            using var conexao = new SqlConnection(__connectionString);
            return await conexao.QueryFirstOrDefaultAsync<TObject>(command, parameters, commandTimeout: timeout);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    protected async Task ExecuteAsync(string command, object parameters, int? timeout = null)
    {
        try
        {
            using var conexao = new SqlConnection(__connectionString);
            await conexao.ExecuteAsync(command, parameters, commandTimeout: timeout);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }
}