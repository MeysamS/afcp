using Annual_faculty_promotions.Core.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Service.Contracts
{
    public interface IBaseUserService
    {
        void AddNewBaseUserLogin(BaseUserLogin baseuserLogin);
        IList<BaseUserLogin> GetAllBaseUserLogin();
        IQueryable<BaseUserLogin> GetAllBaseUserLoginAsQueryable();
        IQueryable<BaseUserLogin> Where(Expression<Func<BaseUserLogin, bool>> predicate);
        bool Delete(int id);
        BaseUserLogin Find(int? id);
        void Edit(BaseUserLogin bul);
    }
}
