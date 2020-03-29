using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;


namespace Annual_faculty_promotions.Service.Implemention
{
    public class EfUnivercityStructureService : IUnivercityStructureService
    {
       IUnitOfWork _uow;
        readonly IDbSet<UnivercityStructure> _univercity;
        public EfUnivercityStructureService(IUnitOfWork uow)
        {
            _uow = uow;
            _univercity = _uow.Set<UnivercityStructure>();
        }

        public void AddNewUnivercityStructure(UnivercityStructure univercityStructure)
        {
            _univercity.Add(univercityStructure);
        }

        public IList<UnivercityStructure> GetAllUnivercityStructures()
        {
            return _univercity.ToList();
        }

        public IQueryable<UnivercityStructure> GetAllUnivercityStructuresAsQueryable()
        {
            return _univercity.AsQueryable();
        }

        public bool Delete(int id)
        {
            try
            {
                var entity = _univercity.Find(id);
                _univercity.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
          
        }

        public UnivercityStructure Find(int? id)
        {
            return _univercity.Find(id);
        }

        public IQueryable<UnivercityStructure> Where(Expression<Func<UnivercityStructure, bool>> predicate)
        {
            try
            {
                return _univercity.Where(predicate);
            }
            catch
            {
                return null;
            }
        }

        public void EditUnivercityStructure(UnivercityStructure univercityStructure)
        {
            _univercity.AddOrUpdate(c => c.Id, univercityStructure);
        }



        public IEnumerable<UnivercityStructure> GetUnivercityStructures(int? parentId)
        {
            try
            {
                return _univercity.Where(p => p.ParentId == parentId).ToList();
            }
            catch (Exception)
            {

                return null;
            }
        }

    }
}
