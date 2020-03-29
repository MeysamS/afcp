using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;


namespace Annual_faculty_promotions.Data.Mapping
{
  public  class CartableMap:EntityTypeConfiguration<Cartable>
  {
      public CartableMap()
      {
          this.HasKey(c => c.Id);

          this.HasRequired(c=>c.Request)
              .WithMany(c=>c.Cartables)
              .HasForeignKey(c=>c.RequestId)
              .WillCascadeOnDelete();


          this.HasRequired(c=>c.UserSender)
              .WithMany(c => c.UserSenderCartables)
              .HasForeignKey(c=>c.UserSenderId)
              .WillCascadeOnDelete(false);


          this.HasRequired(c => c.UserRecive)
              .WithMany(c => c.UserReciveCartables)
              .HasForeignKey(c => c.UserReciveId)
              .WillCascadeOnDelete(false);
      }
    }
}
