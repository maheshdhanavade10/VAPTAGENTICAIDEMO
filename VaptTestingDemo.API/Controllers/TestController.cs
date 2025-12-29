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
        // SQL Injection vulnerability
        [HttpGet("sql")]
        public IActionResult SqlInjection(string input)
        {
            // Simulate a vulnerable SQL query
            string query = $"SELECT * FROM Users WHERE Username = '{input}'";
            // In a real scenario, this would execute against a database
            return Ok(new { Query = query, Message = "This is vulnerable to SQL injection" });
        }

        // XSS vulnerability
        [HttpGet("xss")]
        public IActionResult CrossSiteScripting(string input)
        {
            // Return user input directly without sanitization
            string response = $"<html><body>Hello, {input}!</body></html>";
            return Content(response, "text/html");
        }

        // Broken Authentication
        [HttpGet("auth")]
        public IActionResult BrokenAuthentication()
        {
            // No authentication check - anyone can access
            return Ok(new { SensitiveData = "This should be protected", UserId = 12345 });
        }

        // Insecure Deserialization
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

        // Sensitive Data Exposure
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

        // Command Injection vulnerability
        [HttpGet("cmd")]
        public IActionResult CommandInjection(string input)
        {
            try
            {
                // Execute system command with user input - vulnerable to injection
                Process.Start("cmd.exe", $"/c echo {input}");
                return Ok(new { Message = $"Executed command: echo {input}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // Broken Access Control
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

        // Security Misconfiguration
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

        // Server-Side Request Forgery (SSRF)
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

        // Weak Authentication / Identification Failures
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