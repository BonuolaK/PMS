using Microsoft.EntityFrameworkCore;
using PMS.Shared.Constants;
using PMS.Shared.DataAccess;
using PMS.Shared.Models;
using Proj.Core.Dtos;
using Proj.Core.Enums;
using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
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
                resultModel.AddError(ErrorConstant.ProjectMessages.ProjectChangeState);

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
                    Id = y.Child.Id,
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

            if (subProjects.Count() != newSubProjects.Count())
                resultModel.AddError(ErrorConstant.ProjectMessages.SubProjectsNotFound);
            else
            {
                foreach (var project in subProjects)
                {
                    model.SubProjects.Add(new SubProject
                    {
                        ChildId = project.Id
                    });
                }
            }

            if (model.Id ==0 &&  subProjects.Count() > 0 && subProjects.All(x => x.State == Enums.ProjectState.Completed))
                resultModel.AddError(ErrorConstant.ProjectMessages.ProjectsAllCompleted);

            // validate project code exists
            var existingProject = Get(x => x.Code == model.Code && x.Id != model.Id);

            if (existingProject != null)
                resultModel.AddError(ErrorConstant.ProjectMessages.ProjectCodeExists);

            return resultModel;

        }


    }


}
