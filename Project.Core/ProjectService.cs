using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PMS.Shared.Constants;
using PMS.Shared.DataAccess;
using PMS.Shared.Helpers;
using PMS.Shared.HttpService;
using PMS.Shared.Models;
using Proj.Core.Dtos;
using Proj.Core.Enums;
using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proj.Core
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<SubProject> _subProjRepo;
        private IHttpService _httpService;
        private readonly ILogger<ProjectService> _logger;
        private readonly IConfiguration _configuration;

        public ProjectService(IRepository<Project> projectRepo,
            IRepository<SubProject> subProjectRepo,
            IHttpService httpService,
            ILogger<ProjectService> logger,
            IConfiguration configuration)
        {
            _projectRepo = projectRepo;
            _subProjRepo = subProjectRepo;
            _httpService = httpService;
            _logger = logger;
            _configuration = configuration;
        }


        public ServiceResultModel<Project> Create(ProjectCreateDto model)
        {
            if (model == null)
                throw new ArgumentNullException();

            var project = (Project)model;

            var resultModel = ValidateProject(project, model.SubProjectIds);

            if (resultModel.HasError)
                return resultModel;

            _projectRepo.Insert(project);
            resultModel.Data = project;

            return resultModel;
        }

        public ServiceResultModel<int> Delete(int id)
        {
            var result = new ServiceResultModel<int>();
            var project = Get(id);

            if (project == null)
                throw new ArgumentNullException();

            // find subProjects

            var subProjectCount = _subProjRepo.GetAllIncluding().Where(x => x.ChildId == project.Id).Count();

            if (subProjectCount > 0)
            {
                result.AddError(CommonConstant.ProjectMessages.ProjectExistsAsSubProject);
                return result;
            }

            // validate task exists
            var taskServiceResponse = ValidateTaskExistsForProject(project.Id);
            if (taskServiceResponse.HasError)
            {
                result.AddError(taskServiceResponse.ErrorMessages.ElementAt(0));
                return result;
            }

            if (taskServiceResponse.Data == true)
            {
                result.AddError(CommonConstant.ProjectMessages.TasksBelongToProject);
                return result;
            }

            _projectRepo.Delete(project);
            return result;
        }

        // should utilize some service discovery implementation for more standard work
        private ServiceResultModel<bool> ValidateTaskExistsForProject(int projectId)
        {
            // validate project has tasks -- API CALL
            _logger.LogInformation("---BEGIN API CALL");
            var response = new ServiceResultModel<bool>();

            try
            {
                var uri = _configuration.GetValue<string>(CommonConstant.GetTaskByProjectApiService);
                response.Data = _httpService.GetAsync<bool>(string.Format(uri, projectId)).Result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.InnerException?.Message);
                response.AddError("Error calling task service");
            }

            return response;
        }


        public Project Get(int id)
        {
            return _projectRepo.Get(id);
        }


        public Project Get(Expression<Func<Project, bool>> expression)
        {
            return _projectRepo.Get(expression);
        }

        public IEnumerable<ProjectDto> GetAll()
        {
            return GetAllDto().ToList();
        }

        public IEnumerable<ProjectDto> GetAll(Expression<Func<Project, bool>> expression)
        {
            return GetAllDto(expression).ToList();
        }


        public ServiceResultModel<Project> Update(ProjectCreateDto model)
        {
            if (model == null)
                throw new ArgumentNullException();

            var project = _projectRepo.GetAllIncluding(x => x.SubProjects).Where(x => x.Id == model.Id)
                .Include(x => x.SubProjects).ThenInclude(x => x.Child).FirstOrDefault();

            if (project == null)
                throw new ArgumentNullException();


            project.Name = model.Name;
            project.StartDate = model.StartDate;
            project.Code = model.Code;

            var addedSubProjects = model.SubProjectIds.Except(project.SubProjects.Select(x => x.ChildId)).ToList();

            var resultModel = ValidateProject(project, addedSubProjects);

            if (resultModel.HasError)
                return resultModel;

            _projectRepo.Update(project);

            resultModel.Data = project;

            return resultModel;
        }

        public ServiceResultModel<Project> TryUpdateStatus(int Id, ProjectState projectStatus = default)
        {

            var resultModel = new ServiceResultModel<Project>();

            var project = _projectRepo.GetAllIncluding().Where(x => x.Id == Id)
                .Include(x => x.SubProjects).ThenInclude(x => x.Child).FirstOrDefault();

            if (project == null)
                throw new ArgumentNullException();


            project.TryUpdateState(out bool stateChanged, projectStatus);

            if (stateChanged)
                _projectRepo.Update(project);
            else
                resultModel.AddError(CommonConstant.ProjectMessages.ProjectChangeState);

            resultModel.Data = project;

            return resultModel;
        }



        private IQueryable<ProjectDto> GetAllDto(Expression<Func<Project, bool>> expression = default)
        {
            var query = _projectRepo.GetAllIncluding();

            if (expression != default)
                query = query.Where(expression);

            return query.Select(x => new ProjectDto
            {
                Code = x.Code,
                FinishDate = x.FinishDate,
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                State = x.State,
                SubProjects = x.SubProjects.Select(y => new ProjectDto
                {
                    Code = y.Child.Code,
                    FinishDate = y.Child.FinishDate,
                    Id = y.ChildId,
                    Name = y.Child.Name,
                    StartDate = y.Child.StartDate,
                    State = y.Child.State,
                }).ToList()
            });
        }

        private ServiceResultModel<Project> ValidateProject(Project model, List<int> newSubProjects)
        {
            var resultModel = new ServiceResultModel<Project>();

            var subProjects = new List<ProjectDto>();

            subProjects = GetAll(x => newSubProjects.Contains(x.Id)).ToList();

            if (model.Id == 0 && subProjects.Count() > 0 && subProjects.All(x => x.State == Enums.ProjectState.Completed))
                resultModel.AddError(CommonConstant.ProjectMessages.ProjectsAllCompleted);

            if (subProjects.Count() != newSubProjects.Count())
                resultModel.AddError(CommonConstant.ProjectMessages.SubProjectsNotFound);

            // validate project code exists
            var existingProject = Get(x => x.Code == model.Code && x.Id != model.Id);

            if (existingProject != null)
                resultModel.AddError(CommonConstant.ProjectMessages.ProjectCodeExists);

            // validate sub project is not a parent to the project
            int verifyChildCannotBeParent = subProjects.Where(x => x.SubProjects.Select(x => x.Id).Contains(model.Id)).Count();

            if (verifyChildCannotBeParent > 0)
                resultModel.AddError(CommonConstant.ProjectMessages.ChildCannotBeParent);

            if (!resultModel.HasError)
            {
                foreach (var project in subProjects)
                {
                    model.SubProjects.Add(new SubProject
                    {
                        ChildId = project.Id
                    });
                }
            }


            return resultModel;

        }

        public IEnumerable<ProjectDto> GetProjectReport(DateTime date)
        {
            return _projectRepo.GetAllIncluding()
                .Where(x => (x.StartDate < date && x.State == ProjectState.InProgress)
                || (x.State == ProjectState.Completed && x.FinishDate > date))
                .Select(x => new ProjectDto
                {
                    Code = x.Code,
                    FinishDate = x.FinishDate,
                    Id = x.Id,
                    Name = x.Name,
                    StartDate = x.StartDate,
                    State = x.State
                }).ToList();
        }

        public void DeleteSubProject(int subProjectId)
        {
            var subProject = _subProjRepo.Get(subProjectId);
            if (subProject == null)
                throw new ArgumentNullException();

            _subProjRepo.Delete(subProject);
        }
    }


}
