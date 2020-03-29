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
    public class EfStageService : IStageService
    {
       IUnitOfWork _uow;
        readonly IDbSet<Stage> _stages;
        public EfStageService(IUnitOfWork uow)
        {
            _uow = uow;
            _stages = _uow.Set<Stage>();
        }

        public void AddNewStage(Stage stage)
        {
            _stages.Add(stage);
        }



        public IList<Stage> GetAllStages()
        {
            return _stages.ToList();
        }

        public IQueryable<Stage> GetAllStagesAsQueryable()
        {
            return _stages.AsQueryable();
        }

        public bool Delete(int id)
        {
            try
            {
                var entity = _stages.Find(id);
                _stages.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
          
        }

        public Stage Find(int? id)
        {
            return _stages.Find(id);
        }
        public IQueryable<Stage> Where(Expression<Func<Stage, bool>> predicate)
        {
            try
            {
                return _stages.Where(predicate);
            }
            catch
            {
                return null;
            }
        }
        public void EditStage(Stage stage)
        {
            _stages.AddOrUpdate(c => c.Id, stage);
        }
    }
}
