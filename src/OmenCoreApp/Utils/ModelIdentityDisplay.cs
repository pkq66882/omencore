using OmenCore.Localization;
using System;
using OmenCore.Services.Diagnostics;

namespace OmenCore.Utils
{
    /// <summary>
    /// Display-layer projection for <see cref="ModelIdentityResolutionSummary"/>. The summary class itself stays a
    /// byte-for-byte upstream artifact because its Summary/WarningText/RawIdentitySummary/TraceText feed the
    /// diagnostics export; this helper renders the same structured data in the UI language for the settings page
    /// without touching the canonical strings the export (and upstream issue reports) rely on.
    /// </summary>
    public static class ModelIdentityDisplay
    {
        /// <summary>Maps Core's canonical resolution-source values to UI text.</summary>
        public static string LocalizeSource(string? source) => source switch
        {
            "No resolved capability profile" => Strings.Settings_ModelSourceNoResolvedProfile,
            "Exact ProductId" => Strings.Settings_ModelSourceExactProductId,
            "Model-name pattern" => Strings.Settings_ModelSourceModelNamePattern,
            "Family fallback" => Strings.Settings_ModelSourceFamilyFallback,
            "Default fallback" => Strings.Settings_ModelSourceDefaultFallback,
            "Runtime fallback" => Strings.Settings_ModelSourceRuntimeFallback,
            "No database match" => Strings.Settings_ModelSourceNoDatabaseMatch,
            "Ambiguous ProductId + model-name disambiguation" => Strings.Settings_ModelSourceAmbiguousProductId,
            "Model-name series match" => Strings.Settings_ModelSourceModelSeriesMatch,
            "Partial ProductId match" => Strings.Settings_ModelSourcePartialProductId,
            "Heuristic match" => Strings.Settings_ModelSourceHeuristic,
            "Unknown" => Strings.Settings_ModelSourceUnknown,
            _ => source ?? string.Empty
        };

        /// <summary>Maps Core's canonical confidence values to UI text.</summary>
        public static string LocalizeConfidence(string? confidence) => confidence switch
        {
            "Low" => Strings.SettingsViewModel_ConfidenceLow,
            "Medium" => Strings.SettingsViewModel_ConfidenceMedium,
            "High" => Strings.SettingsViewModel_ConfidenceHigh,
            _ => confidence ?? string.Empty
        };

        /// <summary>Maps "Unknown" and the generation-suffix model names; other values pass through.</summary>
        public static string LocalizeModelName(string? model) => string.IsNullOrWhiteSpace(model) || model == "Unknown"
            ? Strings.SettingsViewModel_ValueUnknown
            : DisplayNames.ModelName(model);

        /// <summary>
        /// Unified model display: translated name without the ProductId suffix. The product id has its own labeled
        /// fields (diagnostics page "产品 ID" row, settings raw-identity line), so the composite
        /// "OMEN (2025 generation) (8F1C)" shows as「OMEN（2025 款）」everywhere. The tail is only stripped when it
        /// exactly matches " ({productId})", so genuine name suffixes like " (2024)" survive.
        /// </summary>
        public static string LocalizeResolvedModel(string? resolvedModel, string? productId)
        {
            if (string.IsNullOrWhiteSpace(resolvedModel) || resolvedModel == "Unknown")
            {
                return Strings.SettingsViewModel_ValueUnknown;
            }

            var tail = $" ({productId})";
            if (!string.IsNullOrWhiteSpace(productId) &&
                resolvedModel.EndsWith(tail, StringComparison.OrdinalIgnoreCase))
            {
                return DisplayNames.ModelName(resolvedModel[..^tail.Length].TrimEnd());
            }

            return DisplayNames.ModelName(resolvedModel);
        }

        private const string BrandingSentenceEn = "Model name contains OMEN/Victus branding, but exact model is not in database.";

        private static string LocalizeCapabilitySentence(string? sentence) => sentence switch
        {
            "Capability detection did not resolve a model profile." => Strings.SettingsViewModel_WarnCapabilityNoProfile,
            "Capability profile matched by ProductId, but the entry is not user-verified yet." => Strings.SettingsViewModel_WarnCapabilityUnverified,
            "Capability profile used the WMI model name to disambiguate a shared ProductId." => Strings.SettingsViewModel_WarnCapabilityAmbiguousProductId,
            "Capability profile was inferred from the WMI model name instead of an exact ProductId entry." => Strings.SettingsViewModel_WarnCapabilityModelNameInferred,
            "No exact model entry matched; capability defaults were inferred from the broader model family." => Strings.Settings_WarnNoExactModelFamily,
            "HP system detected, but no validated OMEN/Victus model mapping was found; capability defaults were inferred from a broad family match." => Strings.Settings_WarnHpNoValidatedMapping,
            "OMEN/Victus hints were detected, but the specific model could not be verified; capability defaults were inferred from a broad family match." => Strings.Settings_WarnOmenHintsUnverified,
            "No exact model or family entry matched; default capabilities are in use." => Strings.SettingsViewModel_WarnNoModelMatch,
            "HP system detected, but no validated OMEN/Victus mapping was found; generic default capabilities are in use." => Strings.Settings_WarnHpNoValidatedMappingDefaults,
            "OMEN/Victus hints were detected, but model identity confidence is low; generic default capabilities are in use." => Strings.Settings_WarnOmenHintsLowConfidence,
            "Capability profile came from a runtime fallback path and should be treated as provisional." => Strings.Settings_WarnRuntimeFallbackProvisional,
            "HP system detected, but capability profile came from a runtime fallback path; treat this mapping as provisional." => Strings.Settings_WarnHpRuntimeFallbackProvisional,
            "OMEN/Victus hints were detected, but capability profile came from a runtime fallback path; treat this mapping as provisional." => Strings.Settings_WarnOmenRuntimeFallbackProvisional,
            _ => sentence ?? string.Empty
        };

        /// <summary>
        /// Localizes the canonical capability warning. Fallback warnings may carry the OMEN/Victus branding
        /// sentence as a suffix; split it off, translate the base sentence and re-attach the localized suffix.
        /// </summary>
        public static string LocalizeCapabilityWarning(string? canonical)
        {
            if (string.IsNullOrWhiteSpace(canonical))
            {
                return string.Empty;
            }

            if (canonical.EndsWith(BrandingSentenceEn, StringComparison.Ordinal))
            {
                var baseSentence = canonical[..^BrandingSentenceEn.Length].TrimEnd();
                return string.Join(" ", LocalizeCapabilitySentence(baseSentence), Strings.Settings_WarnOmenBrandingExactModelMissing);
            }

            return LocalizeCapabilitySentence(canonical);
        }

        /// <summary>Localizes the canonical keyboard warning sentences.</summary>
        public static string LocalizeKeyboardWarning(string? canonical) => canonical switch
        {
            "Keyboard profile could not be matched from the current ProductId or WMI model." => Strings.SettingsViewModel_WarnKeyboardNoMatch,
            "Keyboard ProductId is shared across models; the profile was selected using the WMI model context." => Strings.SettingsViewModel_WarnKeyboardSharedProductId,
            "Keyboard profile matched by ProductId, but the entry is not user-verified yet." => Strings.SettingsViewModel_WarnKeyboardUnverified,
            "Keyboard profile was inferred from the model series instead of an exact ProductId match." => Strings.SettingsViewModel_WarnKeyboardSeriesInferred,
            "Keyboard profile was selected from a partial ProductId match and should be treated as provisional." => Strings.SettingsViewModel_WarnKeyboardPartialProvisional,
            "Keyboard profile came from a heuristic fallback path and should be validated before relying on it." => Strings.SettingsViewModel_WarnKeyboardHeuristic,
            _ => canonical ?? string.Empty
        };

        /// <summary>Renders the short identity summary in the UI language from structured fields.</summary>
        public static string BuildDisplaySummary(ModelIdentityResolutionSummary summary)
        {
            var platformQualifier = summary.IsHpSystem
                ? (summary.HasOmenVictusHint ? Strings.SettingsViewModel_PlatformHpOmen : Strings.SettingsViewModel_PlatformHp)
                : Strings.SettingsViewModel_PlatformNonHp;

            return string.Format(Strings.Settings_ModelSummaryFormat,
                LocalizeResolvedModel(summary.ResolvedModel, summary.CapabilityProductId), LocalizeSource(summary.ResolutionSource), LocalizeConfidence(summary.Confidence), platformQualifier,
                LocalizeModelName(summary.KeyboardModel), LocalizeSource(summary.KeyboardResolutionSource), LocalizeConfidence(summary.KeyboardConfidence));
        }

        /// <summary>Renders the raw-identity line in the UI language; raw values stay untouched.</summary>
        public static string BuildDisplayRawIdentity(ModelIdentityResolutionSummary summary)
        {
            return string.Format(Strings.Settings_ModelRawIdentityFormat,
                summary.RawWmiModel, summary.RawBaseboardProduct, summary.RawSystemSku, summary.RawSystemProductIdentifyingNumber, summary.HpSupportProductNumber);
        }
    }
}
