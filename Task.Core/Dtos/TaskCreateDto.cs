using PMS.Shared.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
    }
}
