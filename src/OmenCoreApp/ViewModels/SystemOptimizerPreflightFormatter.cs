using System.Linq;
using OmenCore.Localization;
using OmenCore.Services.SystemOptimizer;

namespace OmenCore.ViewModels
{
    public static class SystemOptimizerPreflightFormatter
    {
        public static string BuildSummary(PreflightReport report)
        {
            var rebootTag = report.RequiresReboot ? " | Reboot: Yes" : " | Reboot: No";
            return string.Format(Strings.SystemOptimizer_PreflightLine, report.LowRiskCount, report.MediumRiskCount, report.HighRiskCount, rebootTag);
        }

        public static string BuildWarningRollup(PreflightReport report, int maxWarnings = 2)
        {
            if (report.Warnings.Count == 0)
            {
                return Strings.SystemOptimizerViewModel_NoPreflightWarnings;
            }

            var selected = report.Warnings.Take(maxWarnings).ToList();
            var suffix = report.Warnings.Count > maxWarnings ? $" (+{report.Warnings.Count - maxWarnings} more)" : string.Empty;
            return string.Join(" | ", selected) + suffix;
        }
    }
}
