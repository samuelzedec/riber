using Riber.Domain.Constants.Messages.ValueObjects;
using Riber.Domain.ValueObjects.Money.Exceptions;

namespace Riber.Domain.ValueObjects.Money;

public sealed record Money : BaseValueObject
{
    #region Properties

    public decimal Value { get; private init; }
    public string Currency { get; private init; } = "BRL";

    #endregion

    #region Constructors

    private Money() => (Value, Currency) = (0, "BRL");

    private Money(decimal value, string currency)
        => (Value, Currency) = (value, currency);

    #endregion

    #region Factories

    public static Money Create(decimal value, string currency = "BRL")
        => value < 0
            ? throw new InvalidMoneyException(MoneyErrors.NegativeValue)
            : new Money(value, currency);

    public static Money CreatePrice(decimal value, string currency = "BRL")
        => value <= 0
            ? throw new InvalidMoneyException(MoneyErrors.ZeroValue)
            : new Money(value, currency);

    public static Money Zero() => new();

    #endregion

    #region Operators

    public static implicit operator decimal(Money money)
        => money.Value;

    public static Money operator +(Money left, Money right)
        => left.Currency != right.Currency
            ? throw new CurrencyMismatchException(MoneyErrors.Sum)
            : Create(left.Value + right.Value, left.Currency);

    public static Money operator -(Money left, Money right)
        => left.Currency != right.Currency
            ? throw new CurrencyMismatchException(MoneyErrors.Subtraction)
            : Create(left.Value - right.Value, left.Currency);

    public static Money operator *(Money money, int multiplier)
        => Create(money.Value * multiplier, money.Currency);

    public static Money operator *(Money money, decimal multiplier)
        => Create(money.Value * multiplier, money.Currency);

    #endregion

    #region Methods

    public Money Multiply(int quantity) => this * quantity;
    public Money Multiply(decimal factor) => this * factor;

    #endregion
}