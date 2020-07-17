using PMS.Shared.Helpers;
using PMS.Shared.Service;
using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Proj.Core.Dtos
{
    public class ProjectCreateDto : BaseDto
    {
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }


        public List<int> SubProjectIds { get; set; }


        public static implicit operator Project(ProjectCreateDto dto)
        {
            return new Project
            {
                Code = dto.Code,
                DateCreated = Helper.GetCurrentDate(),
                Name = dto.Name,
                StartDate = dto.StartDate,
                State = Enums.ProjectState.InProgress,
                SubProjects = dto.SubProjectIds.Select(x => new SubProject {
                    ChildId = x
                }).ToList()
            };
        }

    }

}
