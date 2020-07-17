using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proj.Tests
{
    public static class StubGenerator
    {
        public static IEnumerable<object[]> GetCompletedProjects()
        {
            yield return new object[] {
                new List<Project>{new Project{
                    Id = 1,
                    State = Core.Enums.ProjectState.Completed,
                    SubProjects = new List<SubProject>()
                },
                new Project{
                     Id = 2,
                    State = Core.Enums.ProjectState.Completed,
                    SubProjects = new List<SubProject>()
                },
                new Project{
                     Id = 3,
                    State = Core.Enums.ProjectState.Completed,
                    SubProjects = new List<SubProject>()
                }
             }
            };
        }


        public static IEnumerable<object[]> GetProject()
        {
            yield return new object[] {
                new Project{Code = "proj_1",
                    State = Core.Enums.ProjectState.Completed,
                    Id = 1,
                    SubProjects = new List<SubProject>()
                    {
                        new SubProject{Id = 1, ChildId = 2, ParentId = 1},
                        new SubProject{Id = 1, ChildId = 3, ParentId = 1},
                        new SubProject{Id = 1, ChildId = 4, ParentId = 1}
                    }
                }
            };
        }


        public static IEnumerable<object[]> GetUpdateProjects()
        {
            yield return new object[] {
                 new List<Project>
                 {
                     new Project{Code = "proj_1",
                    State = Core.Enums.ProjectState.Completed,
                    Id = 1,
                    SubProjects = new List<SubProject>()
                    {
                        new SubProject{Id = 1, ChildId = 3, ParentId = 1},
                        new SubProject{Id = 2, ChildId = 4, ParentId = 1},
                    }
                  },

                     new Project{Code = "proj_1",
                    State = Core.Enums.ProjectState.Planned,
                    Id = 3,
                     SubProjects = new List<SubProject>()
                },
                         new Project{Code = "proj_1",
                    State = Core.Enums.ProjectState.Planned,
                    Id = 6,
                     SubProjects = new List<SubProject>()
                }
                 }
            };
        }

        
        public static IEnumerable<object[]> GetStateChangeProject()
        {
            yield return new object[] {
                new List<Project>{
                 new Project{Code = "proj_1",
                    State = Core.Enums.ProjectState.InProgress,
                    Id = 1,
                    SubProjects = new List<SubProject>()
                    {
                        new SubProject{Id = 1,
                            ChildId = 4,
                            Child = new Project{ State = Core.Enums.ProjectState.Completed},
                            ParentId = 1
                        },
                        new SubProject{Id = 2,
                            ChildId = 3,
                            Child = new Project{ State = Core.Enums.ProjectState.Completed},
                            ParentId = 1
                        }
                    }
                }
             }
            };
        }


    }

}
