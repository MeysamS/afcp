using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Data.Mapping
{
    public class DefinitionsMap:EntityTypeConfiguration<Definitions>
   {
       public DefinitionsMap()
       {
           this.HasKey(c => c.Id);
       }
    }
}
