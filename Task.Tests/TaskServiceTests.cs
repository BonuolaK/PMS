using MassTransit;
using Moq;
using PMS.Shared.Constants;
using PMS.Shared.DataAccess;
using PMS.Shared.HttpService;
using PMS.Shared.Models;
using PMS.Shared.PubSub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskSvc.Core;
using TaskSvc.Core.Dtos;
using TaskSvc.Core.Models;
using Xunit;

namespace TaskSvc.Tests
{
    public partial class TaskServiceTests
    {
        private readonly Mock<IRepository<PMSTask>> _taskRepositoryMock;
        private readonly Mock<IRepository<SubTask>> _subTaskRepoMock;
        private readonly Mock<IHttpService> _httpService;
        private readonly Mock<IPublishEndpoint> _publisher;

        // System under test
        private readonly ITaskService _taskService;
        private readonly TaskCreateDto _newTask;
        private readonly TaskCreateDto _updateTask;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<IRepository<PMSTask>>();
            _subTaskRepoMock = new Mock<IRepository<SubTask>>();
            _httpService = new Mock<IHttpService>();
            _publisher = new Mock<IPublishEndpoint>();

            _taskService = new TaskService(_taskRepositoryMock.Object,
                _subTaskRepoMock.Object, _publisher.Object);

            _newTask = new TaskCreateDto
            {
                Id = 0,
                SubTaskIds = new List<int> { 1, 2, 3 }
            };

            _updateTask = new TaskCreateDto
            {
                Id = 1,
                SubTaskIds = new List<int> { 3, 6 }
            };
        }

        [Fact]
        public void When_Create_Null_Task_Throw_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => _taskService.Create(null));
        }

        [Fact]
        public void When_Create_Task_And_SubTask_NotFound_Stop()
        {
            // Arrange  
            _taskRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(Enumerable.Empty<PMSTask>().AsQueryable());

            var result = _taskService.Create(_newTask);

            //Assert
            Assert.Contains(CommonConstant.TaskMessages.SubProjectsNotFound, result.ErrorMessages);
        }


        [Theory]
        [MemberData(nameof(StubGenerator.GetTaskDatabase), MemberType = typeof(StubGenerator))]
        public void Ensure_Task_Is_Created(List<PMSTask> existingTasks)
        {
            // Arrange  
            PMSTask savedTask = null;

            _taskRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(existingTasks.AsQueryable());

            _taskRepositoryMock.Setup(x => x.Insert(It.IsAny<PMSTask>()))
                 .Callback<PMSTask>((x) =>
                 {
                     x.Id = 1;
                     savedTask = x;
                 });

            // Act  
            var result = _taskService.Create(_newTask);

            // Assert  
            _taskRepositoryMock.Verify(x => x.Insert(It.IsAny<PMSTask>()), Times.Once);
            Assert.False(result.HasError);
            Assert.NotNull(result.Data);
        }


        [Fact]
        public void When_Get_Task_Not_Found_Return_Null()
        {
            // Arrange  
            _taskRepositoryMock.Setup(x => x.Get(It.IsAny<int>())).Returns<PMSTask>(default);
            // Act  
            var result = _taskService.Get(1);
            // Assert  
            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(StubGenerator.GetTaskDatabase), MemberType = typeof(StubGenerator))]
        public void When_GetTaskAnd_Exists_Return_Task(List<PMSTask> tasks)
        {
            // Arrange  
            var task = tasks.Where(x => x.Id == 1).FirstOrDefault();

            _taskRepositoryMock.Setup(x => x.Get(1)).Returns(task);
            // Act  
            var result = _taskService.Get(1);
            // Assert  
            Assert.Equal(result, task);

        }

        [Theory]
        [MemberData(nameof(StubGenerator.GetTaskDatabase), MemberType = typeof(StubGenerator))]
        public void When_GetALLTasks_Return_Tasks(IEnumerable<PMSTask> projects)
        {
            // Arrange  
            _taskRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(projects.AsQueryable());

            // Act  
            var result = _taskService.GetAll();

            // Assert  
            Assert.Equal(result.Count(), projects.Count());
        }


        [Theory]
        [MemberData(nameof(StubGenerator.GetTaskDatabase), MemberType = typeof(StubGenerator))]
        public void When_Update_Task_New_Sub_Task_Is_Added(List<PMSTask> existingProject)
        {
            PMSTask updatedTask = null;

            // Arrange  

            _taskRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(existingProject.AsQueryable());

            _taskRepositoryMock.Setup(x => x.GetAllIncluding(x => x.SubTasks)).Returns(existingProject.AsQueryable);


            _taskRepositoryMock.Setup(x => x.Update(It.IsAny<PMSTask>()))
                 .Callback<PMSTask>((x) =>
                 {
                     updatedTask = x;
                 });

            var result = _taskService.Update(_updateTask);

            //Assert
            Assert.Contains(6, result.Data.SubTasks.Select(x => x.ChildId));
        }


        [Fact]
        public void When_Update_Task_New_SubTasks_Cannot_Be_A_Parent()
        {

            var parentProjectToUpdate = new PMSTask
            {
                State = Core.Enums.TaskState.Planned,
                Id = 1,
                SubTasks = new List<SubTask>()
            };

            var new_SubTask_Having_Parent_As_Child = new List<PMSTask>
                 {
                    new PMSTask{
                        State = Core.Enums.TaskState.Planned,
                        Id = 3,
                        SubTasks = new List<SubTask>(){
                            new SubTask{Id = 2,
                                 Child = new PMSTask{ State = Core.Enums.TaskState.Completed},
                                ChildId = 1,
                                ParentId = 2
                            }
                        },
                 },
                      new PMSTask{
                          State = Core.Enums.TaskState.Planned,
                          Id = 6,
                         SubTasks = new List<SubTask>(){
                            new SubTask{
                                Id = 3,
                                ChildId = 1, ParentId = 2,
                                Child = new PMSTask{
                                    State = Core.Enums.TaskState.Completed
                                },

                            }
                        }
                      }
                };

            // Arrange  
            _taskRepositoryMock.Setup(x => x.GetAllIncluding(x => x.SubTasks))
                .Returns(new List<PMSTask> { parentProjectToUpdate }.AsQueryable());

            _taskRepositoryMock.Setup(x => x.GetAllIncluding())
                .Returns(new_SubTask_Having_Parent_As_Child.AsQueryable());


            var result = _taskService.Update(_updateTask);

            //Assert
            Assert.Contains(CommonConstant.TaskMessages.ChildCannotBeParent, result.ErrorMessages);
        }


        [Theory]
        [MemberData(nameof(StubGenerator.GetTaskDatabase), MemberType = typeof(StubGenerator))]
        public void Test_Report_Returns_Filtered_Date(List<PMSTask> existingProject)
        {
            PMSTask updatedTask = null;

            // Arrange  

            _taskRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(existingProject.AsQueryable());

            _taskRepositoryMock.Setup(x => x.Update(It.IsAny<PMSTask>()))
                 .Callback<PMSTask>((x) =>
                 {
                     updatedTask = x;
                 });

            var result = _taskService.GetTaskReport(new DateTime(2020, 6, 1));

            //Assert
            Assert.Equal(3, result.Count());
        }


        [Fact]
        public void Ensure_Update_Task_State()
        {

            var parentProjectToUpdate = new PMSTask
            {
                State = Core.Enums.TaskState.Planned,
                Id = 1,
                SubTasks = new List<SubTask>()
            };

            // Arrange  
            _taskRepositoryMock.Setup(x => x.Get(It.IsAny<int>()))
                .Returns(parentProjectToUpdate);


            _taskRepositoryMock.Setup(x => x.Update(It.IsAny<PMSTask>()))
                 .Callback<PMSTask>((x) =>
                 {
                     parentProjectToUpdate = x;
                 });

            //TODO: mock publisher to mass transit in-memory queue implementation - publish in service commented out during this test

            var result = _taskService.UpdateState(1,Core.Enums.TaskState.Completed);

            //Assert
            Assert.Equal(parentProjectToUpdate.State, result.Data.State);
        }



        //[Fact]
        //public void When_All_Tasks_Completed_Should_Publish_To_Receivers()
        //{

        //    var parentProjectToUpdate = new PMSTask
        //    {
        //        State = Core.Enums.TaskState.Completed,
        //        Id = 1,
        //        SubTasks = new List<SubTask>()
        //    };

        //    // Arrange  
        //    _taskRepositoryMock.Setup(x => x.GetAllIncluding(x => x.SubTasks))
        //        .Returns(new List<PMSTask> { parentProjectToUpdate }.AsQueryable());


        //    _taskRepositoryMock.Setup(x => x.GetAllIncluding())
        //        .Returns(Enumerable.Empty<PMSTask>().AsQueryable());

        //    _taskRepositoryMock.Setup(x => x.GetAllIncluding())
        //        .Returns(Enumerable.Empty<PMSTask>().AsQueryable());


        //    var result = _taskService.Update(_updateTask);

        //    //Assert

        //    _publisher.Verify(x => x.Publish(new TaskCompletedMessage()), Times.Once);
        //}




        [Fact]
        public void When_Removing_SubTask_Ensure_Deletes()
        {

            SubTask subTask = new SubTask();

            // Arrange  
            _subTaskRepoMock.Setup(x => x.Get(It.IsAny<int>()))
                .Returns(subTask);

            _subTaskRepoMock.Setup(x => x.Delete(It.IsAny<SubTask>()));

            // Act  
            _taskService.DeleteSubTask(1);

            //Assert
            _subTaskRepoMock.Verify(x => x.Delete(It.IsAny<SubTask>()), Times.Once);

        }


        [Fact]
        public void When_Deleting_Task_And_Is_Sub_Task_Return_Error()
        {

            PMSTask project = new PMSTask();
            List<SubTask> existingSubTasks = new List<SubTask>() { new SubTask { } };

            // Arrange  
            _taskRepositoryMock.Setup(x => x.Get(It.IsAny<int>()))
                .Returns(project);

            _subTaskRepoMock.Setup(x => x.GetAllIncluding())
                .Returns(existingSubTasks.AsQueryable);


            _taskRepositoryMock.Setup(x => x.Delete(It.IsAny<PMSTask>()));

            // Act  
            var result = _taskService.Delete(1);

            //Assert

            Assert.Contains(CommonConstant.TaskMessages.ProjectExistsAsSubProject, result.ErrorMessages);
            _taskRepositoryMock.Verify(x => x.Delete(It.IsAny<PMSTask>()), Times.Never);
        }



        //[Fact]
        //public void Ensure_Project_With_Tasks_Cannot_Delete()
        //{
        //    Project myProject = new Project();

        //    _projRepositoryMock.Setup(x => x.Get(It.IsAny<int>()))
        //      .Returns(myProject);

        //    _subProjectRepoMock.Setup(x => x.GetAllIncluding())
        //        .Returns(Enumerable.Empty<SubProject>().AsQueryable);

        //    var configSection = new Mock<IConfigurationSection>();

        //    configSection.Setup(x => x.Value).Returns("{0}");
        //    _mockConfig.Setup(x => x.GetSection(It.IsAny<String>())).Returns(configSection.Object);


        //    _httpService.Setup(x => x.GetAsync<bool>(It.IsAny<string>())).Returns(new Task<bool>(() => true));


        //    // Act  
        //    var result = _projectService.Delete(1);

        //    Assert.Contains(CommonConstant.TaskMessages.TasksBelongToProject, result.ErrorMessages);

        //}

        //[Fact]
        //public void Ensure_Project_Can_Delete()
        //{

        //}






    }
}
