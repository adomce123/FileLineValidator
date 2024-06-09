using FileLineValidator.Core.Interfaces;
using FileLineValidator.Core.Models;
using FileLineValidator.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FileLineValidator.Tests
{
    public class ContentValidatorServiceShould
    {
        private readonly Mock<ILogger<ContentValidatorService>> _mockLogger;
        private readonly Mock<IValidationRule> _mockAccountNumberValidationRule;
        private readonly Mock<IValidationRule> _mockFirstNameValidationRule;
        private readonly ContentValidatorService _contentValidatorService;

        public ContentValidatorServiceShould()
        {
            _mockLogger = new Mock<ILogger<ContentValidatorService>>();

            _mockAccountNumberValidationRule = new Mock<IValidationRule>();
            _mockAccountNumberValidationRule.Setup(r => r.ValidationName).Returns("Account number");

            _mockFirstNameValidationRule = new Mock<IValidationRule>();
            _mockFirstNameValidationRule.Setup(r => r.ValidationName).Returns("Account name");

            var validationRules = new List<IValidationRule>
            {
                _mockFirstNameValidationRule.Object,
                _mockAccountNumberValidationRule.Object
            };

            _contentValidatorService = new ContentValidatorService(_mockLogger.Object, validationRules);
        }

        private void ValidateContent(string content, ValidationResponse expectedResult)
        {
            // ACT
            var result = _contentValidatorService.ValidateContent(content);

            // ASSERT
            Assert.NotNull(result.InvalidLines);
            Assert.Equal(expectedResult.FileValid, result.FileValid);
            Assert.Equal(expectedResult.InvalidLines!, result.InvalidLines);
        }

        [Fact]
        public void PassValidation()
        {
            // ARRANGE
            _mockFirstNameValidationRule.Setup(r => r.Validate(It.IsAny<string>())).Returns(true);
            _mockAccountNumberValidationRule.Setup(r => r.Validate(It.IsAny<string>())).Returns(true);

            var content = "Rob 3113902p";
            var expectedResult = new ValidationResponse
            {
                FileValid = true,
                InvalidLines = new List<string>()
            };

            // ACT & ASSERT
            ValidateContent(content, expectedResult);

            // VERIFY
            _mockFirstNameValidationRule.Verify(r => r.Validate(It.IsAny<string>()), Times.Once);
            _mockAccountNumberValidationRule.Verify(r => r.Validate(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void FailValidation_InvalidAccountNameAndNumber()
        {
            // ARRANGE
            _mockFirstNameValidationRule.Setup(r => r.Validate("Richard")).Returns(true);
            _mockAccountNumberValidationRule.Setup(r => r.Validate("3293982")).Returns(true);
            _mockFirstNameValidationRule.Setup(r => r.Validate("XAEA-12")).Returns(false);
            _mockAccountNumberValidationRule.Setup(r => r.Validate("8293982")).Returns(false);

            var content = "Richard 3293982\r\nXAEA-12 8293982";
            var expectedResult = new ValidationResponse
            {
                FileValid = false,
                InvalidLines = new List<string>
                {
                    "Account name, Account number not valid for 2 line 'XAEA-12 8293982'"
                }
            };

            // ACT & ASSERT
            ValidateContent(content, expectedResult);
        }

        [Fact]
        public void FailValidation_InvalidAccountName()
        {
            // ARRANGE
            _mockFirstNameValidationRule.Setup(r => r.Validate("michael")).Returns(false);
            _mockAccountNumberValidationRule.Setup(r => r.Validate("3113902")).Returns(true);

            var content = "michael 3113902";
            var expectedResult = new ValidationResponse
            {
                FileValid = false,
                InvalidLines = new List<string>
                {
                    "Account name not valid for 1 line 'michael 3113902'"
                }
            };

            // ACT & ASSERT
            ValidateContent(content, expectedResult);
        }

        [Fact]
        public void FailValidation_InvalidAccountNumber()
        {
            // ARRANGE
            _mockFirstNameValidationRule.Setup(r => r.Validate("Rose")).Returns(true);
            _mockAccountNumberValidationRule.Setup(r => r.Validate("329a982")).Returns(false);

            var content = "Rose 329a982";
            var expectedResult = new ValidationResponse
            {
                FileValid = false,
                InvalidLines = new List<string>
                {
                    "Account number not valid for 1 line 'Rose 329a982'"
                }
            };

            // ACT & ASSERT
            ValidateContent(content, expectedResult);
        }

        [Fact]
        public void FailValidation_RandomInput()
        {
            // ARRANGE
            var content = "Rose 329a982 Randoooooom";
            var expectedResult = new ValidationResponse
            {
                FileValid = false,
                InvalidLines = new List<string>
                {
                    "Values in line does not match validation rules count for 1 line 'Rose 329a982 Randoooooom'"
                }
            };

            // ACT & ASSERT
            ValidateContent(content, expectedResult);

            // VERIFY
            _mockFirstNameValidationRule.Verify(r => r.Validate(It.IsAny<string>()), Times.Never);
            _mockAccountNumberValidationRule.Verify(r => r.Validate(It.IsAny<string>()), Times.Never);
        }
    }
}
