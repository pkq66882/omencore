using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using OmenCore.Hardware;
using OmenCore.Localization;
using OmenCore.Models;
using OmenCore.Services.Diagnostics;
using OmenCore.Utils;
using OmenCore.ViewModels;
using Xunit;

namespace OmenCoreApp.Tests.Utils
{
    /// <summary>
    /// R45 regression tests for the UI/export split around model identity. The summary class must keep producing
    /// byte-for-byte upstream English (it feeds the diagnostics export), while the settings UI gets localized text
    /// through <see cref="ModelIdentityDisplay"/>. Under zh UI culture the export-facing strings must contain no
    /// Chinese at all — that regression shipped once and polluted identity-resolution-trace.txt (Attention lines).
    /// </summary>
    public class ModelIdentityDisplayTests : IDisposable
    {
        private readonly CultureInfo _originalUICulture = CultureInfo.CurrentUICulture;

        public ModelIdentityDisplayTests()
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh");
        }

        public void Dispose()
        {
            CultureInfo.CurrentUICulture = _originalUICulture;
        }

        private static bool HasCjk(string? value) => !string.IsNullOrEmpty(value) && value.Any(c => c >= 0x4E00 && c <= 0x9FFF);

        public static readonly string[] CanonicalCapabilityWarnings =
        {
            "Capability detection did not resolve a model profile.",
            "Capability profile matched by ProductId, but the entry is not user-verified yet.",
            "Capability profile used the WMI model name to disambiguate a shared ProductId.",
            "Capability profile was inferred from the WMI model name instead of an exact ProductId entry.",
            "No exact model entry matched; capability defaults were inferred from the broader model family.",
            "HP system detected, but no validated OMEN/Victus model mapping was found; capability defaults were inferred from a broad family match.",
            "OMEN/Victus hints were detected, but the specific model could not be verified; capability defaults were inferred from a broad family match.",
            "No exact model or family entry matched; default capabilities are in use.",
            "HP system detected, but no validated OMEN/Victus mapping was found; generic default capabilities are in use.",
            "OMEN/Victus hints were detected, but model identity confidence is low; generic default capabilities are in use.",
            "Capability profile came from a runtime fallback path and should be treated as provisional.",
            "HP system detected, but capability profile came from a runtime fallback path; treat this mapping as provisional.",
            "OMEN/Victus hints were detected, but capability profile came from a runtime fallback path; treat this mapping as provisional."
        };

        public static readonly string[] CanonicalKeyboardWarnings =
        {
            "Keyboard profile could not be matched from the current ProductId or WMI model.",
            "Keyboard ProductId is shared across models; the profile was selected using the WMI model context.",
            "Keyboard profile matched by ProductId, but the entry is not user-verified yet.",
            "Keyboard profile was inferred from the model series instead of an exact ProductId match.",
            "Keyboard profile was selected from a partial ProductId match and should be treated as provisional.",
            "Keyboard profile came from a heuristic fallback path and should be validated before relying on it."
        };

        [Fact]
        public void LocalizeCapabilityWarning_TranslatesEveryCanonicalSentence()
        {
            foreach (var canonical in CanonicalCapabilityWarnings)
            {
                var localized = ModelIdentityDisplay.LocalizeCapabilityWarning(canonical);
                localized.Should().NotBe(canonical, $"the zh UI must translate '{canonical}'");
                HasCjk(localized).Should().BeTrue($"the zh UI must contain Chinese for '{canonical}'");
            }
        }

        [Fact]
        public void LocalizeCapabilityWarning_TranslatesBrandingComposite()
        {
            const string canonical = "OMEN/Victus hints were detected, but capability profile came from a runtime fallback path; treat this mapping as provisional. Model name contains OMEN/Victus branding, but exact model is not in database.";

            var localized = ModelIdentityDisplay.LocalizeCapabilityWarning(canonical);

            localized.Should().Contain("检测到 OMEN/Victus 特征");
            localized.Should().Contain("机型名带有 OMEN/Victus 标识");
            HasCjk(localized).Should().BeTrue();
        }

        [Fact]
        public void LocalizeKeyboardWarning_TranslatesEveryCanonicalSentence()
        {
            foreach (var canonical in CanonicalKeyboardWarnings)
            {
                var localized = ModelIdentityDisplay.LocalizeKeyboardWarning(canonical);
                localized.Should().NotBe(canonical, $"the zh UI must translate '{canonical}'");
                HasCjk(localized).Should().BeTrue($"the zh UI must contain Chinese for '{canonical}'");
            }
        }

        [Fact]
        public void LocalizeSource_CoversModelNamePatternAndRuntimeFallback()
        {
            ModelIdentityDisplay.LocalizeSource("Model-name pattern").Should().Be("机型名匹配");
            ModelIdentityDisplay.LocalizeSource("Runtime fallback").Should().Be("运行时回退");
            ModelIdentityDisplay.LocalizeSource("No database match").Should().NotBe("No database match");
        }

        [Fact]
        public void LocalizeModelName_TranslatesGenerationSuffixAndComposite()
        {
            ModelIdentityDisplay.LocalizeModelName("OMEN (2025 generation) (8F1C)").Should().Be("OMEN（2025 款） (8F1C)");
            ModelIdentityDisplay.LocalizeModelName("OMEN (2025 generation)").Should().Be("OMEN（2025 款）");
            ModelIdentityDisplay.LocalizeModelName("Unknown").Should().Be(Strings.SettingsViewModel_ValueUnknown);
        }

        [Fact]
        public void LocalizeResolvedModel_StripsProductIdTailForUnifiedDisplay()
        {
            ModelIdentityDisplay.LocalizeResolvedModel("OMEN (2025 generation) (8F1C)", "8F1C").Should().Be("OMEN（2025 款）");
            ModelIdentityDisplay.LocalizeResolvedModel("OMEN 16-wf1xxx (2024) (8C78)", "8C78").Should().Be("OMEN 16-wf1xxx (2024)");
            ModelIdentityDisplay.LocalizeResolvedModel("OMEN (2025 generation)", "8F1C").Should().Be("OMEN（2025 款）");
            ModelIdentityDisplay.LocalizeResolvedModel("Unknown", "8F1C").Should().Be(Strings.SettingsViewModel_ValueUnknown);
        }

        [Fact]
        public void BuildDisplaySummary_RendersChineseFromStructuredFields()
        {
            var summary = new ModelIdentityResolutionSummary
            {
                ResolvedModel = "OMEN (2025 generation) (8F1C)",
                CapabilityProductId = "8F1C",
                ResolutionSource = "Runtime fallback",
                Confidence = "Low",
                IsHpSystem = true,
                HasOmenVictusHint = true,
                KeyboardModel = "Unknown",
                KeyboardResolutionSource = "No database match",
                KeyboardConfidence = "Low"
            };

            var display = ModelIdentityDisplay.BuildDisplaySummary(summary);

            display.Should().Contain("OMEN（2025 款）");
            display.Should().NotContain("(8F1C)");
            display.Should().Contain("运行时回退");
            display.Should().Contain("HP 系统（检测到 OMEN/Victus）");
            display.Should().Contain("键盘：未知");
        }

        [Fact]
        public void BuildDisplayRawIdentity_KeepsRawValuesUntouched()
        {
            var summary = new ModelIdentityResolutionSummary
            {
                RawWmiModel = "HyperX OMEN Gaming Laptop 16-am2xxx",
                RawBaseboardProduct = "8F1C",
                RawSystemSku = "DN9S5PA#AB2",
                RawSystemProductIdentifyingNumber = "5CD638G2WG",
                HpSupportProductNumber = "DN9S5PA"
            };

            var display = ModelIdentityDisplay.BuildDisplayRawIdentity(summary);

            display.Should().Contain("WMI 机型：HyperX OMEN Gaming Laptop 16-am2xxx");
            display.Should().Contain("主板 ProductId：8F1C");
            display.Should().Contain("系统 SKU：DN9S5PA#AB2");
            display.Should().Contain("HP 支持产品号：DN9S5PA");
        }
    }

    /// <summary>
    /// The diagnostics export is upstream-facing data: every string <see cref="ModelIdentityResolutionService"/>
    /// produces must stay canonical English even when the UI culture is Chinese (the export header text is parsed
    /// by the author's tooling and the page tells users to "send a diagnostics export" to promote a board).
    /// </summary>
    public class ModelIdentityExportCanonicalTests : IDisposable
    {
        private readonly CultureInfo _originalUICulture = CultureInfo.CurrentUICulture;

        public ModelIdentityExportCanonicalTests()
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh");
        }

        public void Dispose()
        {
            CultureInfo.CurrentUICulture = _originalUICulture;
        }

        private static bool HasCjk(string? value) => !string.IsNullOrEmpty(value) && value.Any(c => c >= 0x4E00 && c <= 0x9FFF);

        private static (SystemInfo System, DeviceCapabilities Capabilities) BuildHyperXOmen16Am2()
        {
            var systemInfo = new SystemInfo
            {
                Manufacturer = "HP",
                Model = "HyperX OMEN Gaming Laptop 16-am2xxx",
                ProductName = "8F1C",
                SystemSku = "DN9S5PA#AB2",
                SystemProductIdentifyingNumber = "5CD638G2WG",
                BiosVersion = "F.06"
            };
            var capabilities = new DeviceCapabilities
            {
                ProductId = "8F1C",
                ModelName = systemInfo.Model,
                ModelFamily = OmenModelFamily.OMEN16,
                IsKnownModel = false,
                ModelConfig = new ModelCapabilities
                {
                    ProductId = "8F1C",
                    ModelName = "OMEN (2025 generation)",
                    UserVerified = false,
                    Notes = "Board named by other open-source HP control projects and the Linux hp-wmi driver; no OmenCore field report yet. Behaviour is the conservative family default (WMI fan-mode switching and performance profiles only) until an export confirms more. Send a diagnostics export to have this board promoted to a full entry."
                }
            };
            return (systemInfo, capabilities);
        }

        [Fact]
        public void Build_UnderZhCulture_ExportStringsStayCanonicalEnglish()
        {
            var (systemInfo, capabilities) = BuildHyperXOmen16Am2();

            var summary = ModelIdentityResolutionService.Build(systemInfo, capabilities);

            HasCjk(summary.Summary).Should().BeFalse("Summary feeds the export ResolvedModel lines");
            HasCjk(summary.WarningText).Should().BeFalse("WarningText feeds the export Attention lines");
            HasCjk(summary.RawIdentitySummary).Should().BeFalse();
            HasCjk(summary.TraceText).Should().BeFalse("TraceText is identity-resolution-trace.txt verbatim");
            HasCjk(summary.ClipboardSummary).Should().BeFalse("the copy summary is pasted into upstream issue reports");
        }

        [Fact]
        public void Build_UnderZhCulture_TraceAttentionMatchesUpstreamWording()
        {
            var (systemInfo, capabilities) = BuildHyperXOmen16Am2();

            var summary = ModelIdentityResolutionService.Build(systemInfo, capabilities);

            summary.ResolutionSource.Should().Be("Runtime fallback");
            summary.WarningText.Should().Be("OMEN/Victus hints were detected, but capability profile came from a runtime fallback path; treat this mapping as provisional. Model name contains OMEN/Victus branding, but exact model is not in database.");
            summary.KeyboardWarningText.Should().Be("Keyboard profile could not be matched from the current ProductId or WMI model.");
            summary.TraceText.Should().Contain("Effective resolved model: OMEN (2025 generation) (8F1C)");
            summary.TraceText.Should().Contain("Resolution path: Runtime fallback");
            summary.TraceText.Should().Contain("Attention: OMEN/Victus hints were detected, but capability profile came from a runtime fallback path");
            summary.TraceText.Should().Contain("Attention: Keyboard profile could not be matched from the current ProductId or WMI model.");
            summary.TraceText.Should().Contain("Profile notes: Board named by other open-source HP control projects");
        }

        [Fact]
        public void RuleDefaults_StayCanonicalEnglish()
        {
            new ScheduleRule().RuleName.Should().Be("New Rule");
            new SettingsViewModel.AutomationRuleEditorItem().Name.Should().Be("New automation rule");
        }
    }
}
