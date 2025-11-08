using Riber.Domain.Enums;

namespace Riber.Infrastructure.Extensions;

public static class AssistantTypeExtension
{
    public static string GetModel(this AssistantType assistantType)
        => assistantType switch
        {
            AssistantType.SalesAnalysis or AssistantType.MonthlyReport => "claude-sonnet-4-20250514",
            AssistantType.ProductAnalysis => "claude-haiku-4-5-20251001",
            _ => throw new ArgumentOutOfRangeException(nameof(assistantType), assistantType, null)
        };
}