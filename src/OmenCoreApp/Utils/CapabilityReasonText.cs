using System;
using System.Linq;
using OmenCore.Hardware;
using OmenCore.Localization;

namespace OmenCore.Utils
{
    /// <summary>
    /// Localizes the capability reasons produced by Core's CapabilityReasonResolver. Core keeps the
    /// canonical English sentences (they also go to logs and the diagnostics export); the UI shows
    /// the same reason localized by (feature, code) so the two never disagree about *why* a control
    /// is gated. Unknown combinations fall back to the canonical sentence.
    /// </summary>
    public static class CapabilityReasonText
    {
        /// <summary>Localized sentence for one gated feature, or empty when it is available.</summary>
        public static string Localize(DeviceCapabilities? caps, CapabilityFeature feature)
        {
            if (caps == null) return string.Empty;
            var reason = CapabilityReasonResolver.For(caps, feature);
            return reason.Available ? string.Empty : Localize(reason, caps);
        }

        public static string Localize(CapabilityReason reason, DeviceCapabilities? caps) => (reason.Feature, reason.Code) switch
        {
            (CapabilityFeature.FanControl, CapabilityReasonCode.SafetyPolicy) => Strings.CapabilityReason_FanControl_Safety,
            (CapabilityFeature.FanControl, CapabilityReasonCode.ModelDatabase) =>
                string.Format(Strings.CapabilityReason_FanControl_ModelDbFormat, ModelName(caps)),
            (CapabilityFeature.FanControl, CapabilityReasonCode.DriverMissing) => MissingBackend(caps),
            (CapabilityFeature.FanControl, CapabilityReasonCode.NotPresent) => Strings.CapabilityReason_FanControl_NotPresent,

            (CapabilityFeature.FanCurves, CapabilityReasonCode.SafetyPolicy) => Strings.CapabilityReason_FanCurves_Safety,
            (CapabilityFeature.FanCurves, CapabilityReasonCode.ModelDatabase) => Strings.CapabilityReason_FanCurves_ModelDb,
            (CapabilityFeature.FanCurves, CapabilityReasonCode.NotReady) =>
                string.Format(Strings.CapabilityReason_FanCurves_NotReadyFormat, MissingBackend(caps)),
            (CapabilityFeature.FanCurves, CapabilityReasonCode.NotPresent) => Strings.CapabilityReason_FanCurves_NotPresent,

            (CapabilityFeature.IndependentFanCurves, CapabilityReasonCode.NotReady) => Strings.CapabilityReason_Independent_NotReady,
            (CapabilityFeature.IndependentFanCurves, CapabilityReasonCode.ModelDatabase) => Strings.CapabilityReason_Independent_ModelDb,
            (CapabilityFeature.IndependentFanCurves, CapabilityReasonCode.NotPresent) => Strings.CapabilityReason_Independent_NotPresent,

            (CapabilityFeature.GpuPowerBoost, CapabilityReasonCode.ModelDatabase) => Strings.CapabilityReason_GpuBoost_ModelDb,
            (CapabilityFeature.GpuPowerBoost, CapabilityReasonCode.DriverMissing) => Strings.CapabilityReason_GpuBoost_DriverMissing,
            (CapabilityFeature.GpuPowerBoost, CapabilityReasonCode.NotPresent) => Strings.CapabilityReason_GpuBoost_NotPresent,

            (CapabilityFeature.MuxSwitch, CapabilityReasonCode.ModelDatabase) => Strings.CapabilityReason_Mux_ModelDb,
            (CapabilityFeature.MuxSwitch, CapabilityReasonCode.NotPresent) => Strings.CapabilityReason_Mux_NotPresent,

            (CapabilityFeature.Lighting, CapabilityReasonCode.ModelDatabase) => Strings.CapabilityReason_Lighting_ModelDb,
            (CapabilityFeature.Lighting, CapabilityReasonCode.NotPresent) => Strings.CapabilityReason_Lighting_NotPresent,

            (CapabilityFeature.PerKeyLighting, CapabilityReasonCode.ModelDatabase) => Strings.CapabilityReason_PerKey_ModelDb,
            (CapabilityFeature.PerKeyLighting, CapabilityReasonCode.NotPresent) => Strings.CapabilityReason_PerKey_NotPresent,

            (CapabilityFeature.Undervolt, CapabilityReasonCode.NotReady) => LocalizeUndervoltBlock(reason.Message),
            (CapabilityFeature.Undervolt, CapabilityReasonCode.ModelDatabase) => Strings.CapabilityReason_Undervolt_ModelDb,
            (CapabilityFeature.Undervolt, CapabilityReasonCode.DriverMissing) => Strings.CapabilityReason_Undervolt_DriverMissing,
            (CapabilityFeature.Undervolt, CapabilityReasonCode.NotPresent) => Strings.CapabilityReason_Undervolt_NotPresent,

            (CapabilityFeature.PerformanceModes, CapabilityReasonCode.DriverMissing) => Strings.CapabilityReason_PerfModes_DriverMissing,

            _ => reason.Message
        };

        /// <summary>One localized line per unavailable feature; empty when nothing is gated.</summary>
        public static string Summary(DeviceCapabilities? caps)
        {
            if (caps == null) return string.Empty;
            var unavailable = CapabilityReasonResolver.Resolve(caps).Where(r => !r.Available).ToList();
            return string.Join("; ", unavailable.Select(r => $"{r.Feature}: {Localize(r, caps)}"));
        }

        private static string ModelName(DeviceCapabilities? caps)
        {
            var name = caps?.ModelConfig?.ModelName;
            return string.IsNullOrWhiteSpace(name) ? Strings.CapabilityReason_ThisBoard : name!;
        }

        private static string MissingBackend(DeviceCapabilities? caps)
        {
            if (caps == null) return Strings.CapabilityReason_Backend_None;
            if (caps.SecureBootEnabled && !caps.PawnIOAvailable)
                return Strings.CapabilityReason_Backend_SecureBoot;
            if (caps.OghInstalled && !caps.OghRunning)
                return Strings.CapabilityReason_Backend_OghNotRunning;
            return Strings.CapabilityReason_Backend_None;
        }

        private static string LocalizeUndervoltBlock(string message)
        {
            if (string.Equals(message, "Undervolt probe timed out", StringComparison.Ordinal))
                return Strings.CapabilityReason_Undervolt_Timeout;
            if (string.Equals(message, "Unknown block reason", StringComparison.Ordinal))
                return Strings.CapabilityReason_Undervolt_UnknownBlock;
            const string modelDbPrefix = "Undervolt disabled per model database (not supported on ";
            if (message.StartsWith(modelDbPrefix, StringComparison.Ordinal))
            {
                var model = message.Substring(modelDbPrefix.Length).TrimEnd(')');
                return string.Format(Strings.CapabilityReason_Undervolt_ModelDbDisabledFormat, model);
            }
            return string.Format(Strings.CapabilityReason_Undervolt_NotReadyFormat, message);
        }
    }
}
