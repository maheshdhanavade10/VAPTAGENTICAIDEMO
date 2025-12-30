using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace VaptTestingDemo.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
    

    //generate sample get request with ok response
    [HttpGet("auth")]
    public IActionResult GetAuthTest()
    {
        return Ok(new { Message = "Auth test successful" });
    }


    }
}