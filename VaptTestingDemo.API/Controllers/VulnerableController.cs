using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace DemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VulnerableController : ControllerBase
    {
        //generate sample get request with ok response
        [HttpGet("auth")]
        public IActionResult GetAuthTest()
        {
            return Ok(new { Message = "Auth test successful" });
        }
    }
}
