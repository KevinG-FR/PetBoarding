using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetBoarding_Domain.Emails;

namespace PetBoarding_Infrastructure.Email;

public sealed class SimpleTemplateService : IEmailTemplateService
{
    private readonly EmailConfiguration _config;
    private readonly ILogger<SimpleTemplateService> _logger;

    public SimpleTemplateService(IOptions<EmailConfiguration> config, ILogger<SimpleTemplateService> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    public async Task<string> RenderAsync<T>(string templateName, T model, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rendering email template: {TemplateName}", templateName);

            // Build path to HTML template
            var currentDirectory = Directory.GetCurrentDirectory();
            var templatePath = Path.Combine(currentDirectory, "..", "PetBoarding_Infrastructure", "Email", "Templates", $"{templateName}.html");
            templatePath = Path.GetFullPath(templatePath);
            
            _logger.LogDebug("Looking for template at: {TemplatePath}", templatePath);
            
            if (!File.Exists(templatePath))
            {
                _logger.LogError("Template not found: {TemplatePath}", templatePath);
                throw new FileNotFoundException($"Template not found: {templatePath}");
            }

            var templateContent = await File.ReadAllTextAsync(templatePath, cancellationToken);
            
            // Replace placeholders with model values
            var result = ReplacePlaceholders(templateContent, model);

            _logger.LogInformation("Template {TemplateName} rendered successfully", templateName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to render template {TemplateName}: {ErrorMessage}", 
                templateName, ex.Message);
            throw;
        }
    }

    private string ReplacePlaceholders<T>(string template, T model)
    {
        if (model == null) return template;

        var result = template;
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(model);
            var placeholder = $"{{{{{property.Name}}}}}";
            
            if (value != null)
            {
                string stringValue = value switch
                {
                    DateTime dateTime => dateTime.ToString("dddd dd MMMM yyyy à HH:mm", CultureInfo.GetCultureInfo("fr-FR")),
                    decimal decimalValue => decimalValue.ToString("C", CultureInfo.GetCultureInfo("fr-FR")),
                    _ => value.ToString() ?? ""
                };
                
                result = result.Replace(placeholder, stringValue);
                
                // Handle conditional sections
                var conditionalStart = $"{{{{#{property.Name}}}}}";
                var conditionalEnd = $"{{{{/{property.Name}}}}}";
                
                if (result.Contains(conditionalStart))
                {
                    var startIndex = result.IndexOf(conditionalStart);
                    var endIndex = result.IndexOf(conditionalEnd);
                    
                    if (startIndex >= 0 && endIndex >= 0)
                    {
                        var beforeSection = result.Substring(0, startIndex);
                        var sectionContent = result.Substring(startIndex + conditionalStart.Length, 
                            endIndex - startIndex - conditionalStart.Length);
                        var afterSection = result.Substring(endIndex + conditionalEnd.Length);
                        
                        // Show section only if value is not null/empty
                        var showSection = value switch
                        {
                            string str => !string.IsNullOrWhiteSpace(str),
                            null => false,
                            _ => true
                        };
                        
                        result = beforeSection + (showSection ? sectionContent : "") + afterSection;
                    }
                }
            }
            else
            {
                // Remove placeholder and conditional sections for null values
                result = result.Replace(placeholder, "");
                
                var conditionalStart = $"{{{{#{property.Name}}}}}";
                var conditionalEnd = $"{{{{/{property.Name}}}}}";
                
                if (result.Contains(conditionalStart))
                {
                    var startIndex = result.IndexOf(conditionalStart);
                    var endIndex = result.IndexOf(conditionalEnd);
                    
                    if (startIndex >= 0 && endIndex >= 0)
                    {
                        var beforeSection = result.Substring(0, startIndex);
                        var afterSection = result.Substring(endIndex + conditionalEnd.Length);
                        result = beforeSection + afterSection;
                    }
                }
            }
        }

        return result;
    }
}