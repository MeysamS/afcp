using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;


namespace Annual_faculty_promotions.Service.Implemention
{
    public class EfUserService : IUserService
    {
        private IUnitOfWork _uow;
        private readonly IDbSet<AppUser> _appUsers;
        private readonly IDbSet<CustomUserRole> _userRoles;
        private readonly IDbSet<BaseUserLogin> _baseUserLogins;
        public EfUserService(IUnitOfWork uow)
        {
            _uow = uow;
            _appUsers = uow.Set<AppUser>();
            _userRoles = uow.Set<CustomUserRole>();
            _baseUserLogins = uow.Set<BaseUserLogin>();
        }


        public IList<AppUser> GetAllUsers()
        {
            return _appUsers.Include(x => x.Profile).ToList();
        }

        public IQueryable<AppUser> GetAllUsersAsQueryable()
        {
            return _appUsers.Include(p => p.Profile).AsQueryable();
        }

        public bool Delete(int id)
        {
            try
            {
                var entity = _appUsers.Find(id);
                _appUsers.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public AppUser Find(int id)
        {
            return _appUsers.Find(id);
        }

        public AppUser Find(Expression<Func<AppUser, bool>> predicate)
        {
            return _appUsers.FirstOrDefault(predicate);
        }

        public IQueryable<AppUser> Where(Expression<Func<AppUser, bool>> predicate)
        {
            try
            {
                return _appUsers.Where(predicate);
            }
            catch
            {
                return null;
            }
        }


        public IQueryable<BaseUserLogin> Where(Expression<Func<BaseUserLogin, bool>> predicate)
        {
            try
            {
                return _baseUserLogins.Where(predicate);
            }
            catch
            {
                return null;
            }
        }

        public bool FindBaseUserLogin(int codemeli, string codeestekhdami)
        {
            var result = _baseUserLogins.Where(c => c.CodeMeli == codemeli && c.CodeEstekhdam == codeestekhdami);
            if (result != null)
                return true;
            return false;
        }

        public void EditUser(AppUser user)
        {
            _appUsers.AddOrUpdate(c => c.Id, user);
        }



        public bool Delete(long id)
        {
            try
            {
                var entity = _appUsers.Find(id);
                _appUsers.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public List<CustomUserRole> GetUserRoles(int uid)
        {
            return _userRoles.Where(x => x.UserId == uid).ToList();
        }

        public bool DeleteUserRole(int userId, int roleId, int departmentId)
        {
            try
            {
                var entity =
                    _userRoles.SingleOrDefault(x => x.UserId == userId && x.RoleId == roleId && x.Department.Id == departmentId);
                _userRoles.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
