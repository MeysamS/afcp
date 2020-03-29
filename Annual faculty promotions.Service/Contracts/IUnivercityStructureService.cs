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
    public interface IUnivercityStructureService
    {
        void AddNewUnivercityStructure(UnivercityStructure univercityStructure);
        IList<UnivercityStructure> GetAllUnivercityStructures();
        IQueryable<UnivercityStructure> GetAllUnivercityStructuresAsQueryable();

        bool Delete(int id);

        UnivercityStructure Find(int? id);
        IQueryable<UnivercityStructure> Where(Expression<Func<UnivercityStructure, bool>> predicate);
        void EditUnivercityStructure(UnivercityStructure univercityStructure);

        IEnumerable<UnivercityStructure> GetUnivercityStructures(int? parentId);
    }
}
