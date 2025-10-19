using System.Text.Json.Serialization;
using Riber.Domain.Abstractions;

namespace Riber.Application.Common;

public class Result
{
    #region Properties

    [JsonInclude] public bool IsSuccess { get; init; }
    [JsonInclude] public Error Error { get; init; } = new();

    #endregion

    #region Constructors

    [JsonConstructor]
    protected Result() { }

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    #endregion

    #region Static Methods

    public static Result<object> Success()
        => new(null, true, new Error());

    public static Result<T> Success<T>(T value)
        => new(value, true, new Error());

    public static Result<T> Failure<T>(Error error)
        => new(default, false, error);

    public static Result<T> Failure<T>(string type, Dictionary<string, string[]> details)
        => new(default, false, new Error(type, details));

    protected static Result<T> Create<T>(T? value) =>
        value is not null ? Success(value) : Failure<T>(new Error());

    #endregion
}

public class Result<T> : Result
{
    #region Properties

    [JsonInclude]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public T? Value { get; init; }

    #endregion

    #region Constructors

    [JsonConstructor]
    protected Result() { }

    protected internal Result(T? value, bool isSuccess, Error error) : base(isSuccess, error)
        => Value = value;

    #endregion

    #region Overrides

    public static implicit operator Result<T>(T? value)
        => Create(value);

    #endregion
}