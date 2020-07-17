using PMS.Shared.Service;
using System;
using System.Collections.Generic;
using System.Text;
using TaskSvc.Core.Enums;

namespace TaskSvc.Core.Dtos
{
    public class TaskDto : BaseDto
    {
        public int ProjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public TaskState State { get; set; }

        public List<TaskDto> SubTasks { get; set; }
    }
}
