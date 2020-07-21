using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerService.Contexts;
using CustomerService.Dto;
using CustomerService.Model;
using CustomerService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
namespace CustomerService.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase {
        private readonly IUserRepository _customerRepository;
        public Crypto encoder { get; set; }
        public string DefaultLanguage { get; set; }
        public CustomerController (IUserRepository customerRepository) {
            _customerRepository = customerRepository;
            encoder = new Crypto ();
            DefaultLanguage = "TR";
        }

        private static readonly string[] Customers = new [] {
            "Mert",
            "Barış"
        };

        [Authorize]
        [HttpGet]
        [Route ("index")]
        public IActionResult Index () {
            return Ok ("Welcome to the customer microservice!");
        }

        [HttpGet]
        [Authorize]
        [Route ("getallcustomers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllCustomers () {
            var customers = await _customerRepository.Get ();
            return Ok (customers);
        }

        [Authorize]
        [HttpGet]
        [Route ("getcustomerid")]
        public IActionResult GetCustomerId () {
            var userId = encoder.Decrypt (HttpContext.User.FindFirst ("etp_user").Value);
            return Ok (userId.ToString ());
        }

        [Authorize]
        [HttpPost]
        [Route ("getcustomer")]
        public async Task<ActionResult<IEnumerable<User>>> GetCustomer (string id) {
            var customer = await _customerRepository.Get (encoder.Decrypt (HttpContext.User.FindFirst ("etp_user").Value));
            return Ok (customer);
        }

        [Authorize]
        [HttpPost]
        [Route ("login")]
        public async Task<ActionResult<IEnumerable<User>>> Login (string username, string password) {
            try { //ONLINE STATUS WILL BE IMPLEMENTED **
                var securepassword = encoder.HashHMAC (Encoding.ASCII.GetBytes ("xUhs67g"), Encoding.ASCII.GetBytes (password));
                var login_result = await _customerRepository.Get (Builders<User>.Filter.And (Builders<User>.Filter.Eq ("Username", username), Builders<User>.Filter.Eq ("Password", securepassword)));
                var successResponse = new {
                    successCode = 1,
                    userInfo = login_result.FirstOrDefault ()
                };
                return Ok (successResponse);
            } catch (Exception e) {
                var errorJson = new {
                    error = e.ToString (),
                    error_message = e.Message
                };

                return new JsonResult (errorJson);
            }
        }

        [Authorize]
        [HttpPost]
        [Route ("logout")] //DÜZENLENECEK JWT KİLL AND STATUS FALSE
        public async Task<ActionResult<IEnumerable<User>>> LogOut (string username, string password) {
            try {
                var securepassword = encoder.HashHMAC (Encoding.ASCII.GetBytes ("xUhs67g"), Encoding.ASCII.GetBytes (password));
                var login_result = await _customerRepository.Get (Builders<User>.Filter.And (Builders<User>.Filter.Eq ("Username", username), Builders<User>.Filter.Eq ("Password", securepassword)));
                var successResponse = new {
                    successCode = 1,
                    userInfo = login_result.FirstOrDefault ()
                };
                return Ok (successResponse);
            } catch (Exception e) {
                var errorJson = new {
                    error = e.ToString (),
                    error_message = e.Message
                };

                return new JsonResult (errorJson);
            }
        }

        [HttpPost]
        [Route ("register")]
        public IActionResult Register (RegisterModel rm) {
            try {
                var username = "c_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds ().ToString () + DateTimeOffset.UtcNow.Millisecond.ToString ();
                var password = encoder.HashHMAC (Encoding.ASCII.GetBytes ("xUhs67g"), Encoding.ASCII.GetBytes (DateTimeOffset.UtcNow.ToUnixTimeSeconds ().ToString ()));

                User newCustomer = new User () {
                    Name = rm.Name,
                    Surname = rm.Surname,
                    Email = rm.Email,
                    Status = "New",
                    Username = username,
                    Password = password
                };

                _customerRepository.Create (newCustomer); //insertedid in baserepository if needed

                var successResponse = new {
                    successCode = 1
                };
                return Ok (successResponse);
            } catch (Exception e) {
                var errorJson = new {
                    error = e.ToString (),
                    error_message = e.Message
                };

                return new JsonResult (errorJson);
            }
        }

        [HttpPost]
        [Route ("registercomplete")]
        public async Task<IActionResult> RegisterComplete (RegisterComplete rc) {
            try {
                User userToComplete = await _customerRepository.Get (rc.Id);
                Address address = new Address () {
                    City = rc.City,
                    Full_Address = rc.Address
                };
                Detail Detail = new Detail () {
                    Address = address,
                    Birth_Date = Convert.ToDateTime (rc.BirthDate),
                    Identity_No = rc.IdentityNumber,
                    Invoice_No = rc.InvoiceNumber,
                    Phone_No = rc.Phone,
                    Language = DefaultLanguage,
                };

                userToComplete.Detail = Detail;

                //COMPLETE REGISTRATION
                _customerRepository.Update (userToComplete);

                var successResponse = new {
                    successCode = 1
                };
                return Ok (successResponse);
            } catch (Exception e) {
                var errorJson = new {
                    error = e.ToString (),
                    error_message = e.Message
                };

                return new JsonResult (errorJson);
            }
        }

        [Authorize]
        [HttpGet]
        [Route ("validatecustomertoken")]
        public IActionResult ValidateCustomerToken () {
            return Ok (true);
        }

        [HttpGet]
        [Route ("accessdenied")]
        public IActionResult AccessDenied () {
            return Unauthorized ("You do not have access to this service!");
        }

        /// <summary>
        /// TEST FUNCTIONS BEGIN
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route ("test_f1")]
        public IActionResult Test_F1 () {
            List<string> a = new List<string> ();
            var userId = encoder.Decrypt (HttpContext.User.FindFirst ("etp_user").Value);

            //BEGIN WATCH CLAIMS
            foreach (var claim in HttpContext.User.Claims) {
                a.Add (claim.Type + "IIIIIIIIIII" + claim.Value);
            }
            //END WATCH CLAIMS
            return Ok (userId.ToString ());
        }

        [Authorize]
        [HttpGet]
        [Route ("test_f2")]
        public async Task<ActionResult<IEnumerable<User>>> Test_F2 (string username, string password) {
            var t_result = await _customerRepository.Get (Builders<User>.Filter.And (Builders<User>.Filter.Eq ("Username", username),
                Builders<User>.Filter.Eq ("Password", password)));

            return Ok (t_result);
        }
        /// <summary>
        /// TEST FUNCTIONS END
        /// </summary>
    }
}
