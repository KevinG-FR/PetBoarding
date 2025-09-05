using Enyim.Caching;
using Microsoft.Extensions.Logging;
using PetBoarding_Infrastructure.Caching;

namespace InfrastructureTests.Caching;

public class MemcachedServiceTests
{
    private readonly Mock<IMemcachedClient> _memcachedClientMock;
    private readonly Mock<ILogger<MemcachedService>> _loggerMock;
    private readonly MemcachedService _cacheService;

    public MemcachedServiceTests()
    {
        _memcachedClientMock = new Mock<IMemcachedClient>();
        _loggerMock = new Mock<ILogger<MemcachedService>>();
        _cacheService = new MemcachedService(_memcachedClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldNotThrow()
    {
        // Arrange & Act
        var act = () => new MemcachedService(_memcachedClientMock.Object, _loggerMock.Object);
        
        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task GetAsync_WhenExceptionOccurs_ShouldLogErrorAndReturnNull()
    {
        // Arrange
        var key = "error-key";
        var exception = new Exception("Cache error");
        
        _memcachedClientMock.Setup(x => x.GetAsync<TestObject>(key))
            .ThrowsAsync(exception);

        // Act
        var result = await _cacheService.GetAsync<TestObject>(key);

        // Assert
        result.Should().BeNull();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error retrieving cache key: error-key")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_WithDefaultExpiration_ShouldCallMemcachedClientWithDefaultExpiration()
    {
        // Arrange
        var key = "test-key";
        var value = new TestObject { Id = 1, Name = "Test" };
        var defaultExpirationSeconds = (int)TimeSpan.FromMinutes(30).TotalSeconds;
        
        _memcachedClientMock.Setup(x => x.SetAsync(key, value, defaultExpirationSeconds))
            .ReturnsAsync(true);

        // Act
        var result = await _cacheService.SetAsync(key, value);

        // Assert
        _memcachedClientMock.Verify(x => x.SetAsync(key, value, defaultExpirationSeconds), Times.Once);
    }

    [Fact]
    public async Task SetAsync_WithCustomExpiration_ShouldCallMemcachedClientWithCustomExpiration()
    {
        // Arrange
        var key = "test-key";
        var value = new TestObject { Id = 1, Name = "Test" };
        var expiration = TimeSpan.FromMinutes(15);
        
        _memcachedClientMock.Setup(x => x.SetAsync(key, value, (int)expiration.TotalSeconds))
            .ReturnsAsync(true);

        // Act
        await _cacheService.SetAsync(key, value, expiration);

        // Assert
        _memcachedClientMock.Verify(x => x.SetAsync(key, value, (int)expiration.TotalSeconds), Times.Once);
    }

    [Fact]
    public async Task SetAsync_WhenExceptionOccurs_ShouldLogErrorAndReturnFalse()
    {
        // Arrange
        var key = "error-key";
        var value = new TestObject { Id = 1, Name = "Test" };
        var exception = new Exception("Cache error");
        
        _memcachedClientMock.Setup(x => x.SetAsync(key, value, It.IsAny<int>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _cacheService.SetAsync(key, value);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error setting cache key: error-key")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ReplaceAsync_WithCustomExpiration_ShouldCallMemcachedClientWithCustomExpiration()
    {
        // Arrange
        var key = "test-key";
        var value = new TestObject { Id = 1, Name = "Updated Test" };
        var expiration = TimeSpan.FromMinutes(10);
        
        _memcachedClientMock.Setup(x => x.ReplaceAsync(key, value, (int)expiration.TotalSeconds))
            .ReturnsAsync(true);

        // Act
        await _cacheService.ReplaceAsync(key, value, expiration);

        // Assert
        _memcachedClientMock.Verify(x => x.ReplaceAsync(key, value, (int)expiration.TotalSeconds), Times.Once);
    }

    [Fact]
    public async Task ReplaceAsync_WhenExceptionOccurs_ShouldLogErrorAndReturnFalse()
    {
        // Arrange
        var key = "error-key";
        var value = new TestObject { Id = 1, Name = "Test" };
        var exception = new Exception("Cache error");
        
        _memcachedClientMock.Setup(x => x.ReplaceAsync(key, value, It.IsAny<int>()))
            .ThrowsAsync(exception);

        // Act
        var result = await _cacheService.ReplaceAsync(key, value);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error replacing cache key: error-key")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallMemcachedClient()
    {
        // Arrange
        var key = "test-key";
        
        _memcachedClientMock.Setup(x => x.RemoveAsync(key))
            .ReturnsAsync(true);

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert
        _memcachedClientMock.Verify(x => x.RemoveAsync(key), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_WhenExceptionOccurs_ShouldLogErrorAndReturnFalse()
    {
        // Arrange
        var key = "error-key";
        var exception = new Exception("Cache error");
        
        _memcachedClientMock.Setup(x => x.RemoveAsync(key))
            .ThrowsAsync(exception);

        // Act
        var result = await _cacheService.RemoveAsync(key);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error removing cache key: error-key")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WhenExceptionOccurs_ShouldLogErrorAndReturnFalse()
    {
        // Arrange
        var key = "error-key";
        var exception = new Exception("Cache error");
        
        _memcachedClientMock.Setup(x => x.GetAsync<TestObject>(key))
            .ThrowsAsync(exception);

        // Act
        var result = await _cacheService.ExistsAsync<TestObject>(key);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error checking cache key existence: error-key")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenGetValueAsyncReturnsNull_ShouldCallFactory()
    {
        // Arrange
        var key = "new-key";
        var factoryValue = new TestObject { Id = 2, Name = "Factory Created" };
        var expiration = TimeSpan.FromMinutes(15);
        
        _memcachedClientMock.Setup(x => x.GetValueAsync<TestObject>(key))
            .ReturnsAsync((TestObject?)null);

        _memcachedClientMock.Setup(x => x.SetAsync(key, factoryValue, (int)expiration.TotalSeconds))
            .ReturnsAsync(true);

        var factory = new Mock<Func<Task<TestObject?>>>();
        factory.Setup(x => x()).ReturnsAsync(factoryValue);

        // Act
        await _cacheService.GetOrCreateAsync(key, factory.Object, expiration);

        // Assert
        factory.Verify(x => x(), Times.Once);
        _memcachedClientMock.Verify(x => x.SetAsync(key, factoryValue, (int)expiration.TotalSeconds), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAsync_WhenFactoryReturnsNull_ShouldNotCache()
    {
        // Arrange
        var key = "null-key";
        
        _memcachedClientMock.Setup(x => x.GetValueAsync<TestObject>(key))
            .ReturnsAsync((TestObject?)null);

        var factory = new Mock<Func<Task<TestObject?>>>();
        factory.Setup(x => x()).ReturnsAsync((TestObject?)null);

        // Act
        await _cacheService.GetOrCreateAsync(key, factory.Object);

        // Assert
        factory.Verify(x => x(), Times.Once);
        _memcachedClientMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<TestObject>(), It.IsAny<int>()), Times.Never);
    }

    public class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}