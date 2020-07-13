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
        public ProjectDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
