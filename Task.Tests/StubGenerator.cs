using System;
using System.Collections.Generic;
using System.Text;
using TaskSvc.Core.Enums;
using TaskSvc.Core.Models;

namespace TaskSvc.Tests
{
    public static class StubGenerator
    {
        public static IEnumerable<object[]> GetTaskDatabase()
        {
            yield return new object[] {
                new List<PMSTask>{new PMSTask{
                    Id = 1,
                    State = TaskState.Completed,
                    SubTasks = new List<SubTask>()
                    {
                        new SubTask { 
                            ChildId = 2,
                            ParentId = 1,
                            Child = new PMSTask
                            {
                                State = TaskState.InProgress
                            }
                        }
                    }
                },
                new PMSTask{
                    Id = 2,
                    State = TaskState.Completed,
                    SubTasks = new List<SubTask>()
                },
                new PMSTask{
                    Id = 3,
                    State = TaskState.Completed,
                    SubTasks = new List<SubTask>()
                },
                new PMSTask{
                    Id = 4,
                    State = TaskState.Completed,
                    SubTasks = new List<SubTask>()
                },
                new PMSTask{
                    Id = 5,
                    State = TaskState.Completed,
                    SubTasks = new List<SubTask>()
                },
                new PMSTask{
                    Id = 6,
                    State = TaskState.Completed,
                    SubTasks = new List<SubTask>()
                }
             }
            };
        }




    }

}
