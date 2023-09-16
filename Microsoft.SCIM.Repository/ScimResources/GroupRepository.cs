using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.SCIM.Repository.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Repository.ScimResources;

public class GroupRepository : DatabaseRepositoryBasic, IGroupRepository
{
    public GroupRepository(IConfiguration configuration) : base(configuration)
    {
    }

    #region SQL Eager Loading

    private static readonly string INSERT_GROUP = DatabaseHelper.LoadSqlStatement("InsertGroup.sql", typeof(GroupRepository).Namespace);
    private static readonly string LIST_ALL_GROUPS = DatabaseHelper.LoadSqlStatement("ListAllGroups.sql", typeof(GroupRepository).Namespace);
    private static readonly string GET_GROUP_BY_ID = DatabaseHelper.LoadSqlStatement("GetGroupById.sql", typeof(GroupRepository).Namespace);
    private static readonly string CHECK_GROUP_EXISTS_BY_ID_AND_NAME = DatabaseHelper.LoadSqlStatement("CheckIfGroupExists.sql", typeof(GroupRepository).Namespace);
    private static readonly string UPDATE_GROUP_BY_ID = DatabaseHelper.LoadSqlStatement("UpdateGroupById.sql", typeof(GroupRepository).Namespace);
    private static readonly string DELETE_GROUP_BY_ID = DatabaseHelper.LoadSqlStatement("DeleteGroup.sql", typeof(GroupRepository).Namespace);

    #endregion SQL Eager Loading

    public async Task CreateAsync(Core2Group group)
    {
        try
        {
            var valor = JsonConvert.SerializeObject(group);
            var parameters = new DynamicParameters();
            parameters.Add("@GroupData", valor);

            await ExecuteAsync(INSERT_GROUP, parameters);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    public async Task<IEnumerable<Core2Group>> ListAllAsync()
    {
        try
        {
            var value = await ListAsync<string>(LIST_ALL_GROUPS);

            var returnValue = value.Select(group => JsonConvert.DeserializeObject<Core2Group>(group));

            return returnValue;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    public async Task<Core2Group> GetGroupByIdAsync(string id)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@GroupId", id);

            var value = await GetAsync<string>(GET_GROUP_BY_ID, parameters);
            if (value == null)
            {
                return null;
            }
            var returnValue = JsonConvert.DeserializeObject<Core2Group>(value);
            return returnValue;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    public async Task<bool> CheckIfGroupExistsAsync(string id, string groupName)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@GroupId", id);
            parameters.Add("@DisplayName", groupName);

            return await GetAsync<bool>(CHECK_GROUP_EXISTS_BY_ID_AND_NAME, parameters);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    public async Task<Core2Group> GetUserByQueryItemAsync(string id)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@GroupId", id);

            var value = await GetAsync<string>(GET_GROUP_BY_ID, parameters);

            var returnValue = JsonConvert.DeserializeObject<Core2Group>(value);
            return returnValue;
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    public async Task UpdateGroupByIdAsync(string id, Core2Group group)
    {
        try
        {
            var valor = JsonConvert.SerializeObject(group);
            var parameters = new DynamicParameters();
            parameters.Add("@GroupData", valor);
            parameters.Add("@GroupId", id);

            await ExecuteAsync(UPDATE_GROUP_BY_ID, parameters);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }

    public async Task DeleteGroupByIdAsync(string id)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@GroupId", id);

            await ExecuteAsync(DELETE_GROUP_BY_ID, parameters);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(ex.Message, ex);
        }
    }
}