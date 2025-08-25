using SnackFlow.Domain.Abstractions;
using SnackFlow.Domain.Abstractions.ValueObjects;
using SnackFlow.Domain.ValueObjects.RandomToken;

namespace SnackFlow.Domain.Entities;

public sealed class Order 
    : BaseEntity, IAggregateRoot, IHasRandomToken
{
    #region Properties

    public RandomToken Token { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid AttendantId { get; private set; }
    private readonly List<OrderItem> _items = [];
    public decimal SubTotal => _items.Sum(x => x.SubTotal);
    public decimal TotalDiscounts => _items.Sum(x => x.DiscountAmount);
    public decimal TotalAmount => SubTotal - TotalDiscounts;
    
    #endregion
    
    #region Navigation Properties
    
    public Company Company { get; private set; } = null!;
    public User Attendant { get; private set; } = null!;
    public IReadOnlyCollection<OrderItem> ItemsReadOnly => _items.AsReadOnly();
    
    #endregion
    
    #region Constructors

    private Order() : base(Guid.Empty)
    {
        Token = null!;
        CompanyId = Guid.Empty;
        AttendantId = Guid.Empty;
    }

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