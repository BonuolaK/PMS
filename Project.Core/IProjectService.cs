using PMS.Shared.Service;
using Proj.Core.Dtos;
using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proj.Core
{
    public interface IProjectService : IBaseService<ProjectCreateDto, Project, ProjectDto>
    {

    }
}
