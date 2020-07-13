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
        public DbSet<Task> Tasks { get; set; }

        public DbSet<SubTask> SubTasks { get; set; }

        public TaskDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
