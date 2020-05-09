using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _customerRepository;
        private IEmailSender _emailSender;

        public AdminController(IUserRepository customerRepository, IEmailSender emailSender)
        {
            _customerRepository = customerRepository;
            _emailSender = emailSender;
        }

        // GET: api/Admin
        [Authorize]
        [HttpGet]
        [Route("index")]
        public IEnumerable<string> Index()
        {
            return new string[] { "-admin", "-service" };
        }

        // GET: api/Admin
        [Authorize]
        [HttpGet]
        [Route("getallcustomers")]
        public async Task<JsonResult> GetAllCustomersAsync()
        {
            return new JsonResult(await _customerRepository.Get());
        }

        //POST: Activate the customer
        [Authorize]
        [HttpPost]
        [Route("activatecustomer")]
        public async Task<ActionResult> ActivateCustomerAsync(string id)
        {

            //DO ACTIVATE JOB
            var user = await _customerRepository.Get(id);
            user.Status = "Active";
            _customerRepository.Update(user);
            //

            var successResponse = new
            {
                successCode = 1,
                message = "User activated."
            };
            return Ok(successResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("deactivatecustomer")]
        public async Task<ActionResult> DeactivateCustomerAsync(string id)
        {

            //DO DEACTIVATE JOB
            var user = await _customerRepository.Get(id);
            user.Status = "Not Active";
            _customerRepository.Update(user);
            //
            //

            var successResponse = new
            {
                successCode = 1,
                message = "User deactivated."
            };
            return Ok(successResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("updateuser")]
        public JsonResult UpdateUser(User usr)
        {

            //UpdateUser
            _customerRepository.Update(usr);
            //

            var successResponse = new
            {
                successCode = 1,
                message = "User updated."
            };
            return new JsonResult(successResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("getcustomer")]
        public async Task<ActionResult> GetCustomerAsync(string id)
        {

            //GET CUSTOMER
            var user = await _customerRepository.Get(id);
            //

            var successResponse = new
            {
                successCode = 1,
                message = user
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
