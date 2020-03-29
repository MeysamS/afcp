using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Domain;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Data.Mapping;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Annual_faculty_promotions.Data
{
    public class AfpContext : IdentityDbContext<AppUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>, IUnitOfWork
    {
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
        public DbSet<UnivercityStructure> UnivercityStructures { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<BaseUserLogin> BaseUserLogins { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<TechnologyDetail> TechnologyDetails { get; set; }
        public DbSet<ScientificExecutive> ScientificExecutives { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<FurtherInformation> FurtherInformations { get; set; }
        public DbSet<EducationalResearch> EducationalResearches { get; set; }
        public DbSet<Dissertation> Dissertations { get; set; }
        public DbSet<AttachmentTechnology> AttachmentTechnologies { get; set; }
        public DbSet<AttachmentResearch> AttachmentResearches { get; set; }
        public DbSet<AttachmentFurtherInformation> AttachmentFurtherInformations { get; set; }
        public DbSet<AttachmentBasicDelayedPreviousYears> AttachmentBasicDelayedPreviousYears { get; set; }
        public DbSet<Access> Access { get; set; }
        public DbSet<Archive> Archive { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Messaging> Messagings { get; set; }
        public DbSet<Definitions> Definitions { get; set; }
        public DbSet<CustomUserRole> UserRoles { get; set; }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        public AfpContext()
            : base("cnnString")
        {
            this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.AutoDetectChangesEnabled = false;

        }

        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
              .Where(x => x.Entity is IAuditableEntity
                  && (x.State == System.Data.Entity.EntityState.Added || x.State == System.Data.Entity.EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                var entity = entry.Entity as IAuditableEntity;
                if (entity != null)
                {
                    string identityName = Thread.CurrentPrincipal.Identity.Name;
                    DateTime now = DateTime.UtcNow;

                    if (entry.State == System.Data.Entity.EntityState.Added)
                    {
                        entity.CreatedBy = identityName;
                        entity.CreatedDate = now;
                    }
                    else
                    {
                        base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                    }

                    entity.UpdatedBy = identityName;
                    entity.UpdatedDate = now;
                }
            }
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UnivercityStructureMap());
            modelBuilder.Configurations.Add(new AppUserMap());
            modelBuilder.Configurations.Add(new BaseUserLoginMap());
            modelBuilder.Configurations.Add(new TechnologyMap());
            modelBuilder.Configurations.Add(new TechnologyDetailMap()); 
            modelBuilder.Configurations.Add(new ScientificExecutiveMap());
            modelBuilder.Configurations.Add(new RequestMap());
            modelBuilder.Configurations.Add(new FurtherInformationMap());
            modelBuilder.Configurations.Add(new EducationalResearchMap());
            modelBuilder.Configurations.Add(new DissertationMap());
            modelBuilder.Configurations.Add(new AttachmentTechnologyMap());
            modelBuilder.Configurations.Add(new AttachmentResearchMap());
            modelBuilder.Configurations.Add(new AttachmentFurtherInformationMap());
            modelBuilder.Configurations.Add(new AttachmentBasicDelayedPreviousYearsMap());
            modelBuilder.Configurations.Add(new CartableMap());
            modelBuilder.Configurations.Add(new AccessMap());
            modelBuilder.Configurations.Add(new CustomRoleMap());
            modelBuilder.Configurations.Add(new ArchiveMap());
            modelBuilder.Configurations.Add(new LogMap());
            modelBuilder.Configurations.Add(new MessagingMap());
            modelBuilder.Configurations.Add(new DefinitionsMap());
            modelBuilder.Configurations.Add(new CustomUserRoleMap());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().ToTable("Users");
            modelBuilder.Entity<CustomRole>().ToTable("Roles");
            modelBuilder.Entity<CustomUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<CustomUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<CustomUserLogin>().ToTable("UserLogins");
        }

        public IEnumerable<TEntity> AddThisRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            return ((DbSet<TEntity>)this.Set<TEntity>()).AddRange(entities);
        }

        public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Modified;
        }

        public IList<T> GetRows<T>(string sql, params object[] parameters) where T : class
        {
            return Database.SqlQuery<T>(sql, parameters).ToList();
        }

        public IList<T> GetRows<T>(string sql) where T : class
        {
            return Database.SqlQuery<T>(sql).ToList();
        }
        public void ForceDatabaseInitialize()
        {
            this.Database.Initialize(force: true);
        }
    }
}
