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
    public class EfMessagingService : IMessagingService
    {
        private IUnitOfWork _uow;
        private readonly IDbSet<Messaging> _messaging;

        public EfMessagingService(IUnitOfWork uow)
        {
            _uow = uow;
            _messaging = uow.Set<Messaging>();
        }

        public void AddNewMessaging(Messaging messaging)
        {
            _messaging.Add(messaging);
        }
        public IList<Messaging> GetAllMessaging()
        {
            return _messaging.ToList();
        }
        public IQueryable<Messaging> GetAllMessagingAsQueryable()
        {
            return _messaging.AsQueryable();
        }
        public IQueryable<Messaging> Where(Expression<Func<Messaging, bool>> predicate)
        {
            try
            {
                return _messaging.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public Messaging Find(long id)
        {
            return _messaging.Find(id);
        }
        public void Edit(Messaging messaging)
        {
            _messaging.AddOrUpdate(c => c.Id, messaging);
        }
        public bool Delete(long id)
        {
            try
            {
                var entity = _messaging.Find(id);
                _messaging.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

    }
}
