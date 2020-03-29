using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Service.Contracts;
using Annual_faculty_promotions.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq.Expressions;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Service.Implemention
{
    public class EfDefinitionService:IDefinitionService
    {
        IUnitOfWork _uow;
        readonly IDbSet<Definitions> _definitions;
        public EfDefinitionService(IUnitOfWork uow)
        {
            _uow = uow;
            _definitions = _uow.Set<Definitions>();
        }
        //public void AddNewDefinition(Definitions definition)
        //{
        //    _definitions.Add(definition);
        //}
        public IList<Definitions> GetAllDefinitions()
        {
            return _definitions.ToList();
        }
        public IQueryable<Definitions> GetAllDefinitionsAsQueryable()
        {
            return _definitions.AsQueryable();
        }
        public bool Delete(int id)
        {
            try
            {
                var entity = _definitions.Find(id);
                _definitions.Remove(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Definitions Find(int? id)
        {
            return _definitions.Find(id);
        }
        public IQueryable<Definitions> Where(Expression<Func<Definitions, bool>> predicate)
        {
            try
            {
                return _definitions.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public void AddorUpdateDefinitions(Definitions definition)
        {
            _definitions.AddOrUpdate(definition);
        }
    }
}
