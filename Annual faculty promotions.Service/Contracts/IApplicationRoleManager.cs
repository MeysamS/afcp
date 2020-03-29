using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;
using Microsoft.AspNet.Identity;

namespace Annual_faculty_promotions.Service.Contracts
{
    public interface IApplicationRoleManager : IDisposable
    {
        /// <summary>
        /// Used to validate roles before persisting changes
        /// </summary>
        IIdentityValidator<CustomRole> RoleValidator { get; set; }

        /// <summary>
        /// Returns an IQueryable of roles if the store is an IQueryableRoleStore
        /// </summary>
        IQueryable<CustomRole> Roles { get; }

        /// <summary>
        /// Create a role
        /// </summary>
        /// <param name="role"/>
        /// <returns/>
        Task<IdentityResult> CreateAsync(CustomRole role);

        /// <summary>
        /// Update an existing role
        /// </summary>
        /// <param name="role"/>
        /// <returns/>
        Task<IdentityResult> UpdateAsync(CustomRole role);

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="role"/>
        /// <returns/>
        Task<IdentityResult> DeleteAsync(CustomRole role);

        /// <summary>
        /// Returns true if the role exists
        /// </summary>
        /// <param name="roleName"/>
        /// <returns/>
        Task<bool> RoleExistsAsync(string roleName);

        /// <summary>
        /// Find a role by id
        /// </summary>
        /// <param name="roleId"/>
        /// <returns/>
        Task<CustomRole> FindByIdAsync(int roleId);

        /// <summary>
        /// Find a role by name
        /// </summary>
        /// <param name="roleName"/>
        /// <returns/>
        Task<CustomRole> FindByNameAsync(string roleName);


        // Our new custom methods

        CustomRole FindRoleByName(string roleName);
        IdentityResult CreateRole(CustomRole role);
        IList<CustomUserRole> GetCustomUsersInRole(string roleName);
        IList<AppUser> GetApplicationUsersInRole(string roleName);
        IList<CustomRole> FindUserRoles(int userId);
        string[] GetRolesForUser(int userId);
        bool IsUserInRole(int userId, string roleName);
        Task<List<CustomRole>> GetAllCustomRolesAsync();

        IQueryable<CustomRole> GetAllCustomRolesAsQueryable();
        IQueryable<CustomUserRole> GetAllCustomUserRole();
        bool AddUserRole(int userId, int roleId, int structId);
    }
}