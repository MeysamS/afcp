using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;

namespace Annual_faculty_promotions.Service.Contracts
{
    public interface IProfileService
    {
        void AddNewProfile(Profile profile);
        void AddorUpdateProfile(Profile profile);
        IQueryable<Profile> Where(Expression<Func<Profile, bool>> predicate);
        bool Delete(int id);

        Profile Find(Expression<Func<Profile, bool>> predicate);
        Profile Find(int? id);

        bool HasProfile(int userId);

        void EditProfile(Profile blog);
    }
}
