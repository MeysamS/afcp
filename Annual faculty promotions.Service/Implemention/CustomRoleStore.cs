
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Service.Contracts;
using Microsoft.AspNet.Identity;

namespace Annual_faculty_promotions.Service.Implemention
{
    public class CustomRoleStore : ICustomRoleStore
    {
        private readonly IRoleStore<CustomRole, int> _roleStore;

        public CustomRoleStore(IRoleStore<CustomRole, int> roleStore)
        {
            _roleStore = roleStore;
        }
    }
}