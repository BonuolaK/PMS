using PMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using TaskSvc.Core.Enums;

namespace TaskSvc.Core.Models
{
    public class PMSTask : BaseEntity
    { 
        public int ProjectId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public TaskState State { get; set; }

        public ICollection<SubTask> SubTasks { get; set; }
    }


}
