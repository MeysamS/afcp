using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;


namespace Annual_faculty_promotions.Service.Contracts
{
    public interface IUserService
    {
        IList<AppUser> GetAllUsers();
        IQueryable<AppUser> GetAllUsersAsQueryable();

        bool Delete(int id);

        AppUser Find(int id);

        IQueryable<AppUser> Where(Expression<Func<AppUser, bool>> predicate);

        void EditUser(AppUser category);

        IQueryable<BaseUserLogin> Where(Expression<Func<BaseUserLogin, bool>> predicate);
        bool FindBaseUserLogin(int codemeli, string codeestekhdami);


        List<CustomUserRole> GetUserRoles(int uid);

        bool DeleteUserRole(int userId,int roleId,int departmentId);

    }
}
