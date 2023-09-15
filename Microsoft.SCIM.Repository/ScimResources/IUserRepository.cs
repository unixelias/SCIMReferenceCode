using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Repository.ScimResources;

public interface IUserRepository
{
    public Task CreateAsync(Core2EnterpriseUser user);
    public Task<Core2EnterpriseUser> GetUserByIdAsync(string id);
    public Task UpdateUserByIdAsync(string id, Core2EnterpriseUser user);
    public Task<IEnumerable<Core2EnterpriseUser>> ListAllAsync();
    public Task<bool> CheckIfUserExistsAsync(string id, string username);
    public Task DeleteUserByIdAsync(string id);
}