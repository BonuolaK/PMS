using Proj.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Proj.Core.Models
{
    public static class ProjectExtensions
    {
        public static void TryUpdateState(this Project project, out bool stateChanged, ProjectState state = default)
        {
            stateChanged = false;
            ProjectState initialState = project.State;

            if (project.SubProjects != default && project.SubProjects.Count >= 0)
            {
                if (project.SubProjects.Count == 0 || 
                    project.SubProjects.All(x => x.Child.State == ProjectState.Completed))
                    project.State = ProjectState.Completed;

                else if (project.SubProjects.Any(x => x.Child.State == ProjectState.InProgress))
                    project.State = ProjectState.InProgress;
                else
                    project.State = ProjectState.Planned;
            }
            else
            {
                project.State = state;
            }

            if (initialState != project.State)
                stateChanged = true;
        }
    }
}
