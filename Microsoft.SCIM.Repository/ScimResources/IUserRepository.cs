using System.Threading.Tasks;

namespace Microsoft.SCIM.Repository.ScimResources;

public interface IUserRepository
{
    public Task CreateAsync(Core2EnterpriseUser user);
}