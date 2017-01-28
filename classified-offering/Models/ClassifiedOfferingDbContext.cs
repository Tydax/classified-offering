using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace classified_offering.Models
{
    public class ClassifiedOfferingDbContext : DbContext
    {
        public DbSet<ClassifiedOffering> ClassifiedOfferings { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClassifiedOffering>()
                .HasRequired(c => c.Creator)
                .WithMany(co => co.CreatedClassifiedOfferings)
                .WillCascadeOnDelete(false);
        }
    }
}