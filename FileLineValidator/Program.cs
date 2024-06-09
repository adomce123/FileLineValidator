using FileLineValidator.Core.Interfaces;
using FileLineValidator.Core.Services;
using FileLineValidator.Core.ValidationRules;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSingleton<IContentValidatorService, ContentValidatorService>();
services.AddSingleton<IValidationRule, FirstNameValidationRule>();
services.AddSingleton<IValidationRule, AccountNumberValidationRule>();

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "hh:mm:ss ";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
