﻿using System.Net;
using System.Text.Json.Serialization;
using Riber.Domain.Abstractions;

namespace Riber.Application.Common;

public class Result
{
    #region Properties

    [JsonIgnore] public HttpStatusCode StatusCode { get; }
    [JsonInclude] public bool IsSuccess { get; init; }
    [JsonInclude] public Error Error { get; init; } = new();

    #endregion

    #region Constructors

    [JsonConstructor]
    protected Result() { }

    protected Result(bool isSuccess, Error error, HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
        IsSuccess = isSuccess;
        Error = error;
    }

    #endregion

    #region Success Methods

    public static Result<EmptyResult> Success()
        => new(new EmptyResult(), true, new Error(), HttpStatusCode.OK);

    public static Result<T> Success<T>(
        T? value = default,
        HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(value, true, new Error(), statusCode);

    #endregion

    #region Failure Methods

    public static Result<EmptyResult> Failure(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(new EmptyResult(), false, new Error(message, statusCode), statusCode);

    public static Result<T> Failure<T>(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(default, false, new Error(message, statusCode), statusCode);

    public static Result<T> Failure<T>(
        Dictionary<string, string[]> details,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(default, false, new Error(details, statusCode), statusCode);

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

    protected internal Result(T? value, bool isSuccess, Error error, HttpStatusCode statusCode)
        : base(isSuccess, error, statusCode)
        => Value = value;

    #endregion

    #region Overrides

    public static implicit operator Result<T>(T value)
        => Success(value);

    #endregion
}