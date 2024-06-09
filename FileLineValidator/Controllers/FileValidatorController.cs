using FileLineValidator.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileLineValidator.Controllers
{
    [ApiController]
    [Route("api/file-validator")]
    public class FileValidatorController : ControllerBase
    {
        private readonly IContentValidatorService _contentValidatorService;

        public FileValidatorController(
            IContentValidatorService contentValidatorService)
        {
            _contentValidatorService = contentValidatorService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile fileToValidate)
        {
            if (fileToValidate.Length == 0)
                return BadRequest("Attached file is empty");

            try
            {
                using var fileStream = fileToValidate.OpenReadStream();
                using var reader = new StreamReader(fileStream);
                var content = await reader.ReadToEndAsync();
                var validationResponse = _contentValidatorService.ValidateContent(content);

                return Ok(validationResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error while validating file: {ex.Message}");
            }
        }
    }
}
