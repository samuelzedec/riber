using FluentAssertions;
using Riber.Domain.Tests;
using Riber.Infrastructure.Services.AWS.Email;

namespace Riber.Infrastructure.Tests.Services.AWS.Email;

public sealed class EmailTemplateRenderTests : BaseTest
{
    private readonly EmailTemplateRender _sut;
    private readonly string _templateDirectory;

    public EmailTemplateRenderTests()
    {
        _sut = new EmailTemplateRender();
        _templateDirectory = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Services",
            "AWS",
            "Email",
            "Templates"
        );
        
        Directory.CreateDirectory(_templateDirectory);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should read and return template content successfully")]
    public async Task GetTemplateAsync_WhenTemplateExists_ShouldReturnContent()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        const string templateContent = "<h1>Test Template</h1>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>();

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(templateContent);
        
        // Cleanup
        File.Delete(templatePath);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should replace single placeholder with value")]
    public async Task GetTemplateAsync_WhenTemplateHasSinglePlaceholder_ShouldReplaceWithValue()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        var userName = _faker.Person.FullName;
        const string templateContent = "<h1>Hello {{name}}!</h1>";
        var expectedContent = $"<h1>Hello {userName}!</h1>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>
        {
            ["name"] = userName
        };

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(expectedContent);
        
        // Cleanup
        File.Delete(templatePath);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should replace multiple placeholders with values")]
    public async Task GetTemplateAsync_WhenTemplateHasMultiplePlaceholders_ShouldReplaceAllWithValues()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        var userName = _faker.Person.FullName;
        var userEmail = _faker.Internet.Email();
        var companyName = _faker.Company.CompanyName();
        
        const string templateContent = "<h1>Hello {{name}}!</h1><p>Email: {{email}}</p><p>Company: {{company}}</p>";
        var expectedContent = $"<h1>Hello {userName}!</h1><p>Email: {userEmail}</p><p>Company: {companyName}</p>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>
        {
            ["name"] = userName,
            ["email"] = userEmail,
            ["company"] = companyName
        };

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(expectedContent);
        
        // Cleanup
        File.Delete(templatePath);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw FileNotFoundException when template does not exist")]
    public async Task GetTemplateAsync_WhenTemplateDoesNotExist_ShouldThrowFileNotFoundException()
    {
        // Arrange
        var nonExistentTemplate = $"{_faker.Random.Guid()}.html";
        var data = new Dictionary<string, object?>();

        // Act
        var act = async () => await _sut.GetTemplateAsync(nonExistentTemplate, data);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<FileNotFoundException>()
            .WithMessage($"Template não encontrado: *{nonExistentTemplate}");
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle template with no placeholders")]
    public async Task GetTemplateAsync_WhenTemplateHasNoPlaceholders_ShouldReturnOriginalContent()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        const string templateContent = "<h1>Static Content</h1><p>No placeholders here</p>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>
        {
            ["unusedProperty"] = "This won't be replaced"
        };

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(templateContent);
        
        // Cleanup
        File.Delete(templatePath);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle special characters in placeholder values")]
    public async Task GetTemplateAsync_WhenPlaceholderHasSpecialCharacters_ShouldReplaceCorrectly()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        var specialContent = "<script>alert('XSS')</script> & \"quotes\" 'apostrophe' €";
        const string templateContent = "<div>{{content}}</div>";
        var expectedContent = $"<div>{specialContent}</div>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>
        {
            ["content"] = specialContent
        };

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(expectedContent);
        
        // Cleanup
        File.Delete(templatePath);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle same placeholder appearing multiple times")]
    public async Task GetTemplateAsync_WhenPlaceholderAppearsMultipleTimes_ShouldReplaceAllOccurrences()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        var userName = _faker.Person.FullName;
        const string templateContent = "<h1>Hello {{name}}!</h1><p>Welcome {{name}}</p><footer>{{name}}</footer>";
        var expectedContent = $"<h1>Hello {userName}!</h1><p>Welcome {userName}</p><footer>{userName}</footer>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>
        {
            ["name"] = userName
        };

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(expectedContent);
        
        // Cleanup
        File.Delete(templatePath);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle empty string values")]
    public async Task GetTemplateAsync_WhenPlaceholderValueIsEmpty_ShouldReplaceWithEmptyString()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        const string templateContent = "<h1>Hello {{name}}!</h1>";
        var expectedContent = "<h1>Hello !</h1>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>
        {
            ["name"] = ""
        };

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(expectedContent);
        
        // Cleanup
        File.Delete(templatePath);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle large template content")]
    public async Task GetTemplateAsync_WhenTemplateIsLarge_ShouldProcessSuccessfully()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        var largeParagraph = string.Join("", Enumerable.Repeat(_faker.Lorem.Paragraphs(10), 10));
        const string templateContent = "<div>{{content}}</div>";
        var expectedContent = $"<div>{largeParagraph}</div>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>
        {
            ["content"] = largeParagraph
        };

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(expectedContent);
        result.Length.Should().BeGreaterThan(1000);
        
        // Cleanup
        File.Delete(templatePath);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle numeric values in placeholders")]
    public async Task GetTemplateAsync_WhenPlaceholderIsNumeric_ShouldReplaceCorrectly()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        var age = _faker.Random.Int(18, 100);
        var price = _faker.Random.Decimal(10, 1000);
        const string templateContent = "<p>Age: {{age}}, Price: {{price}}</p>";
        var expectedContent = $"<p>Age: {age}, Price: {price}</p>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>
        {
            ["age"] = age,
            ["price"] = price
        };

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(expectedContent);
        
        // Cleanup
        File.Delete(templatePath);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should handle null values in placeholders")]
    public async Task GetTemplateAsync_WhenPlaceholderValueIsNull_ShouldReplaceWithEmptyString()
    {
        // Arrange
        var templateName = $"{_faker.Random.Guid()}.html";
        const string templateContent = "<h1>Hello {{name}}!</h1>";
        var expectedContent = "<h1>Hello !</h1>";
        var templatePath = Path.Combine(_templateDirectory, templateName);
        await File.WriteAllTextAsync(templatePath, templateContent);

        var data = new Dictionary<string, object?>
        {
            ["name"] = null
        };

        // Act
        var result = await _sut.GetTemplateAsync(templateName, data);

        // Assert
        result.Should().Be(expectedContent);
        
        // Cleanup
        File.Delete(templatePath);
    }
}