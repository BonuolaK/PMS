using PMS.Shared.Models;
using Proj.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proj.Core.Models
{
    public class Project : BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public ProjectState State { get; set; }

        public ICollection<SubProject> SubProjects { get; set; }

        public bool HasTask { get; set; }
    }
}
