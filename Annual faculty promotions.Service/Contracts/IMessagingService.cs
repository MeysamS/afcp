using Annual_faculty_promotions.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Service.Contracts
{
    public interface IMessagingService
    {
        void AddNewMessaging(Messaging messaging);
        IList<Messaging> GetAllMessaging();
        IQueryable<Messaging> GetAllMessagingAsQueryable();
        IQueryable<Messaging> Where(Expression<Func<Messaging, bool>> predicate);
        bool Delete(long id);
        Messaging Find(long id);
        void Edit(Messaging messaging);
    }
}
