using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Email.Models;
using PetBoarding_Domain.Emails;

namespace PetBoarding_Application.Core.Email.SendWelcomeEmail;

public sealed class SendWelcomeEmailCommandHandler : ICommandHandler<SendWelcomeEmailCommand>
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;
    private readonly ILogger<SendWelcomeEmailCommandHandler> _logger;

    public SendWelcomeEmailCommandHandler(
        IEmailService emailService,
        IEmailTemplateService templateService,
        ILogger<SendWelcomeEmailCommandHandler> logger)
    {
        _emailService = emailService;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<Result> Handle(SendWelcomeEmailCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Sending welcome email to {Email}", command.Email);

            var model = new WelcomeEmailModel
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                LoginUrl = "https://petboarding.com/login"
            };

            var emailBody = await _templateService.RenderAsync("WelcomeEmail", model, cancellationToken);

            var emailMessage = new EmailMessage
            {
                ToEmail = command.Email,
                ToName = $"{command.FirstName} {command.LastName}",
                Subject = "Bienvenue sur PetBoarding ! 🐾",
                Body = emailBody,
                IsHtml = true
            };

            var result = await _emailService.SendAsync(emailMessage, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Welcome email sent successfully to {Email}", command.Email);
                return Result.Ok();
            }

            _logger.LogError("Failed to send welcome email to {Email}: {Error}", command.Email, result.ErrorMessage);
            return Result.Fail(EmailErrors.SendingFailed(result.ErrorMessage ?? "Unknown error"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending welcome email to {Email}", command.Email);
            return Result.Fail(EmailErrors.SendingFailed(ex.Message));
        }
    }
}