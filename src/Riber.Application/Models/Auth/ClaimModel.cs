namespace Riber.Application.Models.Auth;

public sealed record ClaimModel(
    string Type,
    string Value
);