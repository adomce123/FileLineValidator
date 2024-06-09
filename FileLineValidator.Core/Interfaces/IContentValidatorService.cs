using FileLineValidator.Core.Models;

namespace FileLineValidator.Core.Interfaces
{
    public interface IContentValidatorService
    {
        ValidationResponse ValidateContent(string content);
    }
}