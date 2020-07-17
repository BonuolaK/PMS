using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Shared.ExcelGenerator
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DataExport : Attribute
    {
        public string SheetName { get; set; }

        public bool AddIndex { get; set; } = false;
    }
}
