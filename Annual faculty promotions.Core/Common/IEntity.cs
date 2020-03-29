using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Core.Common
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
