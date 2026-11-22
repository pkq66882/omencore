using System.Linq;
using OmenCore.Localization;
using OmenCore.Services.SystemOptimizer;

namespace OmenCore.ViewModels
{
    public static class SystemOptimizerPreflightFormatter
    {
        public static string BuildSummary(PreflightReport report)
        {
            var rebootTag = report.RequiresReboot ? Strings.SystemOptimizerPreflightFormatter_RebootYes : Strings.SystemOptimizerPreflightFormatter_RebootNo;
            return string.Format(Strings.SystemOptimizerPreflightFormatter_SummaryFormat, report.LowRiskCount, report.MediumRiskCount, report.HighRiskCount, rebootTag);
        }

        public static string BuildWarningRollup(PreflightReport report, int maxWarnings = 2)
        {
            if (report.Warnings.Count == 0)
            {
                return Strings.SystemOptimizerViewModel_NoPreflightWarnings;
            }

            var selected = report.Warnings.Take(maxWarnings).ToList();
            var suffix = report.Warnings.Count > maxWarnings ? string.Format(Strings.SystemOptimizerPreflightFormatter_MoreWarnings, report.Warnings.Count - maxWarnings) : string.Empty;
            return string.Join(" | ", selected.Select(MapWarning)) + suffix;
        }

        private static string MapWarning(string warning) => warning switch
        {
            "Increases idle power draw; not recommended on battery." => Strings.Optimizer_WarningIdlePower,
            "May slow app-launch times on HDDs; beneficial on SSDs." => Strings.Optimizer_WarningHddAppLaunch,
            "Windows Search will be slower after this change." => Strings.Optimizer_WarningSearchSlower,
            "Some HDR and variable-refresh features may not work in affected apps." => Strings.Optimizer_WarningHdrVrr,
            "Some legacy 16-bit or very old programs may fail to launch after this change." => Strings.Optimizer_WarningLegacy16,
            "Backup tools that rely on last-access timestamps may not detect unchanged files correctly." => Strings.Optimizer_WarningBackupTimestamps,
            _ => warning
        };
    }
}
