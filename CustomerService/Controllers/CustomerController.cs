using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net.Mail;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUserRepository _customerRepository;
        public Crypto encoder { get; set; }
        private IEmailSender _emailSender;
        public string DefaultLanguage { get; set; }
        public CustomerController(IUserRepository customerRepository, IEmailSender emailSender)
        {
            _customerRepository = customerRepository;
            encoder = new Crypto();
            DefaultLanguage = "TR";
            _emailSender = emailSender;
        }

        private static readonly string[] Customers = new[]
        {
            "Mert","Barış"
        };

        [Authorize]
        [HttpGet]
        [Route("index")]
        public IActionResult Index()
        {
            return Ok("Welcome to the customer microservice!");
        }

        [HttpGet]
        [Authorize]
        [Route("getallcustomers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllCustomers()
        {
            var customers = await _customerRepository.Get();
            return Ok(customers);
        }

        [Authorize]
        [HttpGet]
        [Route("getcustomerid")]
        public IActionResult GetCustomerId()
        {
            var userId = encoder.Decrypt(HttpContext.User.FindFirst("etp_user").Value);
            return Ok(userId.ToString());
        }

        [Authorize]
        [HttpPost]
        [Route("getcustomer")]
        public async Task<ActionResult<IEnumerable<User>>> GetCustomer()
        {
            var customer = await _customerRepository.Get(encoder.Decrypt(HttpContext.User.FindFirst("etp_user").Value));
            return Ok(customer);
        }

        [Obsolete]
        [Authorize]
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<IEnumerable<User>>> Login(string username, string password)
        {
            try
            {
                var securepassword = encoder.HashHMAC(Encoding.ASCII.GetBytes("xUhs67g"), Encoding.ASCII.GetBytes(password));
                var login_result = await _customerRepository.Get(Builders<User>.Filter.And(Builders<User>.Filter.Eq("Username", username), Builders<User>.Filter.Eq("Password", securepassword)));

                if (login_result != null)
                {
                    var updateStatus = login_result[0];
                    updateStatus.OnlineStatus = true;

                    _customerRepository.Update(updateStatus);

                    var successResponse = new
                    {
                        successCode = 1,
                        userInfo = login_result.FirstOrDefault()
                    };
                    return Ok(successResponse);
                }
                else
                {
                    var successResponse = new
                    {
                        successCode = 0,
                        message = "User does not exist or wrong information!"
                    };
                    return Ok(successResponse);
                }

            }
            catch (Exception e)
            {
                var errorJson = new
                {
                    error = e.ToString(),
                    error_message = e.Message
                };

                return new JsonResult(errorJson);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult<IEnumerable<User>>> LogOut(string username, string password)
        {
            try
            {
                var securepassword = encoder.HashHMAC(Encoding.ASCII.GetBytes("xUhs67g"), Encoding.ASCII.GetBytes(password));
                var logout_result = await _customerRepository.Get(Builders<User>.Filter.And(Builders<User>.Filter.Eq("Username", username), Builders<User>.Filter.Eq("Password", securepassword)));
                //status changed to false
                var updateStatus = logout_result[0];
                updateStatus.OnlineStatus = false;

                _customerRepository.Update(updateStatus);

                var successResponse = new
                {
                    successCode = 1
                };
                return Ok(successResponse);
            }
            catch (Exception e)
            {
                var errorJson = new
                {
                    error = e.ToString(),
                    error_message = e.Message
                };

                return new JsonResult(errorJson);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<JsonResult> Register(RegisterModel rm)
        {
            try
            {
                var control = await _customerRepository.Get(Builders<User>.Filter.Eq("Email", rm.Email));

                if(control.Count == 0)
                {
                    var username = "c_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + DateTimeOffset.UtcNow.Millisecond.ToString();
                    var securepassword = encoder.HashHMAC(Encoding.ASCII.GetBytes("xUhs67g"), Encoding.ASCII.GetBytes("no_password_set"));

                    //create the customer instance
                    User newCustomer = new User()
                    {
                        Name = rm.Name,
                        Surname = rm.Surname,
                        Email = rm.Email,
                        Status = "New",
                        Username = username,
                        Password = securepassword,
                        OnlineStatus = false,
                    };

                    var resultid = await _customerRepository.Create(newCustomer);

                    //SEND MAIL
                    //MAIL BEGIN

                    var To = rm.Email;
                    var Subject = "Verify your account";
                    var Body = "Please verify your email address by clicking here to move on to the next step in your energy trading platform membership." +
                               Environment.NewLine + "<br /><b><a href='www.google.com?id=" + resultid + "'>Verify My Account</a></b>";

                    _emailSender.Send(To, Subject, Body);

                    //MAIL END

                    var successJson = new
                    {
                        successCode = 1,
                        message = "Email was sent!"
                    };

                    return new JsonResult(successJson);
                }
                else
                {
                    var successJson = new
                    {
                        successCode = 0,
                        message = "Email already exist!"
                    };

                    return new JsonResult(successJson);
                }                
            }
            catch (Exception e)
            {
                var errorJson = new
                {
                    error = e.ToString(),
                    error_message = e.Message
                };

                return new JsonResult(errorJson);
            }
        }

        [HttpPost]
        [Route("registercomplete")]
        public async Task<IActionResult> RegisterComplete(RegisterComplete rc)
        {
            try
            {
                User userToComplete = await _customerRepository.Get(rc.Id);
                userToComplete.Status = "Waiting";
                //fill the address field
                Address address = new Address() { 
                    City = rc.City, 
                    Full_Address = rc.Address };
                //fill the customer details
                Detail Detail = new Detail() {
                    Address = address,
                    Birth_Date = Convert.ToDateTime(rc.BirthDate),
                    Identity_No = rc.IdentityNumber,
                    Invoice_No = rc.InvoiceNumber,
                    Phone_No = rc.Phone,
                    Language = DefaultLanguage,
                    Short_Location = "Default Location"
                };

                userToComplete.Detail = Detail;
                
                //COMPLETE REGISTRATION
                _customerRepository.Update(userToComplete);


                var successResponse = new
                {
                    successCode = 1
                };
                return Ok(successResponse);
            }
            catch (Exception e)
            {
                var errorJson = new
                {
                    error = e.ToString(),
                    error_message = e.Message
                };

                return new JsonResult(errorJson);
            }
        }

        [HttpPost]
        [Route("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            try
            {
                var passwordRecover = await _customerRepository.Get(Builders<User>.Filter.Eq("Email", Email));
                var userId = passwordRecover[0].Id;
                if(passwordRecover!=null)
                {
                    //SEND EMAIL
                    //EMAIL BEGIN

                    var To = Email;
                    var Subject = "Recover your password";
                    var Body = "You can create a new password by clicking the link below." +
                               Environment.NewLine + 
                               "<br /><b><a href='www.google.com?id=" + userId + "'>Set Password</a></b>";

                    _emailSender.Send(To, Subject, Body);

                    //EMAIL END


                    var successResponse = new
                    {
                        successCode = 1,
                        message = "Email was sent."
                    };
                    return Ok(successResponse);
                }
                else
                {
                    var successResponse = new
                    {
                        successCode = 0,
                        message = "Email does not exist."
                    };
                    return Ok(successResponse);
                }
                
            }
            catch (Exception e)
            {
                var errorJson = new
                {
                    error = e.ToString(),
                    error_message = e.Message
                };

                return new JsonResult(errorJson);
            }
        }

        [HttpPost]
        [Route("setpassword")]
        public async Task<IActionResult> SetPassword(string id,string password)
        {
            try
            {
                if(password != String.Empty)
                {
                    var passwordSetter = await _customerRepository.Get(id);
                    var securePassword = encoder.HashHMAC(Encoding.ASCII.GetBytes("xUhs67g"), Encoding.ASCII.GetBytes(password));
                    passwordSetter.Password = securePassword;
                    //Update the new hashed password
                    _customerRepository.Update(passwordSetter);

                    var successResponse = new
                    {
                        successCode = 1,
                        message = "New password has been set."
                    };
                    return Ok(successResponse);
                }
                else
                {
                    var successResponse = new
                    {
                        successCode = 0,
                        message = "An error occurred while setting the password."
                    };
                    return Ok(successResponse);
                }

            }
            catch (Exception e)
            {
                var errorJson = new
                {
                    error = e.ToString(),
                    error_message = e.Message
                };

                return new JsonResult(errorJson);
            }
        }


        [Authorize]
        [HttpGet]
        [Route("validatecustomertoken")]
        public JsonResult ValidateCustomerToken()
        {
            var responseJson = new
            {
                succesCode = 1,
                message = "Token is confirmed"
            };

            return new JsonResult(responseJson);
        }

        [HttpGet]
        [Route("accessdenied")]
        public IActionResult AccessDenied()
        {
            return Unauthorized("You do not have access to this service!");
        }

        /// <summary>
        /// TEST FUNCTIONS BEGIN
        /// </summary>
        /// 

        //[HttpGet]
        //[Route("test")]
        //public async Task<IActionResult> TestAsync()
        //{
        //    var data = await _testRepository.Get();
        //    return Ok();
        //}

        [Authorize]
        [HttpGet]
        [Route("test_f1")]
        public IActionResult Test_F1()
        {
            List<string> a = new List<string>();
            var userId = encoder.Decrypt(HttpContext.User.FindFirst("etp_user").Value);

            //BEGIN WATCH CLAIMS
            foreach (var claim in HttpContext.User.Claims)
            {
                a.Add(claim.Type + "IIIIIIIIIII" + claim.Value);
            }
            //END WATCH CLAIMS
            return Ok(userId.ToString());
        }

        [Authorize]
        [HttpGet]
        [Route("test_f2")]
        public async Task<ActionResult<IEnumerable<User>>> Test_F2(string username, string password)
        {
            var t_result = await _customerRepository.Get(Builders<User>.Filter.And(Builders<User>.Filter.Eq("Username", username),
                                                         Builders<User>.Filter.Eq("Password", password)));

            return Ok(t_result);
        }
        /// <summary>
        /// TEST FUNCTIONS END
        /// </summary>
    }
}