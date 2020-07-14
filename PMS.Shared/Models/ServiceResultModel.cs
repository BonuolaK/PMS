using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Shared.Models
{
    public class ServiceResultModel<T>
    {
        public List<string> ErrorMessages { get; set; }

        public T Data { get; set; }

        public bool HasError => ErrorMessages.Count > 0;
    }
}
