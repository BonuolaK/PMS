using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.Shared.NetCore
{
    public class BaseController : ControllerBase
    {
        protected List<string> ListModelErrors
        {
            get
            {
                return ModelState.Values
                  .SelectMany(x => x.Errors
                    .Select(ie => ie.ErrorMessage))
                    .ToList();
            }
        }

        protected IActionResult HandleBadRequest(List<string> errors)
        {
            return BadRequest(new { errors });
        }
    }
}
