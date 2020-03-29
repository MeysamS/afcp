using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Service.Contracts
{
  public   interface ICartableService
    {
        void AddNewCartable(Cartable cartable);
        IList<Cartable> GetAllCartables();
        IQueryable<Cartable> GetAllCartablesAsQueryable();

        bool Delete(long id);

        Cartable Find(long? id);
        IQueryable<Cartable> Where(Expression<Func<Cartable, bool>> predicate);
        void EditCartable(Cartable cartable);
    }
}
