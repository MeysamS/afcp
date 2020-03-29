using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;

namespace Annual_faculty_promotions.Service.Contracts
{
    public interface IArchiveService
    {
        void AddNewArchive(Archive archive);
        IList<Archive> GetAllArchives();
        IQueryable<Archive> GetAllArchivesAsQueryable();

        bool Delete(int id);

        Archive Find(int? id);
        IQueryable<Archive> Where(Expression<Func<Archive, bool>> predicate);
        void EditArchive(Archive archive);

    }
}
