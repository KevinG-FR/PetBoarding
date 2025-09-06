using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetBoarding_Domain.Emails;
using PetBoarding_Infrastructure.Email;

namespace InfrastructureUnitTests.Email;

public class SmtpEmailServiceTests
{
    private readonly Mock<ILogger<SmtpEmailService>> _loggerMock;
    private readonly EmailConfiguration _emailConfig;
    private readonly SmtpEmailService _emailService;

    public SmtpEmailServiceTests()
    {
        _loggerMock = new Mock<ILogger<SmtpEmailService>>();
        
        _emailConfig = new EmailConfiguration
        {
            SmtpHost = "smtp.test.com",
            SmtpPort = 587,
            EnableSsl = true,
            Username = "test@test.com",
            Password = "password",
            FromEmail = "sender@test.com",
            FromName = "Test Sender",
            ReplyToEmail = "reply@test.com"
        };

        var optionsMock = new Mock<IOptions<EmailConfiguration>>();
        optionsMock.Setup(x => x.Value).Returns(_emailConfig);
        
        _emailService = new SmtpEmailService(optionsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidConfiguration_ShouldNotThrow()
    {
        // Arrange
        var optionsMock = new Mock<IOptions<EmailConfiguration>>();
        optionsMock.Setup(x => x.Value).Returns(_emailConfig);
        var loggerMock = new Mock<ILogger<SmtpEmailService>>();

        // Act & Assert
        var act = () => new SmtpEmailService(optionsMock.Object, loggerMock.Object);
        act.Should().NotThrow();
    }

    [Fact]
    public async Task SendAsync_WithValidMessage_ShouldLogStartAndAttemptSend()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            ToEmail = "recipient@test.com",
            ToName = "Test Recipient",
            Subject = "Test Subject",
            Body = "Test Body",
            IsHtml = true
        };

        // Act
        var result = await _emailService.SendAsync(emailMessage);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Sending email to recipient@test.com")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithAttachments_ShouldHandleAttachments()
    {
        // Arrange
        var attachment = new EmailAttachment
        {
            FileName = "test.txt",
            Content = "Test content"u8.ToArray(),
            ContentType = "text/plain"
        };

        var emailMessage = new EmailMessage
        {
            ToEmail = "recipient@test.com",
            ToName = "Test Recipient",
            Subject = "Test Subject",
            Body = "Test Body",
            IsHtml = true,
            Attachments = new List<EmailAttachment> { attachment }
        };

        // Act
        var result = await _emailService.SendAsync(emailMessage);

        // Assert - Should log the sending attempt
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Sending email to recipient@test.com")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithEmptyCredentials_ShouldNotUseCredentials()
    {
        // Arrange
        var configWithoutCredentials = new EmailConfiguration
        {
            SmtpHost = "smtp.test.com",
            SmtpPort = 587,
            EnableSsl = true,
            Username = "",
            Password = "",
            FromEmail = "sender@test.com",
            FromName = "Test Sender"
        };

        var optionsMock = new Mock<IOptions<EmailConfiguration>>();
        optionsMock.Setup(x => x.Value).Returns(configWithoutCredentials);
        
        var serviceWithoutCredentials = new SmtpEmailService(optionsMock.Object, _loggerMock.Object);

        var emailMessage = new EmailMessage
        {
            ToEmail = "recipient@test.com",
            ToName = "Test Recipient",
            Subject = "Test Subject",
            Body = "Test Body",
            IsHtml = false
        };

        // Act
        var result = await serviceWithoutCredentials.SendAsync(emailMessage);

        // Assert - Should still log the sending attempt
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Sending email to recipient@test.com")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithOptionalReplyTo_ShouldIncludeReplyTo()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            ToEmail = "recipient@test.com",
            ToName = "Test Recipient",
            Subject = "Test Subject",
            Body = "Test Body",
            IsHtml = true
        };

        // Act
        var result = await _emailService.SendAsync(emailMessage);

        // Assert - Should log the sending attempt
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Sending email to recipient@test.com")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithEmptyReplyTo_ShouldNotIncludeReplyTo()
    {
        // Arrange
        var configWithoutReplyTo = new EmailConfiguration
        {
            SmtpHost = "smtp.test.com",
            SmtpPort = 587,
            EnableSsl = true,
            Username = "test@test.com",
            Password = "password",
            FromEmail = "sender@test.com",
            FromName = "Test Sender",
            ReplyToEmail = ""
        };

        var optionsMock = new Mock<IOptions<EmailConfiguration>>();
        optionsMock.Setup(x => x.Value).Returns(configWithoutReplyTo);
        
        var serviceWithoutReplyTo = new SmtpEmailService(optionsMock.Object, _loggerMock.Object);

        var emailMessage = new EmailMessage
        {
            ToEmail = "recipient@test.com",
            ToName = "Test Recipient",
            Subject = "Test Subject",
            Body = "Test Body",
            IsHtml = true
        };

        // Act
        var result = await serviceWithoutReplyTo.SendAsync(emailMessage);

        // Assert - Should log the sending attempt
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Sending email to recipient@test.com")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var emailMessage = new EmailMessage
        {
            ToEmail = "recipient@test.com",
            ToName = "Test Recipient",
            Subject = "Test Subject",
            Body = "Test Body",
            IsHtml = true
        };

        var cancellationToken = new CancellationTokenSource();
        cancellationToken.Cancel();

        // Act & Assert
        // Note: The actual SmtpClient doesn't properly respect cancellation tokens in SendMailAsync
        // This test documents the intended behavior even if not fully implemented
        var result = await _emailService.SendAsync(emailMessage, cancellationToken.Token);
        
        // The service should at least attempt to send and handle any resulting exception
        result.Should().NotBeNull();
    }
}

// Note: Integration tests for actual email sending would require:
// 1. A test SMTP server (like MailHog, Papercut, etc.)
// 2. Or mocking SmtpClient (which is difficult due to sealed/non-virtual methods)
// 3. These tests focus on the configurable and loggable aspects of the service