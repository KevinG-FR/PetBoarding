using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Abstractions;
using PetBoarding_Application.Email.Models;
using PetBoarding_Domain.Emails;

namespace PetBoarding_Application.Email.SendPaymentConfirmation;

public sealed class SendPaymentConfirmationCommandHandler : ICommandHandler<SendPaymentConfirmationCommand>
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;
    private readonly ILogger<SendPaymentConfirmationCommandHandler> _logger;

    public SendPaymentConfirmationCommandHandler(
        IEmailService emailService,
        IEmailTemplateService templateService,
        ILogger<SendPaymentConfirmationCommandHandler> logger)
    {
        _emailService = emailService;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<Result> Handle(SendPaymentConfirmationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Sending payment confirmation email to {Email} for payment {PaymentId}", 
                command.Email, command.PaymentId);

            var model = new PaymentConfirmationModel
            {
                CustomerName = command.CustomerName,
                PaymentId = command.PaymentId,
                Amount = command.Amount,
                PaymentDate = command.PaymentDate,
                PaymentMethod = command.PaymentMethod,
                Status = command.Status,
                ReservationNumber = command.ReservationNumber,
                ServiceName = command.ServiceName
            };

            var emailBody = await _templateService.RenderAsync("PaymentConfirmation", model, cancellationToken);

            var emailMessage = new EmailMessage
            {
                ToEmail = command.Email,
                ToName = command.CustomerName,
                Subject = $"Confirmation de paiement - {command.Amount:C} ✅",
                Body = emailBody,
                IsHtml = true
            };

            var result = await _emailService.SendAsync(emailMessage, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Payment confirmation email sent successfully to {Email} for payment {PaymentId}", 
                    command.Email, command.PaymentId);
                return Result.Ok();
            }

            _logger.LogError("Failed to send payment confirmation email to {Email}: {Error}", command.Email, result.ErrorMessage);
            return Result.Fail(EmailErrors.SendingFailed(result.ErrorMessage ?? "Unknown error"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending payment confirmation email to {Email}", command.Email);
            return Result.Fail(EmailErrors.SendingFailed(ex.Message));
        }
    }
}