using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PMS.Shared.ExcelGenerator;
using PMS.Shared.Helpers;
using PMS.Shared.NetCore;
using TaskSvc.Core;
using TaskSvc.Core.Dtos;
using TaskSvc.Core.Enums;

namespace Proj.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : BaseController
    {
        private readonly ILogger<TaskController> _logger;
        private readonly ITaskService _taskService;

        public TaskController(ILogger<TaskController> logger,
            ITaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), 200)]
        public IActionResult GetAll()
        {
            var result = _taskService.GetAll();
            if (result.Count() == 0)
                return NoContent();

            return Ok(result);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(TaskDto), 200)]
        public IActionResult Get([FromRoute] int id)
        {
            var result = _taskService.Get(id);
            if (result == null)
                return NotFound();
            else
                return Ok(result);
        }

        // add check-exists

        [Route("check-exists")]
        [HttpGet]
        [ProducesResponseType(typeof(TaskDto), 200)]
        public IActionResult GetTaskByProjectId([FromQuery] int projectId)
        {
            var result = _taskService.GetAll(x=> x.ProjectId == projectId);
            if (result == null)
                return NotFound();
            else
                return Ok(result.Count() > 0);
        }


        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(200)]
        public IActionResult Delete([FromRoute] int id)
        {
            var task = _taskService.Get(id);

            if (task == null)
                return NotFound();

           var result = _taskService.Delete(task.Id);

            if (result.HasError)
                return HandleBadRequest(result.ErrorMessages);

            return Ok();
        }


        [Route("subproject/{id}")]
        [HttpDelete]
        [ProducesResponseType(200)]
        public IActionResult DeleteSubTask([FromRoute] int id)
        {
            try
            {
                _taskService.DeleteSubTask(id);

            }
            catch (Exception ex)
            {
                return HandleBadRequest(new List<string> { "Invalid operation" });
            }


            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult Create([FromBody] TaskCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ListModelErrors.ToArray());

            var result = _taskService.Create(model);
            if (result.HasError)
                return HandleBadRequest(result.ErrorMessages);

            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(200)]
        public IActionResult Update([FromBody] TaskCreateDto model)
        {
            if (!ModelState.IsValid)
                return HandleBadRequest(ListModelErrors);

            var project = _taskService.Get(model.Id);

            if (project == null)
                return NotFound();

            var result = _taskService.Update(model);

            if (result.HasError)
                return HandleBadRequest(result.ErrorMessages);

            return Ok();
        }


        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(200)]
        public IActionResult UpdateStatus([FromRoute] int id, [FromBody]TaskState status)
        {
            if (!ModelState.IsValid)
                return HandleBadRequest(ListModelErrors);

            var project = _taskService.Get(id);

            if (project == null)
                return NotFound();

            var result = _taskService.UpdateState(id, status);

            if (result.HasError)
                return HandleBadRequest(result.ErrorMessages);

            return Ok();
        }



        [Route("report")]
        [HttpGet]
        public HttpResponseMessage GetTasksInProgressBeforeDate([FromQuery] string date)
        {

            var dateFormat = date.ToInvariantDateTime("dd/MM/yyyy", out bool succeded);

            if (!succeded)
                HandleBadRequest(new List<string> { "Invalid date format supplied: Expecting dd/MM/yyyy" });

            var projectInProgress = _taskService.GetTaskReport(dateFormat);

            var file = ExcelGenerator.ToExcel(projectInProgress.ToList());

            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(file) };

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = $"Task_Report_{date}" };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            return response;
        }
    }
}