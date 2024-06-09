using FileLineValidator.Core.Interfaces;
using System.Text.RegularExpressions;

namespace FileLineValidator.Core.ValidationRules
{
    public class AccountNumberValidationRule : IValidationRule
    {
        // Valid account number must start with a digit 3 or 4 
        // Account number is a 7 digit number (ex. 3293982) or 7 digit number + 'p' at the end (4113902p)
        private static readonly Regex _validAccountNumberRegex = new Regex(@"^[34]\d{6}(p)?$", RegexOptions.Compiled);
        public string ValidationName => "Account number";

        public bool Validate(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (!_validAccountNumberRegex.IsMatch(value))
                return false;

            return true;
        }
    }
}
