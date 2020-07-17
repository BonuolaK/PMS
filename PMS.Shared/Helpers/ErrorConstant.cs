using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Shared.Constants
{
    public static class ErrorConstant
    {
        public static class ProjectMessages
        {

            public static readonly string ProjectCodeExists = "Project with code exists";

            public static readonly string SubProjectsNotFound = "Some sub projects not found";

            public static readonly string ProjectsAllCompleted = "At least one sub-project must be active";

            public static readonly string ProjectChangeState = "State could not be changed. One or more projects are still pending";

        }

    }
}
