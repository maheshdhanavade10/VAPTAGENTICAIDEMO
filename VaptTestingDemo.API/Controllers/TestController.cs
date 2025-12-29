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
        // SQL Injection vulnerability - Now with actual execution for CodeQL detection
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

        // XSS vulnerability - Direct HTML output
        [HttpGet("xss")]
        public IActionResult CrossSiteScripting(string input)
        {
            // Return user input directly without sanitization
            string response = $"<html><body>Hello, {input}!</body></html>";
            return Content(response, "text/html");
        }

        // Broken Authentication - No auth checks
        [HttpGet("auth")]
        public IActionResult BrokenAuthentication()
        {
            // No authentication check - anyone can access
            return Ok(new { SensitiveData = "This should be protected", UserId = 12345 });
        }

        // Insecure Deserialization - Deserialize without type checking
        [HttpPost("deserialization")]
        public IActionResult InsecureDeserialization([FromBody] string jsonInput)
        {
            try
            {
                // Deserialize without type checking - vulnerable to injection
                var obj = JsonSerializer.Deserialize<object>(jsonInput);
                return Ok(new { Deserialized = obj, Message = "Deserialized successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // Sensitive Data Exposure - Hardcoded sensitive data
        [HttpGet("data")]
        public IActionResult SensitiveDataExposure()
        {
            // Return sensitive data without encryption or proper handling
            return Ok(new
            {
                CreditCard = "4111111111111111",
                SSN = "123-45-6789",
                Password = "password123",
                ApiKey = "sk-1234567890abcdef"
            });
        }

        // Command Injection - Now with actual command execution
        [HttpGet("cmd")]
        public IActionResult CommandInjection(string input)
        {
            try
            {
                // Actual command execution - vulnerable to injection
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",  // Windows command prompt
                        Arguments = $"/c echo {input}",  // Vulnerable to injection
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                
                return Ok(new { Command = $"echo {input}", Output = output, Message = "Command executed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // Broken Access Control - No authorization
        [HttpGet("admin")]
        public IActionResult BrokenAccessControl()
        {
            // No role-based access control - anyone can access admin data
            return Ok(new
            {
                AdminData = "This is admin-only data",
                UserCount = 1000,
                SystemConfig = "Secret config"
            });
        }

        // Security Misconfiguration - Exposed config
        [HttpGet("config")]
        public IActionResult SecurityMisconfiguration()
        {
            // Expose sensitive configuration information
            return Ok(new
            {
                DatabaseConnection = "Server=localhost;Database=test;User=admin;Password=secret123",
                ApiKeys = new[] { "key1", "key2", "key3" },
                DebugMode = true,
                Environment = "Production"
            });
        }

        // Server-Side Request Forgery (SSRF) - Unvalidated URL
        [HttpGet("ssrf")]
        public async Task<IActionResult> ServerSideRequestForgery(string url)
        {
            try
            {
                using var client = new HttpClient();
                // No URL validation - vulnerable to SSRF
                var response = await client.GetStringAsync(url);
                return Ok(new { Url = url, Content = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // Weak Authentication - Hardcoded credentials
        [HttpPost("login")]
        public IActionResult WeakAuthentication([FromBody] LoginRequest request)
        {
            // Simple hardcoded check - no proper authentication
            if (request.Username == "admin" && request.Password == "password")
            {
                return Ok(new { Token = "fake-jwt-token", Message = "Login successful" });
            }
            return Unauthorized(new { Message = "Invalid credentials" });
        }
    }

    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}