using PMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proj.Core.Models
{
    public class SubProject : BaseEntity<int>
    {
        public Project Parent { get; set; }

        public int ParentId { get; set; }

        public Project Child { get; set; }

        public int ChildId { get; set; }
    }
}
