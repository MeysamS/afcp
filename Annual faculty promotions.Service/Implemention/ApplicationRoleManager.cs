using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using Microsoft.AspNet.Identity;

namespace Annual_faculty_promotions.Service.Implemention
{
    public class ApplicationRoleManager : RoleManager<CustomRole, int>, IApplicationRoleManager
    {
        private readonly IUnitOfWork _uow;
        private readonly IRoleStore<CustomRole, int> _roleStore;
        private readonly IDbSet<CustomUserRole> _userRoles;
        private readonly IDbSet<UnivercityStructure> _univercityStructures;
        private readonly IDbSet<AppUser> _users;
        public ApplicationRoleManager(IUnitOfWork uow, IRoleStore<CustomRole, int> roleStore)
            : base(roleStore)
        {
            _uow = uow;
            _roleStore = roleStore;
            _users = _uow.Set<AppUser>();
            _univercityStructures = _uow.Set<UnivercityStructure>();
            _userRoles = _uow.Set<CustomUserRole>();
        }

        public CustomRole FindRoleByName(string roleName)
        {
            return this.FindByName(roleName); // RoleManagerExtensions
        }

        public IdentityResult CreateRole(CustomRole role)
        {
            return this.Create(role); // RoleManagerExtensions
        }

        public IList<CustomUserRole> GetCustomUsersInRole(string roleName)
        {
            return this.Roles.Where(role => role.Name == roleName)
                             .SelectMany(role => role.Users)
                             .ToList();
            // = this.FindByName(roleName).Users
        }

        public IList<AppUser> GetApplicationUsersInRole(string roleName)
        {
            var roleUserIdsQuery = from role in this.Roles
                                   where role.Name == roleName
                                   from user in role.Users
                                   select user.UserId;
            return _users.Where(applicationUser => roleUserIdsQuery.Contains(applicationUser.Id))
                         .ToList();
        }

        public IList<CustomRole> FindUserRoles(int userId)
        {
            var userRolesQuery = from role in this.Roles
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;

            return userRolesQuery.OrderBy(x => x.Name).ToList();
        }

        public string[] GetRolesForUser(int userId)
        {
            var roles = FindUserRoles(userId);
            if (roles == null || !roles.Any())
            {
                return new string[] { };
            }

            return roles.Select(x => x.Name).ToArray();
        }

        public bool IsUserInRole(int userId, string roleName)
        {
            var userRolesQuery = from role in this.Roles
                                 where role.Name == roleName
                                 from user in role.Users
                                 where user.UserId == userId
                                 select role;
            var userRole = userRolesQuery.FirstOrDefault();
            return userRole != null;
        }

        public Task<List<CustomRole>> GetAllCustomRolesAsync()
        {
            return this.Roles.ToListAsync();
        }

        public IQueryable<CustomRole> GetAllCustomRolesAsQueryable()
        {
            return this.Roles.AsQueryable();
        }


        public IQueryable<CustomUserRole> GetAllCustomUserRole()
        {
            return _userRoles.AsQueryable();
        }


        public bool AddUserRole(int userId, int roleId, int structId)
        {
            try
            {
                CustomUserRole customUserRole = new CustomUserRole();
                customUserRole.UserId = userId;
                customUserRole.RoleId = roleId;
                customUserRole.Department = _univercityStructures.Find(structId);
                _userRoles.Add(customUserRole);
                _uow.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}