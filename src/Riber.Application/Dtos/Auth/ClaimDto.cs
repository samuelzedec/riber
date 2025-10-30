namespace Riber.Application.Dtos.Auth;

public sealed record ClaimDto(
    string Type,
    string Value
);