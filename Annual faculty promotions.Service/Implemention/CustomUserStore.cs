
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Service.Contracts;
using Microsoft.AspNet.Identity;

namespace Annual_faculty_promotions.Service.Implemention
{
    public class CustomUserStore : ICustomUserStore
    {
        private readonly IUserStore<AppUser, int> _userStore;

        public CustomUserStore(IUserStore<AppUser, int> userStore)
        {
            _userStore = userStore;
        }


    }
}