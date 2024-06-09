namespace FileLineValidator.Core.Interfaces
{
    public interface IValidationRule
    {
        public string ValidationName { get; }
        public bool Validate(string value);
    }
}
