using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;

namespace Annual_faculty_promotions.Service.Implemention
{
  public   class EfCartableService:ICartableService
    {
        IUnitOfWork _uow;
        readonly IDbSet<Cartable> _cartables;
        public EfCartableService(IUnitOfWork uow)
        {
            _uow = uow;
            _cartables = _uow.Set<Cartable>();
        }

        public void AddNewCartable(Cartable cartable)
        {
            _cartables.Add(cartable);
        }

        public IList<Cartable> GetAllCartables()
        {
            return _cartables.ToList();
        }

        public IQueryable<Cartable> GetAllCartablesAsQueryable()
        {
            return _cartables.AsQueryable();
        }

        public bool Delete(long id)
        {
            try
            {
                var entity = _cartables.Find(id);
                _cartables.Remove(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Cartable Find(long? id)
        {
            return _cartables.Find(id);
        }

        public IQueryable<Cartable> Where(Expression<Func<Cartable, bool>> predicate)
        {
            try
            {
                return _cartables.Where(predicate);
            }
            catch
            {
                return null;
            }
        }

        public void EditCartable(Cartable cartable)
        {
            _cartables.AddOrUpdate(c => c.Id, cartable);
        }
    }
}
