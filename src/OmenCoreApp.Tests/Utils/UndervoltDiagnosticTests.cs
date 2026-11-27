using FluentAssertions;
using OmenCore.Hardware;
using OmenCore.Utils;
using Xunit;

namespace OmenCoreApp.Tests.Utils
{
    /// <summary>
    /// The undervolt providers report diagnostics as canonical English sentences; the UI must translate
    /// them before placing them inside Chinese templates. These tests pin the mapping for both the fixed
    /// sentences and the ones with dynamic parts (CPU name, AMD family, SMU status).
    /// </summary>
    public class UndervoltDiagnosticTests
    {
        [Fact]
        public void FixedSentence_IsTranslated()
        {
            var original = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh");
            try
            {
                const string english = "MSR access backend is unavailable. Install PawnIO (Secure Boot compatible) and reboot to enable CPU undervolting.";

                var text = DisplayNames.UndervoltDiagnostic(english);

                text.Should().NotBe(english);
                text.Should().Contain("PawnIO");
                text.Should().NotContain("backend is unavailable");
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = original;
            }
        }

        [Fact]
        public void DynamicCpuNameSentence_KeepsTheCpuName()
        {
            var original = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh");
            try
            {
                const string english = "CPU AMD Ryzen 7 8845HS does not expose a supported Curve Optimizer control path on this firmware.";

                var text = DisplayNames.UndervoltDiagnostic(english);

                text.Should().Contain("AMD Ryzen 7 8845HS");
                text.Should().NotContain("does not expose");
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = original;
            }
        }

        [Fact]
        public void ConcatenatedWarnings_AreBothTranslated()
        {
            var original = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh");
            try
            {
                const string english =
                    "Cannot verify applied voltage offsets because the MSR backend is unavailable. Showing last requested values. " +
                    "Per-core voltage offsets were requested but were not written - this backend only applies the global Core/Cache offsets.";

                var text = DisplayNames.UndervoltDiagnostic(english);

                text.Should().NotContain("Cannot verify applied voltage offsets");
                text.Should().NotContain("Per-core voltage offsets were requested");
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = original;
            }
        }

        [Fact]
        public void CapabilityReasonText_UsesTheLocalizedBlockReason()
        {
            // This is the path the tuning card actually binds to (UndervoltReason), not the removed
            // UndervoltNotSupportedReason property. The probe's English sentence must not survive here.
            var original = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh");
            try
            {
                var caps = new DeviceCapabilities
                {
                    UndervoltBlockReason = "AMD SMU backend is unavailable. Install PawnIO, run OmenCore as administrator, and reboot if the driver was just installed."
                };

                var text = CapabilityReasonText.Localize(caps, CapabilityFeature.Undervolt);

                text.Should().Contain("降压当前不可用");
                text.Should().Contain("PawnIO");
                text.Should().NotContain("backend is unavailable");
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = original;
            }
        }

        [Fact]
        public void UnknownText_PassesThrough()
        {
            const string other = "Something the mapper has never seen.";

            DisplayNames.UndervoltDiagnostic(other).Should().Be(other);
        }
    }
}
