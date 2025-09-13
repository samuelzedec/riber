using Riber.Domain.Constants;
using Riber.Domain.ValueObjects.Discount.Exceptions;

namespace Riber.Domain.ValueObjects.Discount;

public sealed record Discount : BaseValueObject
{
    #region Properties

    public decimal Percentage { get; private init; }
    public decimal FixedAmount { get; private init; }
    public string? Reason { get; private init; }

    #endregion

    #region Constructors

    private Discount() => (Percentage, FixedAmount, Reason) = (0, 0, null);

    private Discount(decimal percentage, decimal fixedAmount, string? reason)
        => (Percentage, FixedAmount, Reason) = (percentage, fixedAmount, reason);

    #endregion
    
    #region Factories

    public static Discount None() => new();

    public static Discount CreateByPercentage(decimal percentage, string? reason = null)
        => percentage is <= 0 or > 100 
            ? throw new InvalidPercentageException(ErrorMessage.Discount.PercentageIsInvalid)
            : new Discount(percentage, 0, reason);

    public static Discount CreateByFixedAmount(decimal fixedAmount, string? reason = null)
        => fixedAmount <= 0 
            ? throw new InvalidFixedAmountException(ErrorMessage.Discount.FixedAmountIsInvalid)
            : new Discount(0, fixedAmount, reason);

    #endregion
    
    #region Methods

    public decimal CalculateDiscount(decimal subTotal)
    {
        if (subTotal <= 0) 
            return 0;
        
        return Percentage > 0
            ? subTotal * Percentage / 100
            : Math.Min(FixedAmount, subTotal);
    }

    public bool HasDiscount() 
        => Percentage > 0 || FixedAmount > 0;
   
    public bool IsPercentage() 
        => Percentage > 0;
    
    public bool IsFixedAmount() 
        => FixedAmount > 0;
    
    #endregion
}