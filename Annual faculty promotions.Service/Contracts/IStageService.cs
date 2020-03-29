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
    public interface IStageService
    {
        void AddNewStage(Stage stage);
        IList<Stage> GetAllStages();
        IQueryable<Stage> GetAllStagesAsQueryable();

        bool Delete(int id);

        Stage Find(int? id);
        IQueryable<Stage> Where(Expression<Func<Stage, bool>> predicate);
        void EditStage(Stage stage);
    }
}
