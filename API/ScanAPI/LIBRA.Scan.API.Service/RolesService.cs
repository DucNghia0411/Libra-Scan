using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service
{
    public class RolesService : IRolesService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesService
        (
            RoleManager<Role> roleManager,
            UserManager<User> userManager
        )
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        public async Task<long> Create(string roleName)
        {
            Role identityRole = new Role
            {
                Name = roleName
            };

            try
            {
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                    return (long)Code.OK;
                else
                    return (long)Code.InternalServerError;
            }
            catch (Exception)
            {
                return (long)Code.InternalServerError;
            }
        }

        public async Task<IEnumerable<Role>> GetAll()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<long> AddUserToRole(long userId, long roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if(role == null)
            {
                return (long)Code.NotFound;
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (role == null)
            {
                return (long)Code.NotFound;
            }

            try
            {
                IdentityResult result = await _userManager.AddToRoleAsync(user, role.Name);

                if (result.Succeeded)
                    return (long)Code.OK;
                else
                    return (long)Code.InternalServerError;
            }
            catch (Exception)
            {
                return (long)Code.InternalServerError;
            }
        }

        public async Task<IList<string>> GetRolesByUser(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var roles = await _userManager.GetRolesAsync(user);

            return roles;
        }
    }
}
