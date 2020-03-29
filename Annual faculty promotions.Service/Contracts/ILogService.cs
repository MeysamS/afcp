using Annual_faculty_promotions.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Service.Contracts
{
    public interface ILogService
    {
        void AddNewLog(Log log);
        IList<Log> GetAllLog();
        IQueryable<Log> GetAllLogAsQueryable();
        IQueryable<Log> Where(Expression<Func<Log, bool>> predicate);
        Log Find(int? id);
    }
}
