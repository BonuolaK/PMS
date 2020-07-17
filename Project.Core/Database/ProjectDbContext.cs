using Microsoft.EntityFrameworkCore;
using PMS.Shared.DataAccess.EF;
using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proj.Core.Database
{
    public class ProjectDbContext : BaseContext
    {
        public DbSet<Project> Projects { get; set; }

        public DbSet<SubProject> SubProjects { get; set; }

        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubProject>().HasOne(x => x.Parent).WithMany(x => x.SubProjects).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
