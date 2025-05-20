using Microsoft.AspNetCore.Identity;
using Motel.DTO;
using Motel.Models;

namespace Motel.Interfaces
{
    public interface IRoleRepository
    {
        Task<bool> CreateRole(string roleName);
        Task<bool> DeleteRole(Guid id);
        Task<bool> UpdateRole(Guid id, RoleDTO role);
        Task<IList<IdentityRole>> GetRolesByUser(string userId);
        List<ApplicationRole> GetRoles();
        int Count();
        Task<bool> SetRole(Guid userId, List<Guid> rolesId);
    }
}
