using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Data;
using Annual_faculty_promotions.Service.Contracts;


namespace Annual_faculty_promotions.Service.Implemention
{
    public class EfProfileService : IProfileService
    {
        IUnitOfWork _uow;
        readonly IDbSet<Profile> _profiles;
        public EfProfileService(IUnitOfWork uow)
        {
            _uow = uow;
            _profiles = _uow.Set<Profile>();
        }

        public void AddNewProfile(Profile profile)
        {
            _profiles.Add(profile);
        }

        public void AddorUpdateProfile(Profile profile)
        {
            _profiles.AddOrUpdate(profile);
        }
        public IQueryable<Profile> Where(Expression<Func<Profile, bool>> predicate)
        {
            try
            {
                return _profiles.Where(predicate);
            }
            catch
            {
                return null;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var entity = _profiles.Find(id);
                _profiles.Remove(entity);
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public Profile Find(Expression<Func<Profile, bool>> predicate)
        {
            return _profiles.FirstOrDefault(predicate);
        }

        public Profile Find(int? id)
        {
            return _profiles.Find(id);
        }

        public bool HasProfile(int userId)
        {
            bool isexist = true;
            var p = _profiles.Find(userId);
            if (p == null)
            {
                isexist= false;
            }
            return isexist;
        }


        public void EditProfile(Profile opinion)
        {
            _profiles.AddOrUpdate(c => c.Id, opinion);
        }

    }
}
