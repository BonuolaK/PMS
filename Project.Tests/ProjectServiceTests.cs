using Moq;
using PMS.Shared.Constants;
using PMS.Shared.DataAccess;
using PMS.Shared.Models;
using Proj.Core;
using Proj.Core.Dtos;
using Proj.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Proj.Tests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IRepository<Project>> _projRepositoryMock;
        private readonly IProjectService _projectService;
        private readonly ProjectCreateDto _newProject;
        private readonly List<Project> _fakeExistingProjects;

        public ProjectServiceTests()
        {
            _projRepositoryMock = new Mock<IRepository<Project>>();
            _projectService = new ProjectService(_projRepositoryMock.Object);

            _newProject = new ProjectCreateDto
            {
                Id = 0
            };

            _fakeExistingProjects = new List<Project>{
                new Project
                {
                    Code = "proj_1",
                }
            };
        }

        [Fact]
        public void Ensure_Project_Is_Created()
        {
            // Arrange  
            Project savedProject = null;

            _projRepositoryMock.Setup(x => x.Insert(It.IsAny<Project>()))
                 .Callback<Project>((x) =>
                 {
                     x.Id = 1;
                     savedProject = x;
                 });

            var expectedResult = new ServiceResultModel<Project>
            {
                Data = savedProject
            };

            // Act  
            var result = _projectService.Create(_newProject);


            // Assert  
            _projRepositoryMock.Verify(x => x.Insert(It.IsAny<Project>()), Times.Once);
            Assert.Equal(result, expectedResult);
        }

       
        [Fact]
        public void When_Creating_Project_And_Code_Exists_Block_Creation()
        {
            // Arrange  
            _projRepositoryMock.Setup(x => x.GetAllIncluding()).Returns(_fakeExistingProjects.AsQueryable());
            var expectedResult = new ServiceResultModel<Project>
            {
                Data = null
            };

            expectedResult.AddError(string.Format(ErrorConstant.ProductCodeExists, _newProject.Code));

            // Act  
            var result = _projectService.Create(_newProject);

            // Assert  
            _projRepositoryMock.Verify(x => x.GetAllIncluding(), Times.Once);
            _projRepositoryMock.Verify(x => x.Insert(It.IsAny<Project>()), Times.Never);
            Assert.Equal(result, expectedResult);
        }


        [Fact]
        public void When_Project_Not_Found_Return_Null()
        {

        }

        [Fact]
        public void When_Project_Exists_Return_Project()
        {

        }


        [Fact]
        public void Can_Update_Project()
        {

        }

        [Fact]
        public void Can_Update_Project_Status()
        {

        }

        [Fact]
        public void When_SubProjects_Are_Completed_End_Project()
        {

        }


        [Fact]
        public void When_UpdatingProject_SubProjects_Cannot_Include_ParentProjects()
        {
            
        }

        [Fact]
        public void Can_Delete()
        {

        }

        [Fact]
        public void Ensure_Project_With_Tasks_Cannot_Delete()
        {

        }

    }
}
