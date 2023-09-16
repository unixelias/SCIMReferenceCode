using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.SCIM.Repository.ScimResources;

public interface IGroupRepository
{
    public Task CreateAsync(Core2Group group);
    public Task<Core2Group> GetGroupByIdAsync(string id);
    public Task UpdateGroupByIdAsync(string id, Core2Group group);
    public Task<IEnumerable<Core2Group>> ListAllAsync();
    public Task<bool> CheckIfGroupExistsAsync(string id, string groupName);
    public Task DeleteGroupByIdAsync(string id);
}