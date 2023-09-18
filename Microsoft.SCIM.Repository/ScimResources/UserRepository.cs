using Dapper;
using Microsoft.SCIM.Repository.Utils;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Repository.ScimResources;

public class UserRepository : DatabaseRepositoryBasic, IUserRepository
{
    #region SQL Eager Loading

    private static readonly string INSERT_USER = DatabaseHelper.LoadSqlStatement("InsertUser.sql", typeof(UserRepository).Namespace);

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
            return;
        }
    }
}