using PMS.Shared.Models;
using PMS.Shared.Service;
using System;
using System.Collections.Generic;
using System.Text;
using TaskSvc.Core.Dtos;
using TaskSvc.Core.Enums;
using TaskSvc.Core.Models;

namespace TaskSvc.Core
{
    public interface ITaskService : IBaseService<TaskCreateDto, PMSTask, TaskDto>
    {
        void DeleteSubTask(int subTaskId);

        ServiceResultModel<PMSTask> UpdateState(int taskId, TaskState state);


        IEnumerable<TaskDto> GetTaskReport(DateTime date);
    }
}
