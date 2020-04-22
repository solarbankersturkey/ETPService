using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        // GET: api/Admin
        [Authorize]
        [HttpGet]
        [Route("index")]
        public IEnumerable<string> Index()
        {
            return new string[] { "-admin", "-service" };
        }

        //POST: Activate the customer
        [Authorize]
        [HttpPost]
        [Route("activatecustomer")]
        public ActionResult<JsonResult> ActivateCustomer()
        {

            //DO ACTIVATE JOB
            
            //
            //

            var successResponse = new
            {
                successCode = 1
            };
            return Ok(successResponse);
        }

        [HttpGet]
        [Route("accessdenied")]
        public IActionResult AccessDenied()
        {
            return Unauthorized("You do not have access to this service!");
        }

    }
}
