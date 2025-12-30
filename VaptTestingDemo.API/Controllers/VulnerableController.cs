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
        // ---------------------------------------------------------
        // 1. SQL Injection
        // ---------------------------------------------------------
        [HttpGet("user")]
        public IActionResult GetUser(string username)
        {
            // ❌ Vulnerable: SQL Injection
            string query = "SELECT * FROM Users WHERE Username = '" + username + "'";

            using var conn = new SqlConnection("Server=.;Database=Test;Trusted_Connection=True;");
            using var cmd = new SqlCommand(query, conn);

            conn.Open();
            cmd.ExecuteReader();

            return Ok("Executed vulnerable SQL query");
        }

        // ---------------------------------------------------------
        // 2. Hardcoded Secret
        // ---------------------------------------------------------
        private const string ApiKey = "HARDCODED-SECRET-12345"; // ❌ Hardcoded secret

        [HttpGet("secret")]
        public IActionResult ShowSecret()
        {
            return Ok($"Secret is: {ApiKey}");
        }

        // ---------------------------------------------------------
        // 3. Path Traversal
        // ---------------------------------------------------------
        [HttpGet("file")]
        public IActionResult ReadFile(string filename)
        {
            // ❌ Vulnerable: User-controlled file path
            string path = Path.Combine("C:\\data\\files", filename);
            string content = System.IO.File.ReadAllText(path);

            return Ok(content);
        }


        // ---------------------------------------------------------
        // 5. Weak Cryptography
        // ---------------------------------------------------------
        [HttpGet("hash")]
        public IActionResult WeakHash(string input)
        {
            // ❌ Vulnerable: MD5 is cryptographically broken
            using var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(bytes);

            return Ok(Convert.ToBase64String(hash));
        }

        // ---------------------------------------------------------
        // 6. Open Redirect
        // ---------------------------------------------------------
        [HttpGet("redirect")]
        public IActionResult RedirectTo(string url)
        {
            // ❌ Vulnerable: Unvalidated redirect
            return Redirect(url);
        }

        // ---------------------------------------------------------
        // 7. Command Execution (restricted)
        // ---------------------------------------------------------
        [HttpGet("run")]
        public IActionResult RunCommand(string cmd)
        {
            // Only allow a small set of predefined commands identified by key.
            if (string.IsNullOrWhiteSpace(cmd))
            {
                return BadRequest("No command specified.");
            }

            // Map user-provided key to hard-coded, safe commands.
            string arguments;
            switch (cmd)
            {
                case "time":
                    // Show current system time (Windows example)
                    arguments = "/C time /T";
                    break;
                case "whoami":
                    // Show the identity under which the process is running
                    arguments = "/C whoami";
                    break;
                default:
                    // Reject anything that is not explicitly allowed
                    return BadRequest("Unsupported command.");
            }

            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = System.Diagnostics.Process.Start(processStartInfo);
            if (process == null)
            {
                return StatusCode(500, "Failed to start process.");
            }

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                return StatusCode(500, error);
            }

            return Ok(string.IsNullOrEmpty(output) ? "Command executed." : output);
        }
    }
}
