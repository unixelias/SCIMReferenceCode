using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.SCIM.Repository.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Repository.ScimResources;

public class UserRepository : DatabaseRepositoryBasic, IUserRepository
{
    public UserRepository(IConfiguration configuration) : base(configuration)
    {
    }
    #region SQL Eager Loading

    private static readonly string INSERT_USER = DatabaseHelper.LoadSqlStatement("InsertUser.sql", typeof(UserRepository).Namespace);
    private static readonly string LIST_ALL_USERS = DatabaseHelper.LoadSqlStatement("ListAllUsers.sql", typeof(UserRepository).Namespace);
    private static readonly string GET_USER_BY_ID = DatabaseHelper.LoadSqlStatement("GetUserById.sql", typeof(UserRepository).Namespace);
    private static readonly string CHECK_USER_EXISTS_BY_ID_AND_USERNAME = DatabaseHelper.LoadSqlStatement("CheckIfUserExists.sql", typeof(UserRepository).Namespace);
    private static readonly string UPDATE_USER_BY_ID = DatabaseHelper.LoadSqlStatement("UpdateUserById.sql", typeof(UserRepository).Namespace);
    private static readonly string DELETE_USER_BY_ID = DatabaseHelper.LoadSqlStatement("DeleteUser.sql", typeof(UserRepository).Namespace);

    #endregion SQL Eager Loading

    public async Task CreateAsync(Core2EnterpriseUser user)
     {

        try
        {
            var valor = JsonConvert.SerializeObject(user);
            var parameters = new DynamicParameters();
            parameters.Add("@UserData", valor);

            await ExecuteAsync(INSERT_USER, parameters);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    public async Task<IEnumerable<Core2EnterpriseUser>> ListAllAsync()
     {

        try
        {
            var value = await ListAsync<string>(LIST_ALL_USERS);

            var returnValue = value.Select(user => JsonConvert.DeserializeObject<Core2EnterpriseUser>(user));

            return returnValue;

        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    public async Task<Core2EnterpriseUser> GetUserByIdAsync(string id)
    {

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", id);

            var value = await GetAsync<string>(GET_USER_BY_ID, parameters);

            if (value == null)
            {
                return null;
            }

            var returnValue = JsonConvert.DeserializeObject<Core2EnterpriseUser>(value);
            return returnValue;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }
    
    public async Task<bool> CheckIfUserExistsAsync(string id, string username)
    {

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", id);
            parameters.Add("@UserName", username);

            return await GetAsync<bool>(CHECK_USER_EXISTS_BY_ID_AND_USERNAME, parameters);

        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    public async Task<Core2EnterpriseUser> GetUserByQueryItemAsync(string id)
    {

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", id);

            var value = await GetAsync<string>(GET_USER_BY_ID, parameters);

            var returnValue = JsonConvert.DeserializeObject<Core2EnterpriseUser>(value);
            return returnValue;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }
    public async Task UpdateUserByIdAsync(string id, Core2EnterpriseUser user)
    {

        try
        {
            var valor = JsonConvert.SerializeObject(user);
            var parameters = new DynamicParameters();
            parameters.Add("@UserData", valor);
            parameters.Add("@UserId", id);

            await ExecuteAsync(UPDATE_USER_BY_ID, parameters);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }
    public async Task DeleteUserByIdAsync(string id)
    {

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", id);

            await ExecuteAsync(DELETE_USER_BY_ID, parameters);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }
}