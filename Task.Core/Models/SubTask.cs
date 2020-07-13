﻿using PMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskSvc.Core.Models
{
    public class SubTask : BaseEntity
    {
        public Task Parent { get; set; }

        public int ParentId { get; set; }

        public Task Child { get; set; }

        public int ChildId { get; set; }
    }
}
