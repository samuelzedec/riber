using FluentAssertions;
using Riber.Domain.Entities;

namespace Riber.Domain.Tests.Entities;

public sealed class OrderTests : BaseTest
{
    #region Creation Tests

    [Fact(DisplayName = "Should create order successfully with valid data")]
    public void Create_WhenValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var attendantId = Guid.NewGuid();

        // Act
        var result = Order.Create(companyId, attendantId);

        // Assert
        result.Should().NotBeNull();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Id.Should().NotBeEmpty();
        result.CompanyId.Should().Be(companyId);
        result.AttendantId.Should().Be(attendantId);
        result.Token.Should().NotBeNull();
        result.Token.Value.Should().NotBeNullOrEmpty();
        result.SubTotal.Should().Be(0);
        result.TotalDiscounts.Should().Be(0);
        result.TotalAmount.Should().Be(0);
        result.ItemsReadOnly.Should().BeEmpty();
    }

    [Fact(DisplayName = "Should create order with unique token")]
    public void Create_WhenCreatingMultipleOrders_ShouldHaveUniqueTokens()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var attendantId = Guid.NewGuid();

        // Act
        var order1 = Order.Create(companyId, attendantId);
        var order2 = Order.Create(companyId, attendantId);

        // Assert
        order1.Token.Value.Should().NotBe(order2.Token.Value);
    }

    [Fact(DisplayName = "Should create order with unique id")]
    public void Create_WhenCreatingMultipleOrders_ShouldHaveUniqueIds()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var attendantId = Guid.NewGuid();

        // Act
        var order1 = Order.Create(companyId, attendantId);
        var order2 = Order.Create(companyId, attendantId);

        // Assert
        order1.Id.Should().NotBe(order2.Id);
    }

    [Fact(DisplayName = "Should create order with empty items collection")]
    public void Create_WhenCreated_ShouldHaveEmptyItemsCollection()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var attendantId = Guid.NewGuid();

        // Act
        var result = Order.Create(companyId, attendantId);

        // Assert
        result.ItemsReadOnly.Should().NotBeNull();
        result.ItemsReadOnly.Should().BeEmpty();
        result.ItemsReadOnly.Count.Should().Be(0);
    }

    [Fact(DisplayName = "Should create order with readonly items collection")]
    public void Create_WhenCreated_ShouldHaveReadOnlyItemsCollection()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var attendantId = Guid.NewGuid();

        // Act
        var result = Order.Create(companyId, attendantId);

        // Assert
        result.ItemsReadOnly.Should().BeAssignableTo<IReadOnlyCollection<OrderItem>>();
    }

    #endregion

    #region Calculated Properties Tests

    [Fact(DisplayName = "Should calculate subtotal as zero when no items")]
    public void SubTotal_WhenNoItems_ShouldReturnZero()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = order.SubTotal;

        // Assert
        result.Should().Be(0);
    }

    [Fact(DisplayName = "Should calculate total discounts as zero when no items")]
    public void TotalDiscounts_WhenNoItems_ShouldReturnZero()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = order.TotalDiscounts;

        // Assert
        result.Should().Be(0);
    }

    [Fact(DisplayName = "Should calculate total amount as zero when no items")]
    public void TotalAmount_WhenNoItems_ShouldReturnZero()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = order.TotalAmount;

        // Assert
        result.Should().Be(0);
    }

    [Fact(DisplayName = "Should calculate total amount as subtotal minus discounts")]
    public void TotalAmount_Always_ShouldEqualSubTotalMinusDiscounts()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var totalAmount = order.TotalAmount;
        var expectedAmount = order.SubTotal - order.TotalDiscounts;

        // Assert
        totalAmount.Should().Be(expectedAmount);
    }

    #endregion

    #region Token Tests

    [Fact(DisplayName = "Should have valid random token after creation")]
    public void Token_WhenOrderCreated_ShouldHaveValidRandomToken()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var attendantId = Guid.NewGuid();

        // Act
        var result = Order.Create(companyId, attendantId);

        // Assert
        result.Token.Should().NotBeNull();
        result.Token.Value.Should().NotBeNullOrEmpty();
        result.Token.Value.Length.Should().BeGreaterThan(0);
    }

    [Fact(DisplayName = "Should generate different tokens for different orders")]
    public void Token_WhenCreatingMultipleOrders_ShouldGenerateDifferentTokens()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var attendantId = Guid.NewGuid();
        var orders = new List<Order>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            orders.Add(Order.Create(companyId, attendantId));
        }

        // Assert
        var tokens = orders.Select(o => o.Token.Value).ToList();
        tokens.Should().OnlyHaveUniqueItems();
    }

    #endregion

    #region Navigation Properties Tests

    [Fact(DisplayName = "Should have company navigation property")]
    public void Company_WhenAccessed_ShouldHaveNavigationProperty()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        // Navigation property exists and can be accessed
        var company = order.Company;
        company.Should().BeNull(); // Will be null until loaded by EF
    }

    [Fact(DisplayName = "Should have attendant navigation property")]
    public void Attendant_WhenAccessed_ShouldHaveNavigationProperty()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        // Navigation property exists and can be accessed
        var attendant = order.Attendant;
        attendant.Should().BeNull(); // Will be null until loaded by EF
    }

    #endregion

    #region Edge Cases Tests

    [Fact(DisplayName = "Should handle empty guid for company id")]
    public void Create_WhenEmptyCompanyId_ShouldCreateWithEmptyGuid()
    {
        // Arrange
        var companyId = Guid.Empty;
        var attendantId = Guid.NewGuid();

        // Act
        var result = Order.Create(companyId, attendantId);

        // Assert
        result.CompanyId.Should().Be(Guid.Empty);
        result.Should().NotBeNull();
    }

    [Fact(DisplayName = "Should handle empty guid for attendant id")]
    public void Create_WhenEmptyAttendantId_ShouldCreateWithEmptyGuid()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var attendantId = Guid.Empty;

        // Act
        var result = Order.Create(companyId, attendantId);

        // Assert
        result.AttendantId.Should().Be(Guid.Empty);
        result.Should().NotBeNull();
    }

    [Fact(DisplayName = "Should handle both empty guids")]
    public void Create_WhenBothIdsEmpty_ShouldCreateWithEmptyGuids()
    {
        // Arrange
        var companyId = Guid.Empty;
        var attendantId = Guid.Empty;

        // Act
        var result = Order.Create(companyId, attendantId);

        // Assert
        result.CompanyId.Should().Be(Guid.Empty);
        result.AttendantId.Should().Be(Guid.Empty);
        result.Should().NotBeNull();
        result.Token.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    #endregion
}