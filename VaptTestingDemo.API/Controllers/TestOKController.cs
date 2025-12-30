using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Text.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace VaptTestingDemo.API.Controllers
{
    [ApiController]
    //[Route("api/test")]
    public class TestOKController : ControllerBase
    {

[HttpGet("read")] 
public IActionResult ReadFile(string filename) 
{ 
    // ❌ Vulnerable: user controls file path 
     var path = Path.Combine("C:\\data\\files", filename); 
    var content = System.IO.File.ReadAllText(path); 
    return Ok(content); 
    }

        //generate sample get request with ok response
        [HttpGet("auth")]
        public IActionResult GetAuthTest()
        {
            return Ok(new { Message = "Auth test successful" });
        }

        [HttpGet("sql")]
        public IActionResult SqlInjection(string input)
        {
            try
            {
                // Vulnerable SQL query construction
                string query = $"SELECT * FROM Users WHERE Username = '{input}'";

                // Add actual SQL execution to make it detectable by CodeQL
                using var connection = new SqlConnection("Server=localhost;Database=test;User=admin;Password=secret123");
                using var command = new SqlCommand(query, connection);
                connection.Open();
                var reader = command.ExecuteReader();  // This creates the exploitable path

                // Simulate reading results (though connection may fail)
                var results = new List<string>();
                while (reader.Read())
                {
                    results.Add(reader[0]?.ToString() ?? "null");
                }

                return Ok(new { Query = query, Results = results, Message = "SQL executed (vulnerable to injection)" });
            }
            catch (Exception ex)
            {
                // Return the vulnerable query even on error
                string query = $"SELECT * FROM Users WHERE Username = '{input}'";
                return Ok(new { Query = query, Error = ex.Message, Message = "This is vulnerable to SQL injection" });
            }
        }

    }
}