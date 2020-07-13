using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Shared.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }
    }
}
