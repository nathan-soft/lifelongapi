using LifeLongApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LifeLongApi.Data {
    public class IdentityAppContext : IdentityDbContext<AppUser, AppRole, int> {
        public IdentityAppContext (DbContextOptions<IdentityAppContext> options) : base (options) {

        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating (modelBuilder);

            modelBuilder.Entity<Follow> ()
                .HasIndex (f => new { f.MenteeId, f.MentorId, f.TopicId })
                .IsUnique();

            // modelBuilder.Entity<Follow> ()
            //     .HasOne (f => f.Mentee)
            //     .WithMany (a => a.Mentees)
            //     .HasForeignKey (f => f.MenteeId);

            // modelBuilder.Entity<Follow> ()
            //     .HasOne (f => f.Mentor)
            //     .WithMany (a => a.Mentors)
            //     .HasForeignKey (f => f.MentorId);

            modelBuilder.Entity<UserFieldOfInterest>()
                .HasKey(uf => new { uf.UserId, uf.TopicId });

            modelBuilder.Entity<Qualification>()
                .HasIndex(Q => new { Q.UserId, Q.SchoolName, Q.QualificationType, Q.Major }).IsUnique();

            modelBuilder.Entity<WorkExperience>()
                .HasIndex(W => new { W.UserId, W.CompanyName, W.TopicId, W.StartYear }).IsUnique();
        }

        public DbSet<Follow> Follows { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<UserFieldOfInterest> UserFieldOfInterests { get; set; }
        public DbSet<WorkExperience> WorkExperiences { get; set; }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync( bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            OnBeforeSaving();
            return (await base.SaveChangesAsync(acceptAllChangesOnSuccess,
                          cancellationToken));
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                // for entities that inherit from BaseEntity,
                // set UpdatedOn / CreatedOn appropriately
                if (entry.Entity is BaseEntity trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // set the updated date to "now"
                            trackable.UpdatedOn = utcNow;

                            // mark property as "don't touch"
                            // we don't want to update on a Modify operation
                            entry.Property("CreatedOn").IsModified = false;
                            break;

                        case EntityState.Added:
                            // set both updated and created date to "now"
                            trackable.CreatedOn = utcNow;
                            trackable.UpdatedOn = utcNow;
                            break;
                    }
                }
            }
        }
    }
}