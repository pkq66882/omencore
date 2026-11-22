using FluentAssertions;
using OmenCore.Utils;
using Xunit;

namespace OmenCoreApp.Tests.Utils
{
    /// <summary>GitHub #142: a profile set to Extended showed Medium in the tray because the menu had no Extended item.</summary>
    public class TrayGpuPowerHeaderTests : IDisposable
    {
        private readonly System.Globalization.CultureInfo _original = System.Globalization.CultureInfo.CurrentUICulture;

        public TrayGpuPowerHeaderTests()
        {
            // Header text comes from the localization catalogs; pin to en so English assertions hold.
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en");
        }

        public void Dispose()
        {
            System.Globalization.CultureInfo.CurrentUICulture = _original;
        }
        [Fact]
        public void ExtendedItem_IsTicked_OnlyWhenExtendedIsActive()
        {
            TrayIconService.GpuPowerExtendedHeader("Extended").Should().StartWith("✓").And.Contain("Extended");
            TrayIconService.GpuPowerExtendedHeader("Medium").Should().NotContain("✓").And.Contain("Extended");
        }
    }
}
