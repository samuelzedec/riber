using Riber.Domain.Constants;
using Riber.Domain.ValueObjects.Quantity.Exceptions;

namespace Riber.Domain.ValueObjects.Quantity;

public sealed record Quantity : BaseValueObject, IEquatable<int>
{
    #region Properties

    public int Value { get; private init; }

    #endregion

    #region Constructors

    private Quantity() 
        => Value = 0;

    private Quantity(int value) 
        => Value = value;

    #endregion
    
    #region Factories

    public static Quantity Create(int value)
        => value <= 0 
            ? throw new InvalidQuantityException(ErrorMessage.Invalid.Quantity)
            : new Quantity(value);

    public static Quantity One() 
        => new(1);
    
    public static Quantity Zero() 
        => new();

    #endregion
    
    #region Operators

    public static implicit operator int(Quantity quantity) => quantity.Value;

    public static Quantity operator +(Quantity left, Quantity right)
        => Create(left.Value + right.Value);

    public static Quantity operator -(Quantity left, Quantity right)
        => Create(left.Value - right.Value);

    public static Quantity operator *(Quantity quantity, int multiplier)
        => Create(quantity.Value * multiplier);

    #endregion
    
    #region Methods

    public Quantity Add(int amount) 
        => Create(Value + amount);
    
    public Quantity Subtract(int amount) 
        => Create(Value - amount);
    
    public Quantity Multiply(int factor) 
        => Create(Value * factor);
    
    public bool IsGreaterThan(int value) 
        => Value > value;
    
    public bool IsLessThan(int value) 
        => Value < value;
    
    public bool Equals(int value) 
        => Value == value;

    #endregion
}