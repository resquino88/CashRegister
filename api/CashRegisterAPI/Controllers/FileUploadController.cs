using CashRegisterAPI.DTO;
using CashRegisterAPI.Utility;
using Microsoft.AspNetCore.Mvc;

namespace CashRegisterAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController(
      IFileParser fileParser) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> FileUpload ([FromForm] IFormFile file, [FromForm] UploadInfoDto uploadInfo)
        {
            try
            {
                var result = await fileParser.ProcessFile(file, uploadInfo);
                return File(result, "text/plain", "results.txt");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
