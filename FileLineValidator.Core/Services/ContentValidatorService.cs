using FileLineValidator.Core.Interfaces;
using FileLineValidator.Core.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace FileLineValidator.Core.Services
{
    public class ContentValidatorService : IContentValidatorService
    {
        private readonly ILogger<ContentValidatorService> _logger;
        private readonly IEnumerable<IValidationRule> _validationRules;

        private static readonly string[] _lineEndings = { "\r\n", "\r", "\n" };
        private readonly ConcurrentBag<(int LineNumber, string Message)> _invalidLines = new();

        public ContentValidatorService(
            ILogger<ContentValidatorService> logger,
            IEnumerable<IValidationRule> validationRules)
        {
            _logger = logger;
            _validationRules = validationRules;
        }

        public ValidationResponse ValidateContent(string content)
        {
            _logger.LogInformation("Starting to process file...");

            _invalidLines.Clear();

            var fileStopwatch = Stopwatch.StartNew();

            var lines = content.Split(_lineEndings, StringSplitOptions.RemoveEmptyEntries);

            Parallel.For(0, lines.Length, i =>
            {
                var lineStopwatch = Stopwatch.StartNew();

                var line = lines[i].Trim();
                var parts = line.Split(' ');
                if (parts.Length != _validationRules.Count())
                {
                    _invalidLines.Add((i + 1, $"Values in line does not match validation rules count " +
                        $"for {i + 1} line '{line}'"));

                    return;
                }

                int partsIndex = 0;
                var validationMessage = new StringBuilder();
                foreach (var validationRule in _validationRules)
                {
                    if (!validationRule.Validate(parts[partsIndex]))
                    {
                        if (validationMessage.Length > 0)
                            validationMessage.Append(", ");

                        validationMessage.Append(validationRule.ValidationName);
                    }
                    partsIndex++;
                }

                if (validationMessage.Length > 0)
                    _invalidLines.Add((i + 1, $"{validationMessage} not valid for {i + 1} line '{line}'"));

                LogProcessingTime(lineStopwatch, line);
            });

            var validationResult = PrepareResult();

            LogProcessingTime(fileStopwatch);

            return validationResult;
        }

        private ValidationResponse PrepareResult()
        {
            var sortedInvalidLines = _invalidLines
                .OrderBy(l => l.LineNumber)
                .Select(l => l.Message)
                .ToList();

            return new ValidationResponse()
            {
                FileValid = !sortedInvalidLines.Any(),
                InvalidLines = sortedInvalidLines,
            };
        }

        private void LogProcessingTime(Stopwatch stopwatch, string? line = null)
        {
            stopwatch.Stop();
            double elapsedMilliseconds = stopwatch.ElapsedTicks / (Stopwatch.Frequency / 1000.0);

            if (line != null)
                _logger.LogInformation($"Validating line {line} took {elapsedMilliseconds} ms");
            else
                _logger.LogInformation($"File processing took: {elapsedMilliseconds} ms");
        }
    }
}
