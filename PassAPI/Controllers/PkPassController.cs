using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PassAPI.Controllers
{
     public class UploadModel
    {
        public string data { get; set; }
        public string fileName { get; set; }
    }


	[ApiController]
	[Route("[controller]")]

	public class PkPassController : ControllerBase
	{
		[HttpGet]
		public OkObjectResult Get()
		{
			return Ok("PONG");
		}


		[HttpPost]
        public async Task<IActionResult> UploadFile([FromBody] UploadModel file)
        {
			Console.WriteLine(file.fileName);

            // Check this is a .pkpass file
            if (CheckIfPkPassFile(file))
            {
                // Write file to disk
                string filePath = await WriteFile(file);
                Console.WriteLine($"Converting: {filePath}");

                // Convert
                TicketPdfMaker t = new TicketPdfMaker(filePath);
                string outputFilename = Path.ChangeExtension(filePath, "pdf");
                t.Test(false, outputFilename);

                // Send back
                using (var sr = new StreamReader(outputFilename))
                {
                    var content = await sr.ReadToEndAsync();
                    return Ok(content);
                }

            }
            else
            {
                return BadRequest(new { message = "Invalid file extension" });
            }
        }

		private bool CheckIfPkPassFile(UploadModel file)
        {
            string extension = Path.GetExtension(file.fileName);
            return (extension == ".pkpass");
        }

        private async Task<string> WriteFile(UploadModel file)
        {
            string pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files", file.fileName);

            System.IO.File.WriteAllBytes(path, Convert.FromBase64String(file.data));
            return path;
        }
	}
}
