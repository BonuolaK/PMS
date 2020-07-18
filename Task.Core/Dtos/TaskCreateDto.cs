using PMS.Shared.Helpers;
using PMS.Shared.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using TaskSvc.Core.Enums;
using TaskSvc.Core.Models;

namespace TaskSvc.Core.Dtos
{
   public class TaskCreateDto : BaseDto
    {
        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public List<int> SubTaskIds { get; set; }


        public static implicit operator PMSTask(TaskCreateDto dto)
        {
            return new PMSTask
            {
                DateCreated = Helper.GetCurrentDate(),
                Name = dto.Name,
                ProjectId = dto.ProjectId,
                StartDate = dto.StartDate,
                State = Enums.TaskState.InProgress,
                SubTasks = dto.SubTaskIds.Select(x => new SubTask
                {
                    ChildId = x
                }).ToList()
            };
        }
    }

    public class TaskUpdateDto : TaskCreateDto
    {
        public TaskState TaskState { get; set; }

    }

}
