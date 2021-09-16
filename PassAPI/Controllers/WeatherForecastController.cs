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
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{
			var rng = new Random();
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = rng.Next(-20, 55),
				Summary = Summaries[rng.Next(Summaries.Length)]
			})
			.ToArray();
		}


        [HttpPost]
        public async Task<IActionResult> UploadFile(
        IFormFile file,
        CancellationToken cancellationToken)
        {
            // Check this is a .pkpass file
            if (CheckIfPkPassFile(file))
            {
                // Write file to disk
                string filePath = await WriteFile(file);

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

        /// <summary>
        /// Method to check if file is pkpass file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckIfPkPassFile(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension == ".pkpass"); // Change the extension based on your need
        }

        private async Task<string> WriteFile(IFormFile file)
        {
            string fileName;
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

            if (!Directory.Exists(pathBuilt))
            {
                Directory.CreateDirectory(pathBuilt);
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
               fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return path;
        }
    }
}
