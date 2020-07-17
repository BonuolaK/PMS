using PMS.Shared.Models;
using PMS.Shared.Service;
using Proj.Core.Dtos;
using Proj.Core.Enums;
using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proj.Core
{
    public interface IProjectService : IBaseService<ProjectCreateDto, Project, ProjectDto>
    {
        ServiceResultModel<Project> TryUpdateStatus(int Id, ProjectState projectStatus = default);

        void DeleteSubProject(int subProjectId);
    }
}
