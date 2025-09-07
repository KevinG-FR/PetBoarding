using FluentResults;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Application.Core.Email.Models;
using PetBoarding_Domain.Emails;

namespace PetBoarding_Application.Core.Email.SendReservationConfirmation;

public sealed class SendReservationConfirmationCommandHandler : ICommandHandler<SendReservationConfirmationCommand>
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;
    private readonly ILogger<SendReservationConfirmationCommandHandler> _logger;

    public SendReservationConfirmationCommandHandler(
        IEmailService emailService,
        IEmailTemplateService templateService,
        ILogger<SendReservationConfirmationCommandHandler> logger)
    {
        _emailService = emailService;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task<Result> Handle(SendReservationConfirmationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Sending reservation confirmation email to {Email} for reservation {ReservationNumber}", 
                command.Email, command.ReservationNumber);

            var model = new ReservationConfirmationModel
            {
                CustomerName = command.CustomerName,
                PetName = command.PetName,
                ServiceName = command.ServiceName,
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                TotalAmount = command.TotalAmount,
                ReservationNumber = command.ReservationNumber,
                SpecialInstructions = command.SpecialInstructions
            };

            var emailBody = await _templateService.RenderAsync("ReservationConfirmation", model, cancellationToken);

            var emailMessage = new EmailMessage
            {
                ToEmail = command.Email,
                ToName = command.CustomerName,
                Subject = $"Confirmation de réservation #{command.ReservationNumber} 📅",
                Body = emailBody,
                IsHtml = true
            };

            var result = await _emailService.SendAsync(emailMessage, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Reservation confirmation email sent successfully to {Email} for reservation {ReservationNumber}", 
                    command.Email, command.ReservationNumber);
                return Result.Ok();
            }

            _logger.LogError("Failed to send reservation confirmation email to {Email}: {Error}", command.Email, result.ErrorMessage);
            return Result.Fail(EmailErrors.SendingFailed(result.ErrorMessage ?? "Unknown error"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending reservation confirmation email to {Email}", command.Email);
            return Result.Fail(EmailErrors.SendingFailed(ex.Message));
        }
    }
}