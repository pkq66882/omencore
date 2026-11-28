using FluentAssertions;
using OmenCore.Utils;
using Xunit;

namespace OmenCoreApp.Tests.Utils
{
    /// <summary>
    /// Round-44 display mappings: model-name generation suffixes, RGB sync effect ids and the
    /// "Not recorded" surface sentinel. These are the values that were previously concatenated
    /// into Chinese sentences untranslated.
    /// </summary>
    public class DisplayNamesR44Tests
    {
        [Fact]
        public void ModelName_TranslatesGenerationSuffix_AndKeepsProductName()
        {
            var original = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh");
            try
            {
                DisplayNames.ModelName("OMEN (2025 generation)").Should().Be("OMEN（2025 款）");
                DisplayNames.ModelName("OMEN 15/17 (2017-2018 generation)").Should().Be("OMEN 15/17（2017–2018 款）");
                DisplayNames.ModelName("OMEN (legacy/unknown generation)").Should().Be("OMEN（早期/未知代）");
                DisplayNames.ModelName("OMEN by HP Gaming Laptop 16-xd0xxx").Should().Be("OMEN by HP Gaming Laptop 16-xd0xxx");

                // The settings page appends the ProductId: "OMEN (2025 generation) (8D3F)".
                DisplayNames.ModelName("OMEN (2025 generation) (8D3F)").Should().Be("OMEN（2025 款） (8D3F)");
                DisplayNames.ModelName("OMEN (legacy/unknown generation) (84DA)").Should().Be("OMEN（早期/未知代） (84DA)");
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = original;
            }
        }

        [Fact]
        public void ModelName_EnglishOutputIsUnchanged()
        {
            var original = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en");
            try
            {
                DisplayNames.ModelName("OMEN (2025 generation)").Should().Be("OMEN (2025 generation)");
                DisplayNames.ModelName("OMEN 15/17 (2017-2018 generation)").Should().Be("OMEN 15/17 (2017-2018 generation)");
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = original;
            }
        }

        [Fact]
        public void RgbEffectId_IsReadable()
        {
            var original = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh");
            try
            {
                DisplayNames.RgbEffectId("color:#FF0000").Should().Be("静态 #FF0000");
                DisplayNames.RgbEffectId("effect:wave").Should().Be("波浪");
                DisplayNames.RgbEffectId("effect:breathing").Should().Be("呼吸");
                DisplayNames.RgbEffectId("off").Should().Be("熄灯");
                DisplayNames.RgbEffectId("something-unknown").Should().Be("something-unknown");
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = original;
            }
        }

        [Fact]
        public void NotRecordedSentinel_IsLocalizedInSurfacePicker()
        {
            var original = System.Globalization.CultureInfo.CurrentUICulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("zh");
            try
            {
                DisplayNames.KeyboardSurface("Not recorded").Should().Be("未记录");
                DisplayNames.SettingsToken("Not recorded", "RgbSurface").Should().Be("未记录");
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentUICulture = original;
            }
        }
    }
}
