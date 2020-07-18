using MassTransit;
using Microsoft.EntityFrameworkCore;
using PMS.Shared.Constants;
using PMS.Shared.DataAccess;
using PMS.Shared.Models;
using PMS.Shared.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskSvc.Core.Dtos;
using TaskSvc.Core.Models;

namespace TaskSvc.Core
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<PMSTask> _taskRepo;
        private readonly IRepository<SubTask> _subTaskRepo;
        private readonly IPublishEndpoint _publisher;

        public TaskService(IRepository<PMSTask> taskRepo,
            IRepository<SubTask> subTaskRepo,
            IPublishEndpoint publishEndpoint)
        {
            _taskRepo = taskRepo;
            _subTaskRepo = subTaskRepo;
            _publisher = publishEndpoint;
        }


        public ServiceResultModel<PMSTask> Create(TaskCreateDto model)
        {
            if (model == null)
                throw new ArgumentNullException();

            var task = (PMSTask)model;

            var resultModel = ValidateTask(task, model.SubTaskIds);

            if (resultModel.HasError)
                return resultModel;

            _taskRepo.Insert(task);
            resultModel.Data = task;

            return resultModel;
        }

        public ServiceResultModel<int> Delete(int id)
        {
            var result = new ServiceResultModel<int>();
            var task = Get(id);

            if (task == null)
                throw new ArgumentNullException();

            // find subTAsk

            var subTaskCount = _subTaskRepo.GetAllIncluding().Where(x => x.ChildId == task.Id).Count();

            if (subTaskCount > 0)
            {
                result.AddError(CommonConstant.TaskMessages.ProjectExistsAsSubProject);
                return result;
            }


            _taskRepo.Delete(task);
            return result;
        }


        public PMSTask Get(int id)
        {
            return _taskRepo.Get(id);
        }


        public PMSTask Get(Expression<Func<PMSTask, bool>> expression)
        {
            return _taskRepo.Get(expression);
        }

        public IEnumerable<TaskDto> GetAll()
        {
            return GetAllDto().ToList();
        }

        public IEnumerable<TaskDto> GetAll(Expression<Func<PMSTask , bool>> expression)
        {
            return GetAllDto(expression).ToList();
        }


        public ServiceResultModel<PMSTask> Update(TaskCreateDto model)
        {
            if (model == null)
                throw new ArgumentNullException();

            var task = _taskRepo.GetAllIncluding(x => x.SubTasks).Where(x => x.Id == model.Id)
                .Include(x => x.SubTasks).ThenInclude(x => x.Child).FirstOrDefault();

            if (task == null)
                throw new ArgumentNullException();


            task.Name = model.Name;
            task.StartDate = model.StartDate;

            var addedSubTAsks = model.SubTaskIds.Except(task.SubTasks.Select(x => x.ChildId)).ToList();

            var resultModel = ValidateTask(task, addedSubTAsks);

            if (resultModel.HasError)
                return resultModel;

            _taskRepo.Update(task);

            if (task.State == Enums.TaskState.Completed)
                ConfirmTasksAreCompleted(task, new CancellationToken());

            resultModel.Data = task;

            return resultModel;
        }

        public async Task ConfirmTasksAreCompleted(PMSTask task, CancellationToken cancellationToken)
        {
            var unCompletedProjectTasks = _taskRepo.GetAllIncluding().Where(x =>
            x.ProjectId == task.ProjectId
            && x.State != Enums.TaskState.Completed
            && x.Id != task.Id
            ).Count();

            if(unCompletedProjectTasks == 0)
            {

                await _publisher.Publish<ITaskCompletedMessage>(new TaskCompletedMessage()
                {
                    MessageId = new Guid(),
                    ProjectId = task.ProjectId,

                }, cancellationToken);
            }
        }

        private IQueryable<TaskDto> GetAllDto(Expression<Func<PMSTask, bool>> expression = default)
        {
            var query = _taskRepo.GetAllIncluding();

            if (expression != default)
                query = query.Where(expression);

            return query.Select(x => new TaskDto
            {
                FinishDate = x.FinishDate,
                Id = x.Id,
                ProjectId = x.ProjectId,
                Name = x.Name,
                StartDate = x.StartDate,
                State = x.State,
                SubTasks = x.SubTasks.Select(y => new TaskDto
                {
                    FinishDate = y.Child.FinishDate,
                    Id = y.ChildId,
                    Name = y.Child.Name,
                    StartDate = y.Child.StartDate,
                    State = y.Child.State,
                    ProjectId = y.Child.ProjectId
                }).ToList()
            });
        }

        private ServiceResultModel<PMSTask> ValidateTask(PMSTask model, List<int> newSubTasks)
        {
            var resultModel = new ServiceResultModel<PMSTask>();

            var subTasks = new List<TaskDto>();

            subTasks = GetAll(x => newSubTasks.Contains(x.Id)).ToList();

            
            if (subTasks.Count() != newSubTasks.Count())
                resultModel.AddError(CommonConstant.TaskMessages.SubProjectsNotFound);

            
            // validate sub task is not a parent to the task
            int verifyChildCannotBeParent = subTasks.Where(x => x.SubTasks.Select(x => x.Id).Contains(model.Id)).Count();

            if (verifyChildCannotBeParent > 0)
                resultModel.AddError(CommonConstant.TaskMessages.ChildCannotBeParent);

            if (!resultModel.HasError)
            {
                foreach (var task in subTasks)
                {
                    model.SubTasks.Add(new SubTask
                    {
                        ChildId = task.Id
                    });
                }
            }

            return resultModel;

        }

        public void DeleteSubTask(int subTaskId)
        {
            var subTask = _subTaskRepo.Get(subTaskId);
            if (subTask == null)
                throw new ArgumentNullException();

            _subTaskRepo.Delete(subTask);
        }


        // task microservice can contain project names and be updated when it changes via message bus
        public IEnumerable<TaskDto> GetTaskReport(DateTime date)
        {
            return _taskRepo.GetAllIncluding()
             .Where(x => (x.StartDate < date && x.State == Enums.TaskState.InProgress)
             || (x.State == Enums.TaskState.Completed && x.FinishDate > date))
             .Select(x => new TaskDto
             {
                 FinishDate = x.FinishDate,
                 Id = x.Id,
                 Name = x.Name,
                 StartDate = x.StartDate,
                 State = x.State,
                 ProjectId = x.ProjectId
             }).ToList();

        }
    }
}
