namespace FileLineValidator.Core.Models
{
    public class ValidationResponse
    {
        public bool FileValid { get; set; }
        public IEnumerable<string>? InvalidLines { get; set; }
    }
}
