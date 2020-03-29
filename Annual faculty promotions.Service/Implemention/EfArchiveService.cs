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
    public class EfArchiveService : IArchiveService
    {
       IUnitOfWork _uow;
        readonly IDbSet<Archive> _archives;
        public EfArchiveService(IUnitOfWork uow)
        {
            _uow = uow;
            _archives = _uow.Set<Archive>();
        }

        public void AddNewArchive(Archive archive)
        {
            _archives.Add(archive);
        }



        public IList<Archive> GetAllArchives()
        {
            return _archives.ToList();
        }

        public IQueryable<Archive> GetAllArchivesAsQueryable()
        {
            return _archives.AsQueryable();
        }

        public bool Delete(int id)
        {
            try
            {
                var entity = _archives.Find(id);
                _archives.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
          
        }

        public Archive Find(int? id)
        {
            return _archives.Find(id);
        }

        public IQueryable<Archive> Where(Expression<Func<Archive, bool>> predicate)
        {
            try
            {
                return _archives.Where(predicate);
            }
            catch
            {
                return null;
            }
        }

        public void EditArchive(Archive archive)
        {
            _archives.AddOrUpdate(c => c.Id, archive);
        }


    }
}
