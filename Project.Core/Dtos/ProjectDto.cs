using PMS.Shared.Service;
using Proj.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proj.Core.Dtos
{
    public class ProjectDto : BaseDto
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public ProjectState State { get; set; }

        public List<ProjectDto> SubProjects { get; set; }

    }
}
