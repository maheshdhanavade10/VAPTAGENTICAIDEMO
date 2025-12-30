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
            // Validate and restrict user-controlled file path to a safe base directory
            if (string.IsNullOrWhiteSpace(filename))
            {
                return BadRequest("Filename is required.");
            }

            var baseDirectory = "C:\\data\\files";
            var fullBaseDirectory = Path.GetFullPath(baseDirectory);
            var combinedPath = Path.Combine(fullBaseDirectory, filename);
            var fullPath = Path.GetFullPath(combinedPath);

            // Ensure the resolved path is within the base directory to prevent path traversal
            var baseWithSeparator = fullBaseDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())
                ? fullBaseDirectory
                : fullBaseDirectory + Path.DirectorySeparatorChar;

            if (!fullPath.StartsWith(baseWithSeparator, System.StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid filename.");
            }

            string content = System.IO.File.ReadAllText(fullPath);

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
        // 7. Command Injection
        // ---------------------------------------------------------
        [HttpGet("run")]
        public IActionResult RunCommand(string cmd)
        {
            // ❌ Vulnerable: User input passed directly to shell
            System.Diagnostics.Process.Start("cmd.exe", "/C " + cmd);

            return Ok("Executed command");
        }
    }
}
