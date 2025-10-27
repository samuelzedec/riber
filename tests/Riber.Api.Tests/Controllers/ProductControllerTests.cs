using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Riber.Api.Tests.Fixtures;
using Riber.Application.Features.Products.Commands;
using Riber.Domain.Entities;
using Xunit;

namespace Riber.Api.Tests.Controllers;

public sealed class ProductControllerTests(WebAppFixture webAppFixture, DatabaseFixture databaseFixture)
    : IntegrationTestBase(webAppFixture, databaseFixture)
{
    #region CreateProduct Tests

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should create product successfully with valid data and authentication")]
    public async Task Should_CreateProduct_WithValidDataAndAuthentication()
    {
        // Arrange
        await AuthenticateAsync();
        var categoryId = await CreateTestCategoryAsync();
        var formData = CreateProductFormData(
            name: "Test Product",
            description: "Test Description",
            price: 99.99m,
            categoryId: categoryId
        );

        // Act
        var response = await Client.PostAsync("api/v1/product", formData);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var productResponse = await ReadResultValueAsync<CreateProductCommandResponse>(response);
        productResponse.Should().NotBeNull();
        productResponse.IsSuccess.Should().BeTrue();
        productResponse.Value.Should().NotBeNull();
        productResponse.Value!.ProductId.Should().NotBeEmpty();
        productResponse.Value.Name.Should().Be("Test Product");
        productResponse.Value.Description.Should().Be("Test Description");
        productResponse.Value.Price.Should().Be(99.99m);
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should create product successfully with image")]
    public async Task Should_CreateProduct_WithImage()
    {
        // Arrange
        await AuthenticateAsync();
        var categoryId = await CreateTestCategoryAsync();
        var imageBytes = CreateFakeImageBytes();
        var formData = CreateProductFormData(
            name: "Product with Image",
            description: "Product with image description",
            price: 149.99m,
            categoryId: categoryId,
            imageBytes: imageBytes,
            imageName: "test-image.jpg"
        );

        // Act
        var response = await Client.PostAsync("api/v1/product", formData);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var productResponse = await ReadResultValueAsync<CreateProductCommandResponse>(response);
        productResponse.Should().NotBeNull();
        productResponse.IsSuccess.Should().BeTrue();
        productResponse.Value.Should().NotBeNull();
        productResponse.Value!.ProductId.Should().NotBeEmpty();
        productResponse.Value.ImageName.Should().NotBeNullOrEmpty();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should not create product with nonexistent category")]
    public async Task Should_NotCreateProduct_WithNonexistentCategory()
    {
        // Arrange
        await AuthenticateAsync();
        var nonexistentCategoryId = Guid.NewGuid();
        var formData = CreateProductFormData(
            name: "Test Product",
            description: "Test Description",
            price: 99.99m,
            categoryId: nonexistentCategoryId
        );

        // Act
        var response = await Client.PostAsync("api/v1/product", formData);

        // Assert
        var productResponse = await ReadResultValueAsync<CreateProductCommandResponse>(response);
        productResponse.Should().NotBeNull();
        productResponse.IsSuccess.Should().BeFalse();
        productResponse.Error.Should().NotBeNull();
    }

    [Trait("Category", "Integration")]
    [Fact(DisplayName = "Should validate product persisted in database")]
    public async Task Should_ValidateProductPersistedInDatabase()
    {
        // Arrange
        await AuthenticateAsync();
        var categoryId = await CreateTestCategoryAsync();
        var formData = CreateProductFormData(
            name: "Persisted Product",
            description: "This product should be in database",
            price: 99.99m,
            categoryId: categoryId
        );

        // Act
        var response = await Client.PostAsync("api/v1/product", formData);
        var productResponse = await ReadResultValueAsync<CreateProductCommandResponse>(response);

        // Assert
        var context = GetDbContext();
        var productInDb = await context.Set<Product>().FindAsync(productResponse!.Value!.ProductId);

        productInDb.Should().NotBeNull();
        productInDb.Name.Should().Be("Persisted Product");
        productInDb.Description.Should().Be("This product should be in database");
        productInDb.UnitPrice.Value.Should().Be(99.99m);
        productInDb.CategoryId.Should().Be(categoryId);
    }

    #endregion

    #region Helper Methods

    private async Task<Guid> CreateTestCategoryAsync()
    {
        var context = GetDbContext();
        var category = ProductCategory.Create(
            name: "Test Category",
            description: "Test Description",
            code: "TST001",
            companyId: Guid.Parse("ba99e7c2-f824-490d-a0fb-6dae78cc0163")
        );

        context.Set<ProductCategory>().Add(category);
        await context.SaveChangesAsync();
        return category.Id;
    }

    private static MultipartFormDataContent CreateProductFormData(
        string name,
        string description,
        decimal price,
        Guid categoryId,
        byte[]? imageBytes = null,
        string? imageName = null)
    {
        var formData = new MultipartFormDataContent
        {
            { new StringContent(name), "Name" },
            { new StringContent(description), "Description" },
            { new StringContent(price.ToString(CultureInfo.InvariantCulture)), "Price" },
            { new StringContent(categoryId.ToString()), "CategoryId" }
        };

        if (imageBytes != null && imageName != null)
        {
            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            formData.Add(imageContent, "Image", imageName);
        }

        return formData;
    }

    private static byte[] CreateFakeImageBytes()
        => [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46];

    #endregion
}