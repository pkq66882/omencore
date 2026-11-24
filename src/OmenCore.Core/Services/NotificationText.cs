using System;

namespace OmenCore.Services
{
    /// <summary>
    /// Wording used by <see cref="NotificationService"/> toasts. Core cannot reference the
    /// application's resource file, so the app layer pushes the localized strings in once at
    /// startup; the English defaults below keep Core self-contained (tests, headless runs).
    /// </summary>
    public static class NotificationText
    {
        public static string GameProfileActivatedTitle { get; set; } = "Game Profile Activated";
        public static string GameProfileAppliedFormat { get; set; } = "Applied profile: {0}";
        public static string GameProfileInfoFormat { get; set; } = "{0}: {1}";
        public static string GameClosedTitle { get; set; } = "Game Closed";
        public static string GameDefaultsRestored { get; set; } = "Restored default settings";
        public static string GameClosedInfoFormat { get; set; } = "{0}: restored default settings";

        public static string FanModeTitleFormat { get; set; } = "Fan Mode: {0}";
        public static string PerfModeTitleFormat { get; set; } = "Performance: {0}";
        public static string ChangedViaFormat { get; set; } = "Changed via {0}";

        public static string MemoryFreedFormat { get; set; } = "Freed {0}";
        public static string MemoryOpsFormat { get; set; } = "{0} cleanup operation(s) completed";

        public static string HighTemperatureTitle { get; set; } = "High Temperature Warning";
        public static string TemperatureFormat { get; set; } = "{0}: {1:F0}°C";
        public static string HighTemperatureThresholdFormat { get; set; } = "Threshold: {0:F0}°C · informational only, not a fan trigger";
        public static string HighTemperatureInfoFormat { get; set; } =
            "{0}: {1:F0}°C (threshold {2:F0}°C). This is an informational notification only - it's independent of " +
            "fan behavior and any BIOS thermal/TCC limit. Adjust or disable it under Settings → Notifications.";

        public static string ClampReturnedTitle { get; set; } = "GPU power clamp is back";
        public static string ClampReturnedBodyFormat { get; set; } = "The GPU is limited to {0:F0} W again, of its own {1:F0} W.";
        public static string ClampReturnedHint { get; set; } = "Restart the GPU from Diagnostics when nothing needs it.";
        public static string ClampReturnedInfoFormat { get; set; } =
            "Limited to {0:F0} W of {1:F0} W. The firmware told the driver about the adapter again - restarting the GPU drops it.";

        public static string CriticalTitleFormat { get; set; } = "CRITICAL: {0} Overheating!";
        public static string CriticalTemperatureFormat { get; set; } = "Temperature: {0:F0}°C";
        public static string CriticalBody { get; set; } = "Thermal throttling may occur. Consider reducing load or improving cooling.";
        public static string CriticalInfoFormat { get; set; } = "{0:F0}°C — thermal throttling may occur";

        public static string ThermalProtectionTitleFormat { get; set; } = "Thermal Protection: {0}";
        public static string ThermalProtectionBodyFormat { get; set; } = "{0:F0}°C - Fans boosted to max";
        public static string ThermalProtectionInfoFormat { get; set; } = "{0:F0}°C — fans boosted to max";

        /// <summary>Formats with a fallback for a null/blank template.</summary>
        internal static string Format(string? template, string fallback, params object?[] args)
        {
            var text = string.IsNullOrWhiteSpace(template) ? fallback : template!;
            return string.Format(text, args);
        }
    }
}
