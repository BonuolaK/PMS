using Microsoft.EntityFrameworkCore;
using PMS.Shared.DataAccess.EF;
using System;
using System.Collections.Generic;
using System.Text;
using TaskSvc.Core.Models;

namespace TaskSvc.Core.Database
{
    public class TaskDbContext : BaseContext
    {
        public DbSet<PMSTask> Tasks { get; set; }

        public DbSet<SubTask> SubTasks { get; set; }

        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubTask>().HasOne(x => x.Parent).WithMany(x => x.SubTasks).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
