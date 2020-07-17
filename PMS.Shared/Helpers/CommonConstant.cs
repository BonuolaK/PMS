using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Shared.Constants
{
    public static class CommonConstant
    {
        public static readonly string GetTaskByProjectApiService = "GetTaskByProjectIdService";

        public static class ProjectMessages
        {

            public static readonly string ProjectCodeExists = "Project with code exists";

            public static readonly string SubProjectsNotFound = "Some sub projects not found";

            public static readonly string ProjectsAllCompleted = "At least one sub-project must be active";

            public static readonly string ProjectChangeState = "State could not be changed. One or more projects are still pending";

            public static readonly string ProjectExistsAsSubProject = "Cannot be deleted. Project is currently a subProject";

            public static readonly string TasksBelongToProject = "Cannot be deleted. Tasks are associated with this project";

            public static readonly string ChildCannotBeParent = "A subproject is a parent of this project and cannot be added as a child";

        }

    }
}
