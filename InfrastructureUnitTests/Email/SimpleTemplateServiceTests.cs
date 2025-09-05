using System.Globalization;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PetBoarding_Infrastructure.Email;

namespace InfrastructureTests.Email;

public class SimpleTemplateServiceTests : IDisposable
{
    private readonly Mock<ILogger<SimpleTemplateService>> _loggerMock;
    private readonly EmailConfiguration _emailConfig;
    private readonly SimpleTemplateService _templateService;
    private readonly string _tempDirectory;
    private readonly string _templatesDirectory;

    public SimpleTemplateServiceTests()
    {
        _loggerMock = new Mock<ILogger<SimpleTemplateService>>();
        
        // Create temporary directory structure for tests
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _templatesDirectory = Path.Combine(_tempDirectory, "Email", "Templates");
        Directory.CreateDirectory(_templatesDirectory);

        // Change working directory to temp for template resolution
        var originalDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(_tempDirectory);

        _emailConfig = new EmailConfiguration
        {
            TemplatesPath = "Email/Templates"
        };

        var optionsMock = new Mock<IOptions<EmailConfiguration>>();
        optionsMock.Setup(x => x.Value).Returns(_emailConfig);
        
        _templateService = new SimpleTemplateService(optionsMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task RenderAsync_WithValidTemplate_ShouldRenderSuccessfully()
    {
        // Arrange
        var templateName = "welcome";
        var templateContent = "<h1>Welcome {{Name}}!</h1><p>Your email is {{Email}}</p>";
        await CreateTemplateFile(templateName, templateContent);

        var model = new { Name = "John Doe", Email = "john@example.com" };

        // Act
        var result = await _templateService.RenderAsync(templateName, model);

        // Assert
        result.Should().Be("<h1>Welcome John Doe!</h1><p>Your email is john@example.com</p>");
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Rendering email template: welcome")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RenderAsync_WithNonexistentTemplate_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var templateName = "nonexistent";
        var model = new { Name = "John" };

        // Act & Assert
        var act = async () => await _templateService.RenderAsync(templateName, model);
        await act.Should().ThrowAsync<FileNotFoundException>()
            .WithMessage("Template not found: *nonexistent.html");
        
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Template not found:")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RenderAsync_WithDateTimeProperty_ShouldFormatInFrench()
    {
        // Arrange
        var templateName = "date-test";
        var templateContent = "Date: {{AppointmentDate}}";
        await CreateTemplateFile(templateName, templateContent);

        var appointmentDate = new DateTime(2024, 12, 25, 14, 30, 0);
        var model = new { AppointmentDate = appointmentDate };

        // Act
        var result = await _templateService.RenderAsync(templateName, model);

        // Assert
        var expectedDate = appointmentDate.ToString("dddd dd MMMM yyyy à HH:mm", CultureInfo.GetCultureInfo("fr-FR"));
        result.Should().Be($"Date: {expectedDate}");
    }

    [Fact]
    public async Task RenderAsync_WithDecimalProperty_ShouldFormatAsCurrencyInFrench()
    {
        // Arrange
        var templateName = "price-test";
        var templateContent = "Price: {{Amount}}";
        await CreateTemplateFile(templateName, templateContent);

        var model = new { Amount = 123.45m };

        // Act
        var result = await _templateService.RenderAsync(templateName, model);

        // Assert
        var expectedAmount = 123.45m.ToString("C", CultureInfo.GetCultureInfo("fr-FR"));
        result.Should().Be($"Price: {expectedAmount}");
    }

    [Fact]
    public async Task RenderAsync_WithConditionalSections_ShouldShowSectionWhenValueExists()
    {
        // Arrange
        var templateName = "conditional-test";
        var templateContent = "Hello {{Name}}! {{#Notes}}Notes: {{Notes}}{{/Notes}}";
        await CreateTemplateFile(templateName, templateContent);

        var model = new { Name = "John", Notes = "Important information" };

        // Act
        var result = await _templateService.RenderAsync(templateName, model);

        // Assert
        result.Should().Be("Hello John! Notes: Important information");
    }

    [Fact]
    public async Task RenderAsync_WithConditionalSections_ShouldHideSectionWhenValueIsNull()
    {
        // Arrange
        var templateName = "conditional-null-test";
        var templateContent = "Hello {{Name}}! {{#Notes}}Notes: {{Notes}}{{/Notes}}End";
        await CreateTemplateFile(templateName, templateContent);

        var model = new { Name = "John", Notes = (string?)null };

        // Act
        var result = await _templateService.RenderAsync(templateName, model);

        // Assert
        result.Should().Be("Hello John! End");
    }

    [Fact]
    public async Task RenderAsync_WithConditionalSections_ShouldHideSectionWhenStringIsEmpty()
    {
        // Arrange
        var templateName = "conditional-empty-test";
        var templateContent = "Hello {{Name}}! {{#Notes}}Notes: {{Notes}}{{/Notes}}End";
        await CreateTemplateFile(templateName, templateContent);

        var model = new { Name = "John", Notes = "" };

        // Act
        var result = await _templateService.RenderAsync(templateName, model);

        // Assert
        result.Should().Be("Hello John! End");
    }

    [Fact]
    public async Task RenderAsync_WithConditionalSections_ShouldShowSectionWhenStringHasValue()
    {
        // Arrange
        var templateName = "conditional-value-test";
        var templateContent = "Hello {{Name}}! {{#Notes}}Notes: {{Notes}}{{/Notes}}End";
        await CreateTemplateFile(templateName, templateContent);

        var model = new { Name = "John", Notes = "Some notes" };

        // Act
        var result = await _templateService.RenderAsync(templateName, model);

        // Assert
        result.Should().Be("Hello John! Notes: Some notesEnd");
    }

    [Fact]
    public async Task RenderAsync_WithNullModel_ShouldReturnTemplateWithoutReplacement()
    {
        // Arrange
        var templateName = "null-model-test";
        var templateContent = "Hello {{Name}}! Your email is {{Email}}.";
        await CreateTemplateFile(templateName, templateContent);

        // Act
        var result = await _templateService.RenderAsync<object>(templateName, null!);

        // Assert
        result.Should().Be(templateContent);
    }

    [Fact]
    public async Task RenderAsync_WithNullPropertyValues_ShouldRemovePlaceholders()
    {
        // Arrange
        var templateName = "null-props-test";
        var templateContent = "Hello {{Name}}! Email: {{Email}} Phone: {{Phone}}";
        await CreateTemplateFile(templateName, templateContent);

        var model = new { Name = "John", Email = (string?)null, Phone = (string?)null };

        // Act
        var result = await _templateService.RenderAsync(templateName, model);

        // Assert
        result.Should().Be("Hello John! Email:  Phone: ");
    }

    [Fact]
    public async Task RenderAsync_WithComplexTemplate_ShouldHandleMultipleReplacements()
    {
        // Arrange
        var templateName = "complex-test";
        var templateContent = @"
<html>
<body>
    <h1>Hello {{Name}}!</h1>
    <p>Your reservation for {{AppointmentDate}} has been confirmed.</p>
    <p>Total amount: {{Amount}}</p>
    {{#Notes}}
    <div class=""notes"">
        <h3>Additional Notes:</h3>
        <p>{{Notes}}</p>
    </div>
    {{/Notes}}
    <p>Thank you for choosing our service!</p>
</body>
</html>";
        await CreateTemplateFile(templateName, templateContent);

        var appointmentDate = new DateTime(2024, 12, 25, 14, 30, 0);
        var model = new 
        { 
            Name = "John Doe", 
            AppointmentDate = appointmentDate,
            Amount = 150.75m,
            Notes = "Please bring vaccination records"
        };

        // Act
        var result = await _templateService.RenderAsync(templateName, model);

        // Assert
        result.Should().Contain("Hello John Doe!");
        result.Should().Contain(appointmentDate.ToString("dddd dd MMMM yyyy à HH:mm", CultureInfo.GetCultureInfo("fr-FR")));
        result.Should().Contain(150.75m.ToString("C", CultureInfo.GetCultureInfo("fr-FR")));
        result.Should().Contain("Please bring vaccination records");
    }

    [Fact]
    public async Task RenderAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var templateName = "cancellation-test";
        var templateContent = "Hello {{Name}}!";
        await CreateTemplateFile(templateName, templateContent);

        var model = new { Name = "John" };
        var cancellationToken = new CancellationTokenSource();
        cancellationToken.Cancel();

        // Act & Assert
        var act = async () => await _templateService.RenderAsync(templateName, model, cancellationToken.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    private async Task CreateTemplateFile(string templateName, string content)
    {
        var filePath = Path.Combine(_templatesDirectory, $"{templateName}.html");
        await File.WriteAllTextAsync(filePath, content);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }
        catch
        {
            // Ignore cleanup errors in tests
        }
    }
}