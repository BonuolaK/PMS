using PMS.Shared.DataAccess;
using PMS.Shared.Models;
using Proj.Core.Dtos;
using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Proj.Core
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepo;
        public ProjectService(IRepository<Project> projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public ServiceResultModel<Project> Create(ProjectCreateDto model)
        {
            throw new NotImplementedException();
        }

        public ServiceResultModel<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Project Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProjectDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProjectDto> GetAll(Expression<Func<Project, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public ServiceResultModel<Project> Update(ProjectCreateDto model)
        {
            throw new NotImplementedException();
        }
    }
}
