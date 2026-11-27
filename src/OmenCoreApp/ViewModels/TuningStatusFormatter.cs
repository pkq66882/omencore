using System;
using OmenCore.Localization;
using OmenCore.Models;

namespace OmenCore.ViewModels
{
    public static class TuningStatusFormatter
    {
        /// <summary>
        /// AMD Curve Optimizer offsets go to the SMU as whole counts of 4 mV-equivalent, truncated toward
        /// zero (AmdUndervoltProvider), so -15 is applied as -12. A difference under one step is the
        /// rounding, not a failed write (GitHub #222).
        /// </summary>
        public const double AmdCoStepMv = 4.0;

        /// <summary>True when the applied offset matches the request, allowing for AMD's step rounding.</summary>
        public static bool UndervoltOffsetMatches(double appliedMv, double requestedMv, bool isAmdCpu) =>
            Math.Abs(appliedMv - requestedMv) <= 0.5 ||
            (isAmdCpu && Math.Abs(appliedMv - requestedMv) < AmdCoStepMv && Math.Abs(appliedMv) <= Math.Abs(requestedMv));

        private static bool UndervoltOffsetIsExact(double appliedMv, double requestedMv) =>
            Math.Abs(appliedMv - requestedMv) <= 0.5;

        public static string BuildUndervoltStatusText(double requestedCoreMv, double requestedCacheMv, UndervoltStatus? status, bool isAmdCpu)
        {
            if (status == null)
            {
                return Strings.TuningStatus_RequestedAppliedNa;
            }

            var requested = isAmdCpu
                ? $"{Strings.TuningStatus_RequestedPrefix}" + string.Format(Strings.Tuning_CpuLineAmdFormat, $"{requestedCoreMv:+0;-0;0}", $"{requestedCacheMv:+0;-0;0}")
                : $"{Strings.TuningStatus_RequestedPrefix}" + string.Format(Strings.Tuning_CpuLineIntelFormat, $"{requestedCoreMv:+0;-0;0}", $"{requestedCacheMv:+0;-0;0}");

            var applied = isAmdCpu
                ? $"{Strings.TuningStatus_AppliedPrefix}" + string.Format(Strings.Tuning_CpuLineAmdFormat, $"{status.CurrentCoreOffsetMv:+0;-0;0}", $"{status.CurrentCacheOffsetMv:+0;-0;0}")
                : $"{Strings.TuningStatus_AppliedPrefix}" + string.Format(Strings.Tuning_CpuLineIntelFormat, $"{status.CurrentCoreOffsetMv:+0;-0;0}", $"{status.CurrentCacheOffsetMv:+0;-0;0}");

            string verified;
            if (status.HasExternalController)
            {
                verified = string.Format(Strings.TuningStatus_VerifiedBlocked, status.ExternalController);
            }
            else if (!string.IsNullOrWhiteSpace(status.Error))
            {
                verified = string.Format(Strings.TuningStatus_VerifiedFailed, status.Error);
            }
            else if (!string.IsNullOrWhiteSpace(status.Warning))
            {
                verified = string.Format(Strings.TuningStatus_VerifiedWarning, status.Warning);
            }
            else
            {
                var coreMatch = UndervoltOffsetMatches(status.CurrentCoreOffsetMv, requestedCoreMv, isAmdCpu);
                var cacheMatch = UndervoltOffsetMatches(status.CurrentCacheOffsetMv, requestedCacheMv, isAmdCpu);
                var rounded = !UndervoltOffsetIsExact(status.CurrentCoreOffsetMv, requestedCoreMv) ||
                              !UndervoltOffsetIsExact(status.CurrentCacheOffsetMv, requestedCacheMv);
                verified = coreMatch && cacheMatch
                    ? (rounded
                        ? string.Format(Strings.TuningStatus_VerifiedNearestStep, AmdCoStepMv)
                        : status.HasIndependentReadback
                        ? Strings.TuningStatus_VerifiedReadback
                        : Strings.TuningStatus_VerifiedAcknowledged)
                    : status.ControlledByOmenCore
                        ? Strings.TuningStatus_VerifiedMismatch
                        : Strings.TuningStatus_VerifiedNotControlled;
            }

            return $"{requested} | {applied} | {verified}";
        }

        public static string BuildGpuOcStatusText(
            int requestedCore,
            int requestedMemory,
            int requestedPower,
            int requestedVoltage,
            int appliedCore,
            int appliedMemory,
            int appliedPower,
            int appliedVoltage,
            bool backendAvailable,
            bool allWritesSucceeded)
        {
            if (!backendAvailable)
            {
                return Strings.TuningStatus_RequestedAppliedGpuNa;
            }

            var requested = $"{Strings.TuningStatus_RequestedPrefix}" + string.Format(Strings.Tuning_GpuLineFormat, FormatSigned(requestedCore, "MHz"), FormatSigned(requestedMemory, "MHz"), requestedPower, FormatSigned(requestedVoltage, "mV"));
            var applied = $"{Strings.TuningStatus_AppliedPrefix}" + string.Format(Strings.Tuning_GpuLineFormat, FormatSigned(appliedCore, "MHz"), FormatSigned(appliedMemory, "MHz"), appliedPower, FormatSigned(appliedVoltage, "mV"));

            string verified;
            if (!allWritesSucceeded)
            {
                verified = Strings.TuningStatus_VerifiedWritesFailed;
            }
            else if (requestedCore == appliedCore && requestedMemory == appliedMemory && requestedPower == appliedPower && requestedVoltage == appliedVoltage)
            {
                verified = Strings.TuningStatus_VerifiedReadback;
            }
            else
            {
                verified = Strings.TuningStatus_VerifiedMismatchReadback;
            }

            return $"{requested} | {applied} | {verified}";
        }

        private static string FormatSigned(int value, string unit)
        {
            return value >= 0 ? $"+{value} {unit}" : $"{value} {unit}";
        }
    }
}
