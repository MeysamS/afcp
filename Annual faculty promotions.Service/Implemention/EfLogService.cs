using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace Annual_faculty_promotions.Service.Implemention
{
    public class EfLogService:ILogService
    {
        private IUnitOfWork _uow;
        private readonly IDbSet<Log> _log;
        public EfLogService(IUnitOfWork uow)
        {
            _uow = uow;
            _log = uow.Set<Log>();
        }

        public void AddNewLog(Log log)
        {
            _log.Add(log);
        }
        public IList<Log> GetAllLog()
        {
            return _log.ToList();
        }
        public IQueryable<Log> GetAllLogAsQueryable()
        {
            return _log.AsQueryable();
        }
        public IQueryable<Log> Where(Expression<Func<Log, bool>> predicate)
        {
            try
            {
                return _log.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public Log Find(int? id)
        {
            return _log.Find(id);
        }
    }
}
