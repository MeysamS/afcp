using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Service.Contracts
{
    public interface IDefinitionService
    {
        //void AddNewDefinition(Definitions definition);
        IList<Definitions> GetAllDefinitions();
        IQueryable<Definitions> GetAllDefinitionsAsQueryable();
        bool Delete(int id);
        Definitions Find(int? id);
        IQueryable<Definitions> Where(Expression<Func<Definitions, bool>> predicate);
        void AddorUpdateDefinitions(Definitions definition);
    }
}
