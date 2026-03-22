using CashRegisterAPI.DTO;

namespace CashRegisterAPI.Utility;

public interface IFileParser
{
    public Task<byte[]> ProcessFile(IFormFile file, UploadInfoDto uploadInfo);
}
