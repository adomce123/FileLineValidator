using FileLineValidator.Core.Interfaces;
using System.Text.RegularExpressions;

namespace FileLineValidator.Core.ValidationRules
{
    public class FirstNameValidationRule : IValidationRule
    {
        // First name should only consist of alphabetic characters
        private static readonly Regex _alphabeticRegex = new Regex("^[A-Za-z]+$", RegexOptions.Compiled);
        public string ValidationName => "Account name";

        public bool Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (!char.IsUpper(value[0]))
                return false;

            if (!_alphabeticRegex.IsMatch(value))
                return false;

            return true;
        }
    }
}
