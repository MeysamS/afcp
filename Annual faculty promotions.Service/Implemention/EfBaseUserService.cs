using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Service.Contracts;
using System.Linq.Expressions;
using System.Data.Entity.Migrations;

namespace Annual_faculty_promotions.Service.Implemention
{
    public class EfBaseUserService : IBaseUserService
    {
       private IUnitOfWork _uow;
       private readonly IDbSet<BaseUserLogin> _baseUserLogins;

        public EfBaseUserService(IUnitOfWork uow)
        {
            _uow = uow;
            _baseUserLogins = uow.Set<BaseUserLogin>();
        }

        public void AddNewBaseUserLogin(BaseUserLogin baseuserLogin)
        {
            _baseUserLogins.Add(baseuserLogin);
        }
        public IList<BaseUserLogin> GetAllBaseUserLogin()
        {
            return _baseUserLogins.ToList();
        }
        public IQueryable<BaseUserLogin> GetAllBaseUserLoginAsQueryable()
        {
            return _baseUserLogins.AsQueryable();
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
        public BaseUserLogin Find(int? id)
        {
            return _baseUserLogins.Find(id);
        }                
        public void Edit(BaseUserLogin bul)
        {
            _baseUserLogins.AddOrUpdate(c => c.BaseUserLoginId, bul);
        }
        public bool Delete(int id)
        {
            try
            {
                var entity = _baseUserLogins.Find(id);
                _baseUserLogins.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

    }
}
