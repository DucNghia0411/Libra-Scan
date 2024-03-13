using LIBRA.Scan.API.Entities.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service.Constracts
{
    public interface IRolesService
    {
        Task<long> Create(string roleName);
        Task<IEnumerable<Role>> GetAll();
        Task<long> AddUserToRole(long userId, long roleId);
        Task<IList<string>> GetRolesByUser(long userId);    
    }
}
