using PMS.Shared.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    }
}
