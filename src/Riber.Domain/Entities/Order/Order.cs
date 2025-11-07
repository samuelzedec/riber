using Riber.Domain.Abstractions;
using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Entities.Tenants;
using Riber.Domain.ValueObjects.RandomToken;

namespace Riber.Domain.Entities.Order;

public sealed class Order
    : TenantEntity, IAggregateRoot, IHasRandomToken
{
    #region Properties

    public RandomToken Token { get; private set; }
    public Guid AttendantId { get; private set; }
    private readonly List<OrderItem> _items = [];
    public decimal SubTotal => _items.Sum(x => x.SubTotal);
    public decimal TotalDiscounts => _items.Sum(x => x.DiscountAmount);
    public decimal TotalAmount => SubTotal - TotalDiscounts;

    #endregion

    #region Navigation Properties

    public Company.Company Company { get; private set; } = null!;
    public User.User Attendant { get; private set; } = null!;
    public IReadOnlyCollection<OrderItem> ItemsReadOnly => _items.AsReadOnly();

    #endregion

    #region Constructors

#pragma warning disable CS8618, CA1823
    private Order() : base(Guid.Empty) { }
#pragma warning restore CS8618, CA1823

    private Order(Guid companyId, Guid attendantId)
        : base(Guid.CreateVersion7())
    {
        CompanyId = companyId;
        AttendantId = attendantId;
        Token = RandomToken.Create();
    }

    #endregion

    #region Factories

    public static Order Create(Guid companyId, Guid attendantId)
        => new(companyId, attendantId);

    #endregion
}