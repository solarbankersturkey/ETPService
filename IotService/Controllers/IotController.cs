using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IotService.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IotService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IotController : ControllerBase
    {
        private readonly ICumulativeRepository _cumulativeRepository;
        public IotController(ICumulativeRepository cumulativeRepository)
        {
            _cumulativeRepository = cumulativeRepository;
        }


        // GET: api/Iot
        [Authorize]
        [HttpGet]
        [Route("index")]
        public IEnumerable<string> Index()
        {
            return new string[] { "value1", "value2" };
        }

        [Authorize]
        [HttpPost]
        [Route("senddata")]
        public IActionResult SendData(Cumulative data)
        {

            _cumulativeRepository.Create(data);
            return Ok("Başarılı");
        }


        [HttpGet]
        [Route("accessdenied")]
        public IActionResult AccessDenied()
        {
            return Unauthorized("You do not have access to this service!");
        }


    }
}
