using MassTransit;
using Microsoft.Extensions.Logging;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Events;
using PetBoarding_Domain.Users;
using PetBoarding_Infrastructure.Events.Consumers;

namespace InfrastructureTests.Events.Consumers;

public class UserRegisteredEventConsumerTests
{
    private readonly Mock<IDomainEventHandler<UserRegisteredEvent>> _eventHandlerMock;
    private readonly Mock<ILogger<UserRegisteredEventConsumer>> _loggerMock;
    private readonly Mock<ConsumeContext<UserRegisteredEvent>> _contextMock;
    private readonly UserRegisteredEventConsumer _consumer;

    public UserRegisteredEventConsumerTests()
    {
        _eventHandlerMock = new Mock<IDomainEventHandler<UserRegisteredEvent>>();
        _loggerMock = new Mock<ILogger<UserRegisteredEventConsumer>>();
        _contextMock = new Mock<ConsumeContext<UserRegisteredEvent>>();
        
        _consumer = new UserRegisteredEventConsumer(_eventHandlerMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_WithValidEvent_ShouldCallHandlerAndLogSuccess()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var domainEvent = new UserRegisteredEvent(
            userId,
            "John",
            "Doe", 
            "john.doe@example.com",
            DateTime.UtcNow
        );

        _contextMock.Setup(x => x.Message).Returns(domainEvent);
        _contextMock.Setup(x => x.CancellationToken).Returns(CancellationToken.None);

        _eventHandlerMock.Setup(x => x.HandleAsync(domainEvent, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _eventHandlerMock.Verify(x => x.HandleAsync(domainEvent, CancellationToken.None), Times.Once);
        
        // Verify info log at start
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Consuming UserRegisteredEvent {domainEvent.EventId} for user {userId.Value}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        
        // Verify success log at end
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Successfully consumed UserRegisteredEvent {domainEvent.EventId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Consume_WhenHandlerThrowsException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var domainEvent = new UserRegisteredEvent(
            userId,
            "John",
            "Doe",
            "john.doe@example.com",
            DateTime.UtcNow
        );

        var exception = new Exception("Handler failed");

        _contextMock.Setup(x => x.Message).Returns(domainEvent);
        _contextMock.Setup(x => x.CancellationToken).Returns(CancellationToken.None);

        _eventHandlerMock.Setup(x => x.HandleAsync(domainEvent, It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act & Assert
        var act = async () => await _consumer.Consume(_contextMock.Object);
        await act.Should().ThrowAsync<Exception>().WithMessage("Handler failed");

        // Verify error log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error consuming UserRegisteredEvent {domainEvent.EventId} for user {userId.Value}")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        // Verify success log is NOT called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully consumed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task Consume_WithCancellationToken_ShouldPassTokenToHandler()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var domainEvent = new UserRegisteredEvent(
            userId,
            "John",
            "Doe",
            "john.doe@example.com",
            DateTime.UtcNow
        );

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _contextMock.Setup(x => x.Message).Returns(domainEvent);
        _contextMock.Setup(x => x.CancellationToken).Returns(cancellationToken);

        _eventHandlerMock.Setup(x => x.HandleAsync(domainEvent, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _eventHandlerMock.Verify(x => x.HandleAsync(domainEvent, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Consume_WithDifferentUserData_ShouldProcessCorrectly()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var domainEvent = new UserRegisteredEvent(
            userId,
            "Jane",
            "Smith",
            "jane.smith@example.com",
            new DateTime(2024, 1, 15, 10, 30, 0)
        );

        _contextMock.Setup(x => x.Message).Returns(domainEvent);
        _contextMock.Setup(x => x.CancellationToken).Returns(CancellationToken.None);

        _eventHandlerMock.Setup(x => x.HandleAsync(domainEvent, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _consumer.Consume(_contextMock.Object);

        // Assert
        _eventHandlerMock.Verify(x => x.HandleAsync(
            It.Is<UserRegisteredEvent>(e => 
                e.UserId == userId &&
                e.FirstName == "Jane" &&
                e.LastName == "Smith" &&
                e.Email == "jane.smith@example.com" &&
                e.RegistrationDate == new DateTime(2024, 1, 15, 10, 30, 0)),
            CancellationToken.None), 
            Times.Once);

        // Verify logging includes correct user data
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"for user {userId.Value}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}