using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PMS.Shared.Constants;
using PMS.Shared.DataAccess;
using PMS.Shared.HttpService;
using PMS.Shared.Models;
using Proj.Core;
using Proj.Core.Dtos;
using Proj.Core.Enums;
using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Proj.Tests
{
    public partial class ProjectServiceTests
    {
        private readonly Mock<IRepository<Project>> _projRepositoryMock;
        private readonly Mock<IRepository<SubProject>> _subProjectRepoMock;
        private readonly Mock<IHttpService> _httpService;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<ProjectService>> _mockLogger;

        // System under test
        private readonly IProjectService _projectService;


        private readonly ProjectCreateDto _newProject;
        private readonly ProjectCreateDto _updateProject;

        public ProjectServiceTests()
        {
            _projRepositoryMock = new Mock<IRepository<Project>>();
            _subProjectRepoMock = new Mock<IRepository<SubProject>>();
            _httpService = new Mock<IHttpService>();
            _mockLogger = new Mock<ILogger<ProjectService>>();
            _mockConfig = new Mock<IConfiguration>();


            _projectService = new ProjectService(_projRepositoryMock.Object,
                _subProjectRepoMock.Object,
                _httpService.Object,
                _mockLogger.Object,
                _mockConfig.Object
                );

            _newProject = new ProjectCreateDto
            {
                Id = 0,
                Code = "proj_1",
                SubProjectIds = new List<int> { 1, 2, 3 }
            };

            _updateProject = new ProjectCreateDto
            {
                Id = 1,
                Code = "proj_1",
                SubProjectIds = new List<int> { 3, 6 }
            };



        }

        [Fact]
        public void When_Create_Null_Project_Throw_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => _projectService.Create(null));
        }

        [Fact]
        public void When_Create_Project_And_SubProjects_NotFound_Stop()
        {
            // Arrange  
            _projRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(Enumerable.Empty<Project>().AsQueryable());
            _projRepositoryMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns((Project)null);

            var result = _projectService.Create(_newProject);

            //Assert
            Assert.Contains(CommonConstant.ProjectMessages.SubProjectsNotFound, result.ErrorMessages);
        }


        [Theory]
        [MemberData(nameof(StubGenerator.GetCompletedProjects), MemberType = typeof(StubGenerator))]
        public void When_Create_Project_And_SubProjects_AreAllCompleted_Stop(IEnumerable<Project> completedProjects)
        {
            // Arrange  
            _projRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(completedProjects.AsQueryable());
            _projRepositoryMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns((Project)null);


            var result = _projectService.Create(_newProject);

            //Assert
            Assert.Contains(CommonConstant.ProjectMessages.ProjectsAllCompleted, result.ErrorMessages);

        }

        [Fact]
        public void When_Creating_Project_And_Code_Exists_Stop()
        {
            // Arrange  
            _projRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(Enumerable.Empty<Project>().AsQueryable());
            _projRepositoryMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns(_newProject);


            var result = _projectService.Create(_newProject);

            //Assert
            Assert.Contains(CommonConstant.ProjectMessages.ProjectCodeExists, result.ErrorMessages);
        }


        [Fact]
        public void Ensure_Project_Is_Created()
        {
            // Arrange  
            Project savedProject = null;

            _projRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(_newProject.SubProjectIds
                .Select(x => new Project { Id = x, SubProjects = new List<SubProject>() })
                .AsQueryable());

            _projRepositoryMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns((Project)null);

            _projRepositoryMock.Setup(x => x.Insert(It.IsAny<Project>()))
                 .Callback<Project>((x) =>
                 {
                     x.Id = 1;
                     savedProject = x;
                 });

            // Act  
            var result = _projectService.Create(_newProject);

            // Assert  
            _projRepositoryMock.Verify(x => x.Insert(It.IsAny<Project>()), Times.Once);
            Assert.False(result.HasError);
            Assert.NotNull(result.Data);
        }


        [Fact]
        public void When_Get_Project_Not_Found_Return_Null()
        {
            // Arrange  
            _projRepositoryMock.Setup(x => x.Get(It.IsAny<int>())).Returns<Project>(default);
            // Act  
            var result = _projectService.Get(1);
            // Assert  
            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(StubGenerator.GetProject), MemberType = typeof(StubGenerator))]
        public void When_GetProject_And_Exists_Return_Project(Project project)
        {
            // Arrange  

            _projRepositoryMock.Setup(x => x.Get(project.Id)).Returns(project);
            // Act  
            var result = _projectService.Get(1);
            // Assert  
            Assert.Equal(result, project);

        }

        [Theory]
        [MemberData(nameof(StubGenerator.GetCompletedProjects), MemberType = typeof(StubGenerator))]
        public void When_GetALLProjects_Return_Projects(IEnumerable<Project> projects)
        {
            // Arrange  
            _projRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(projects.AsQueryable());

            // Act  
            var result = _projectService.GetAll();

            // Assert  
            Assert.Equal(result.Count(), projects.Count());

        }

        [Theory]
        [MemberData(nameof(StubGenerator.GetUpdateProjects), MemberType = typeof(StubGenerator))]
        public void When_Update_Project_And_Code_Exists_Stop(List<Project> existingCodeProject)
        {
            // Arrange  
            _projRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(Enumerable.Empty<Project>().AsQueryable());
            _projRepositoryMock.Setup(x => x.GetAllIncluding(x => x.SubProjects)).Returns(existingCodeProject.AsQueryable);

            //_projRepositoryMock.Setup(x => x.Get(_newProject.Id)).Returns(_newProject);
            _projRepositoryMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns(_newProject);

            var result = _projectService.Update(_updateProject);

            //Assert
            Assert.Contains(CommonConstant.ProjectMessages.ProjectCodeExists, result.ErrorMessages);
        }


        [Theory]
        [MemberData(nameof(StubGenerator.GetUpdateProjects), MemberType = typeof(StubGenerator))]
        public void When_Update_Project_New_Sub_Projected_Is_Added(List<Project> existingProject)
        {
            Project updatedProject = null;

            // Arrange  

            _projRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(existingProject.AsQueryable());

            _projRepositoryMock.Setup(x => x.GetAllIncluding(x => x.SubProjects)).Returns(existingProject.AsQueryable);

            _projRepositoryMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>())).Returns((Project)null);


            _projRepositoryMock.Setup(x => x.Update(It.IsAny<Project>()))
                 .Callback<Project>((x) =>
                 {
                     updatedProject = x;
                 });

            var result = _projectService.Update(_updateProject);

            //Assert
            Assert.Contains(6, result.Data.SubProjects.Select(x => x.ChildId));
        }


        [Fact]
        public void When_Update_Project_New_SubProjects_Cannot_Be_A_Parent()
        {

            var parentProjectToUpdate = new Project
            {
                Code = "proj_1",
                State = Core.Enums.ProjectState.Completed,
                Id = 1,
                SubProjects = new List<SubProject>()
            };

            var new_SubProjects_Having_Parent_As_Child = new List<Project>
                 {
                    new Project{
                        Code = "proj_2",
                        State = Core.Enums.ProjectState.Planned,
                        Id = 3,
                        SubProjects = new List<SubProject>(){
                            new SubProject{Id = 2,
                                 Child = new Project{ State = Core.Enums.ProjectState.Completed},
                                ChildId = 1,
                                ParentId = 2 
                            }
                        },
                 },
                      new Project{
                          Code = "proj_6",
                          State = Core.Enums.ProjectState.Planned,
                          Id = 6,
                         SubProjects = new List<SubProject>(){
                            new SubProject{
                                Id = 3,
                                ChildId = 1, ParentId = 2,
                                Child = new Project{ 
                                    State = Core.Enums.ProjectState.Completed
                                },

                            }
                        }
                      }
                };

            // Arrange  
            _projRepositoryMock.Setup(x => x.GetAllIncluding(x => x.SubProjects))
                .Returns(new List<Project> { parentProjectToUpdate }.AsQueryable());

            _projRepositoryMock.Setup(x => x.GetAllIncluding())
                .Returns(new_SubProjects_Having_Parent_As_Child.AsQueryable());

            _projRepositoryMock.Setup(x => x.Get(It.IsAny<Expression<Func<Project, bool>>>()))
                .Returns((Project)null);


            var result = _projectService.Update(_updateProject);

            //Assert
            Assert.Contains(CommonConstant.ProjectMessages.ChildCannotBeParent, result.ErrorMessages);
        }




        [Theory]
        [MemberData(nameof(StubGenerator.GetStateChangeProject), MemberType = typeof(StubGenerator))]
        public void When_SubProjects_Are_Completed_End_Project(IEnumerable<Project> completedSubProjects)
        {
            Project updatedProject = null;
            // Arrange  
            _projRepositoryMock.Setup(x => x.GetAllIncluding())
                .Returns(completedSubProjects.AsQueryable());

            _projRepositoryMock.Setup(x => x.Update(It.IsAny<Project>()))
                 .Callback<Project>((x) =>
                 {
                     updatedProject = x;
                 });

            // Act  
            var result = _projectService.TryUpdateStatus(1);

            //Assert
            _projRepositoryMock.Verify(x => x.Update(It.IsAny<Project>()), Times.Once);
            Assert.Equal(ProjectState.Completed, updatedProject.State);
        }


        [Fact]
        public void When_Removing_SubProject_Ensure_Deletes()
        {

            SubProject mySubProject = new SubProject();
            // Arrange  
            _subProjectRepoMock.Setup(x => x.Get(It.IsAny<int>()))
                .Returns(mySubProject);

            _subProjectRepoMock.Setup(x => x.Delete(It.IsAny<SubProject>()));

            // Act  
            _projectService.DeleteSubProject(1);

            //Assert
            _subProjectRepoMock.Verify(x => x.Delete(It.IsAny<SubProject>()), Times.Once);

        }


        [Fact]
        public void When_Deleting_Project_And_Is_Sub_Project_Return_Error()
        {

            Project myProject = new Project();
            List<SubProject> existingSubProjects = new List<SubProject>() { new SubProject { } };

            // Arrange  
            _projRepositoryMock.Setup(x => x.Get(It.IsAny<int>()))
                .Returns(myProject);

            _subProjectRepoMock.Setup(x => x.GetAllIncluding())
                .Returns(existingSubProjects.AsQueryable);


            _projRepositoryMock.Setup(x => x.Delete(It.IsAny<Project>()));

            // Act  
            var result = _projectService.Delete(1);

            //Assert

            Assert.Contains(CommonConstant.ProjectMessages.ProjectExistsAsSubProject, result.ErrorMessages);
            _projRepositoryMock.Verify(x => x.Delete(It.IsAny<Project>()), Times.Never);
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

        //    Assert.Contains(CommonConstant.ProjectMessages.TasksBelongToProject, result.ErrorMessages);

        //}

        //[Fact]
        //public void Ensure_Project_Can_Delete()
        //{

        //}






    }
}
