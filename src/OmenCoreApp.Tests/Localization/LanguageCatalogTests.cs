using System.Linq;
using FluentAssertions;
using OmenCore.Localization;
using Xunit;

namespace OmenCoreApp.Tests.Localization
{
    /// <summary>
    /// R46 guard for the multilingual contract: the selectable language list must keep every
    /// upstream language (system-follow default, English, Russian, Turkish) plus Simplified
    /// Chinese, and the config codes must stay canonical English data (only the display layer
    /// is localized). If a future refactor drops a language or renames a code, upgrade installs
    /// would strand users' settings - these tests fail first.
    /// </summary>
    public class LanguageCatalogTests
    {
        [Fact]
        public void Supported_KeepsEveryUpstreamLanguagePlusSimplifiedChinese()
        {
            var codes = Language.Supported.Select(o => o.Code).ToArray();

            codes.Should().Equal("auto", "en", "ru", "tr", "zh");
            Language.Supported.Single(o => o.Code == "auto").Name.Should().Be("System default");
            Language.Supported.Single(o => o.Code == "en").Name.Should().Be("English");
            Language.Supported.Single(o => o.Code == "ru").Name.Should().Be("Русский");
            Language.Supported.Single(o => o.Code == "tr").Name.Should().Be("Türkçe");
            Language.Supported.Single(o => o.Code == "zh").Name.Should().Be("简体中文");
        }

        [Fact]
        public void Normalize_AcceptsInstallerSeedCodesAndClampsUnknownsToSystemDefault()
        {
            foreach (var code in new[] { "en", "ru", "tr", "zh" })
            {
                Language.Normalize(code).Should().Be(code);
                Language.Normalize(code.ToUpperInvariant()).Should().Be(code);
                Language.Normalize(" " + code + " ").Should().Be(code);
            }

            Language.Normalize(null).Should().Be(Language.SystemDefault);
            Language.Normalize("").Should().Be(Language.SystemDefault);
            Language.Normalize("de").Should().Be(Language.SystemDefault);
            Language.Normalize("klingon").Should().Be(Language.SystemDefault);
        }
    }
}
