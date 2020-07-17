using PMS.Shared.Service;
using System;
using System.Collections.Generic;
using System.Text;
using TaskSvc.Core.Dtos;
using TaskSvc.Core.Models;

namespace TaskSvc.Core
{
    public interface ITaskService : IBaseService<TaskCreateDto, PMSTask, TaskDto>
    {
        void DeleteSubTask(int subTaskId);


        IEnumerable<TaskDto> GetTaskReport(DateTime date);
    }
}
