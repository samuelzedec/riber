using System.Diagnostics;

namespace Riber.Application.Configurations;

public static class DiagnosticsConfig
{
    public const string ActivitySourceName = "Riber.Application";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}