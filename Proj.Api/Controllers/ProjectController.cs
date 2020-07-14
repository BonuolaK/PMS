using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PMS.Shared.NetCore;
using Proj.Core;
using Proj.Core.Dtos;

namespace Proj.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : BaseController
    {

        private readonly ILogger<ProjectController> _logger;
        private readonly IProjectService _projectService;

        public ProjectController(ILogger<ProjectController> logger,
            IProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectDto>), 200)]
        public IActionResult GetAll()
        {
            var result = _projectService.GetAll();
            if (result.Count() == 0)
                return NoContent();

            return Ok(result);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(ProjectDto), 200)]
        public IActionResult Get([FromRoute] int id)
        {
            var result = _projectService.Get(id);
            if (result == null)
                return NotFound();
            else
                return Ok(result);
        }


        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(200)]
        public IActionResult Delete([FromRoute] int id)
        {
            var project = _projectService.Get(id);

            if (project == null)
                return NotFound();

           var result = _projectService.Delete(project.Id);

            if (result.HasError)
                return HandleBadRequest(result.ErrorMessages);

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult Create([FromBody] ProjectCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ListModelErrors.ToArray());

            var result = _projectService.Create(model);
            if (result.HasError)
                return HandleBadRequest(result.ErrorMessages);

            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(200)]
        public IActionResult Update([FromBody] ProjectCreateDto model)
        {
            if (!ModelState.IsValid)
                return HandleBadRequest(ListModelErrors);

            var project = _projectService.Get(model.Id);

            if (project == null)
                return NotFound();

            var result = _projectService.Update(model);

            if (result.HasError)
                return HandleBadRequest(result.ErrorMessages);

            return Ok();
        }

    }
}