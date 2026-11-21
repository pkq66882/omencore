using System.Globalization;
using System.Resources;

namespace OmenCore.Localization
{
    /// <summary>
    /// Typed access to <c>Localization/Strings.resx</c> and its satellite catalogs (roadmap WP-32).
    ///
    /// Written by hand rather than generated: the SDK only produces a designer class when Visual
    /// Studio's custom tool runs, so a CLI build (which is what CI is) would otherwise leave the
    /// class missing. Each property is one line, and <c>StringsResourceTests</c> fails if a key in
    /// the .resx has no property here or the other way round - that test is what replaces the
    /// generator's guarantee.
    ///
    /// A missing key returns its own name instead of throwing or rendering nothing: the user sees
    /// something obviously unfinished on that one label, and the app still starts. The parity test
    /// means that should never reach a release.
    /// </summary>
    public static class Strings
    {
        private static readonly ResourceManager Resources =
            new(typeof(Strings).FullName!, typeof(Strings).Assembly);

        private static string Get(string key) =>
            Resources.GetString(key, CultureInfo.CurrentUICulture) ?? key;

        // Window chrome
        public static string WindowTitle => Get(nameof(WindowTitle));
        public static string WindowMinimize => Get(nameof(WindowMinimize));
        public static string WindowMaximize => Get(nameof(WindowMaximize));
        public static string WindowRestore => Get(nameof(WindowRestore));
        public static string WindowClose => Get(nameof(WindowClose));

        // Navigation rail
        public static string NavGeneral => Get(nameof(NavGeneral));
        public static string NavCustom => Get(nameof(NavCustom));
        public static string NavTuning => Get(nameof(NavTuning));
        public static string NavDiagnostics => Get(nameof(NavDiagnostics));
        public static string NavMonitoring => Get(nameof(NavMonitoring));
        public static string NavOptimizer => Get(nameof(NavOptimizer));
        public static string NavMemory => Get(nameof(NavMemory));
        public static string NavBloatware => Get(nameof(NavBloatware));
        public static string NavRgb => Get(nameof(NavRgb));
        public static string NavSettings => Get(nameof(NavSettings));
        public static string NavGames => Get(nameof(NavGames));

        // Settings page: window behaviour
        public static string SettingsWindowBehaviour => Get(nameof(SettingsWindowBehaviour));
        public static string SettingsStartWithWindows => Get(nameof(SettingsStartWithWindows));
        public static string SettingsStartWithWindowsHelp => Get(nameof(SettingsStartWithWindowsHelp));
        public static string SettingsStartMinimized => Get(nameof(SettingsStartMinimized));
        public static string SettingsStartMinimizedHelp => Get(nameof(SettingsStartMinimizedHelp));
        public static string SettingsLiteMode => Get(nameof(SettingsLiteMode));
        public static string SettingsLiteModeHelp => Get(nameof(SettingsLiteModeHelp));
        public static string SettingsHeadlessMode => Get(nameof(SettingsHeadlessMode));
        public static string SettingsHeadlessModeHelp => Get(nameof(SettingsHeadlessModeHelp));
        public static string SettingsSoftwareRendering => Get(nameof(SettingsSoftwareRendering));
        public static string SettingsSoftwareRenderingHelp => Get(nameof(SettingsSoftwareRenderingHelp));
        public static string SettingsReduceMotion => Get(nameof(SettingsReduceMotion));
        public static string SettingsReduceMotionHelp => Get(nameof(SettingsReduceMotionHelp));
        public static string SettingsLanguage => Get(nameof(SettingsLanguage));
        public static string SettingsLanguageHelp => Get(nameof(SettingsLanguageHelp));
        public static string SettingsMinimizeToTray => Get(nameof(SettingsMinimizeToTray));
        public static string SettingsMinimizeToTrayHelp => Get(nameof(SettingsMinimizeToTrayHelp));
        public static string SettingsStayOnTop => Get(nameof(SettingsStayOnTop));
        public static string SettingsStayOnTopHelp => Get(nameof(SettingsStayOnTopHelp));
        public static string SettingsTopmostBadge => Get(nameof(SettingsTopmostBadge));
        public static string SettingsActiveBadge => Get(nameof(SettingsActiveBadge));

        // Settings page: tray and interface
        public static string SettingsTrayAndUi => Get(nameof(SettingsTrayAndUi));
        public static string SettingsDisplayScale => Get(nameof(SettingsDisplayScale));
        public static string SettingsDisplayScaleHelp => Get(nameof(SettingsDisplayScaleHelp));
        public static string SettingsQuickAccess => Get(nameof(SettingsQuickAccess));
        public static string SettingsQuickAccessHelp => Get(nameof(SettingsQuickAccessHelp));
        public static string SettingsQuickPopup => Get(nameof(SettingsQuickPopup));
        public static string SettingsQuickPopupHelp => Get(nameof(SettingsQuickPopupHelp));
        public static string SettingsTrayTemperature => Get(nameof(SettingsTrayTemperature));
        public static string SettingsTrayTemperatureHelp => Get(nameof(SettingsTrayTemperatureHelp));
        public static string SettingsCorsairHidOnly => Get(nameof(SettingsCorsairHidOnly));
        public static string SettingsCorsairHidOnlyHelp => Get(nameof(SettingsCorsairHidOnlyHelp));
        public static string SettingsCorsairHidOnlyWarning => Get(nameof(SettingsCorsairHidOnlyWarning));
        public static string SettingsTelemetry => Get(nameof(SettingsTelemetry));
        public static string SettingsTelemetryHelp => Get(nameof(SettingsTelemetryHelp));
    // Auto-extracted literals from AboutWindow.xaml
    public static string AboutWindow_AboutOmencore => Get(nameof(AboutWindow_AboutOmencore));
    public static string AboutWindow_AdvancedThermalPowerAndRgbControlForHpOmen => Get(nameof(AboutWindow_AdvancedThermalPowerAndRgbControlForHpOmen));
    public static string AboutWindow_CheckForUpdates => Get(nameof(AboutWindow_CheckForUpdates));
    public static string AboutWindow_CheckGithubForNewReleases => Get(nameof(AboutWindow_CheckGithubForNewReleases));
    public static string AboutWindow_CloseEsc => Get(nameof(AboutWindow_CloseEsc));
    public static string AboutWindow_CloseThisWindowEsc => Get(nameof(AboutWindow_CloseThisWindowEsc));
    public static string AboutWindow_CpuUndervoltIntelCurveOptimizerAmd => Get(nameof(AboutWindow_CpuUndervoltIntelCurveOptimizerAmd));
    public static string AboutWindow_DriverFreeMonitoringWmiBiosNvapiPawnio => Get(nameof(AboutWindow_DriverFreeMonitoringWmiBiosNvapiPawnio));
    public static string AboutWindow_GamingLaptopAndDesktopControlCenter => Get(nameof(AboutWindow_GamingLaptopAndDesktopControlCenter));
    public static string AboutWindow_GpuDynamicBoostTgpPpab => Get(nameof(AboutWindow_GpuDynamicBoostTgpPpab));
    public static string AboutWindow_GuidedFanDiagnostics => Get(nameof(AboutWindow_GuidedFanDiagnostics));
    public static string AboutWindow_HeadlessModeTrayOnlyNoWindow => Get(nameof(AboutWindow_HeadlessModeTrayOnlyNoWindow));
    public static string AboutWindow_MemoryOptimizer => Get(nameof(AboutWindow_MemoryOptimizer));
    public static string AboutWindow_PerGameProfileAutoSwitching => Get(nameof(AboutWindow_PerGameProfileAutoSwitching));
    public static string AboutWindow_RealTimeHardwareMonitoring => Get(nameof(AboutWindow_RealTimeHardwareMonitoring));
    public static string AboutWindow_V2KeyboardEnginePerKeyEcWmiLighting => Get(nameof(AboutWindow_V2KeyboardEnginePerKeyEcWmiLighting));
    public static string AboutWindow_Version300 => Get(nameof(AboutWindow_Version300));
    public static string AboutWindow_WmiEcFanControlWithCustomCurves => Get(nameof(AboutWindow_WmiEcFanControlWithCustomCurves));

    // Auto-extracted literals from AdvancedView.xaml
    public static string AdvancedView_ApplyGpuModeSwitchRequiresRestart => Get(nameof(AdvancedView_ApplyGpuModeSwitchRequiresRestart));
    public static string AdvancedView_ApplyGpuPowerBoostLevel => Get(nameof(AdvancedView_ApplyGpuPowerBoostLevel));
    public static string AdvancedView_ApplyRequiresRestart => Get(nameof(AdvancedView_ApplyRequiresRestart));
    public static string AdvancedView_AutoPower => Get(nameof(AdvancedView_AutoPower));
    public static string AdvancedView_Balanced => Get(nameof(AdvancedView_Balanced));
    public static string AdvancedView_BalancedPerformanceModeRecommendedAutomati => Get(nameof(AdvancedView_BalancedPerformanceModeRecommendedAutomati));
    public static string AdvancedView_BoostActive => Get(nameof(AdvancedView_BoostActive));
    public static string AdvancedView_ControlGpuTotalGraphicsPowerTgpAndDynamicB => Get(nameof(AdvancedView_ControlGpuTotalGraphicsPowerTgpAndDynamicB));
    public static string AdvancedView_CurrentMode => Get(nameof(AdvancedView_CurrentMode));
    public static string AdvancedView_CurrentPowerMode => Get(nameof(AdvancedView_CurrentPowerMode));
    public static string AdvancedView_CustomFanCurve => Get(nameof(AdvancedView_CustomFanCurve));
    public static string AdvancedView_DisableHpTelemetry => Get(nameof(AdvancedView_DisableHpTelemetry));
    public static string AdvancedView_DisableHpTelemetryServices => Get(nameof(AdvancedView_DisableHpTelemetryServices));
    public static string AdvancedView_DisplayOverdrive => Get(nameof(AdvancedView_DisplayOverdrive));
    public static string AdvancedView_ExtraPower => Get(nameof(AdvancedView_ExtraPower));
    public static string AdvancedView_FirmwareDeterminedCeiling => Get(nameof(AdvancedView_FirmwareDeterminedCeiling));
    public static string AdvancedView_GpuModeSwitchingRequiresASystemRebootSomeO => Get(nameof(AdvancedView_GpuModeSwitchingRequiresASystemRebootSomeO));
    public static string AdvancedView_GpuPowerBoostLevel => Get(nameof(AdvancedView_GpuPowerBoostLevel));
    public static string AdvancedView_GpuPowerBoostTgpDynamicBoost => Get(nameof(AdvancedView_GpuPowerBoostTgpDynamicBoost));
    public static string AdvancedView_GpuSwitchingAdvancedOptimus => Get(nameof(AdvancedView_GpuSwitchingAdvancedOptimus));
    public static string AdvancedView_HardwareLimitation => Get(nameof(AdvancedView_HardwareLimitation));
    public static string AdvancedView_HighFans => Get(nameof(AdvancedView_HighFans));
    public static string AdvancedView_HpOmenSpecificFeaturesPerformanceModesFanC => Get(nameof(AdvancedView_HpOmenSpecificFeaturesPerformanceModesFanC));
    public static string AdvancedView_HpTelemetryBackgroundServices => Get(nameof(AdvancedView_HpTelemetryBackgroundServices));
    public static string AdvancedView_ImprovesPixelResponseTimeOn165hzDisplays => Get(nameof(AdvancedView_ImprovesPixelResponseTimeOn165hzDisplays));
    public static string AdvancedView_LowPower => Get(nameof(AdvancedView_LowPower));
    public static string AdvancedView_MaximumPower => Get(nameof(AdvancedView_MaximumPower));
    public static string AdvancedView_MaxPower => Get(nameof(AdvancedView_MaxPower));
    public static string AdvancedView_OmenControls => Get(nameof(AdvancedView_OmenControls));
    public static string AdvancedView_PanelOverdrive => Get(nameof(AdvancedView_PanelOverdrive));
    public static string AdvancedView_Performance => Get(nameof(AdvancedView_Performance));
    public static string AdvancedView_PerformanceMode => Get(nameof(AdvancedView_PerformanceMode));
    public static string AdvancedView_PerformanceModeMaximumPowerHighFanSpeed => Get(nameof(AdvancedView_PerformanceModeMaximumPowerHighFanSpeed));
    public static string AdvancedView_PowerLevel => Get(nameof(AdvancedView_PowerLevel));
    public static string AdvancedView_QuietPerformanceModeSilentOperationLowPowe => Get(nameof(AdvancedView_QuietPerformanceModeSilentOperationLowPowe));
    public static string AdvancedView_Recommended => Get(nameof(AdvancedView_Recommended));
    public static string AdvancedView_ReducesPanelResponseTimeForLessMotionBlurD => Get(nameof(AdvancedView_ReducesPanelResponseTimeForLessMotionBlurD));
    public static string AdvancedView_RememberCurrentState => Get(nameof(AdvancedView_RememberCurrentState));
    public static string AdvancedView_RememberTheCurrentFanGpuBoostRefreshRateLi => Get(nameof(AdvancedView_RememberTheCurrentFanGpuBoostRefreshRateLi));
    public static string AdvancedView_Restore => Get(nameof(AdvancedView_Restore));
    public static string AdvancedView_RestoreHpTelemetryServices => Get(nameof(AdvancedView_RestoreHpTelemetryServices));
    public static string AdvancedView_SilentFans => Get(nameof(AdvancedView_SilentFans));
    public static string AdvancedView_SilentOperation => Get(nameof(AdvancedView_SilentOperation));
    public static string AdvancedView_SmartFans => Get(nameof(AdvancedView_SmartFans));
    public static string AdvancedView_StopsAndDisablesHpSAnalyticsAndDiagnostics => Get(nameof(AdvancedView_StopsAndDisablesHpSAnalyticsAndDiagnostics));
    public static string AdvancedView_SwitchBetweenIntegratedAndDiscreteGpuForOp => Get(nameof(AdvancedView_SwitchBetweenIntegratedAndDiscreteGpuForOp));
    public static string AdvancedView_SwitchGpuModeTo => Get(nameof(AdvancedView_SwitchGpuModeTo));
    public static string AdvancedView_SwitchTo => Get(nameof(AdvancedView_SwitchTo));
    public static string AdvancedView_SystemPowerAndCoolingProfile => Get(nameof(AdvancedView_SystemPowerAndCoolingProfile));
    public static string AdvancedView_TheseLevelsAreRelativeBoostRequestsSentToT => Get(nameof(AdvancedView_TheseLevelsAreRelativeBoostRequestsSentToT));
    public static string AdvancedView_ThisModeRemembers => Get(nameof(AdvancedView_ThisModeRemembers));
    public static string AdvancedView_WhyGpuModeSwitchingIsUnavailable => Get(nameof(AdvancedView_WhyGpuModeSwitchingIsUnavailable));

    // Auto-extracted literals from BloatwareManagerView.xaml
    public static string BloatwareManagerView_BloatwareManager => Get(nameof(BloatwareManagerView_BloatwareManager));
    public static string BloatwareManagerView_CancelBulkOperation => Get(nameof(BloatwareManagerView_CancelBulkOperation));
    public static string BloatwareManagerView_Category => Get(nameof(BloatwareManagerView_Category));
    public static string BloatwareManagerView_ClickScanToSearchForBloatware => Get(nameof(BloatwareManagerView_ClickScanToSearchForBloatware));
    public static string BloatwareManagerView_DependencyHint => Get(nameof(BloatwareManagerView_DependencyHint));
    public static string BloatwareManagerView_Description => Get(nameof(BloatwareManagerView_Description));
    public static string BloatwareManagerView_Detected => Get(nameof(BloatwareManagerView_Detected));
    public static string BloatwareManagerView_DryRun => Get(nameof(BloatwareManagerView_DryRun));
    public static string BloatwareManagerView_ExportBloatwareDryRunReport => Get(nameof(BloatwareManagerView_ExportBloatwareDryRunReport));
    public static string BloatwareManagerView_ExportLog => Get(nameof(BloatwareManagerView_ExportLog));
    public static string BloatwareManagerView_ExportRemovalLog => Get(nameof(BloatwareManagerView_ExportRemovalLog));
    public static string BloatwareManagerView_ExportRemovalResultsToALogFile => Get(nameof(BloatwareManagerView_ExportRemovalResultsToALogFile));
    public static string BloatwareManagerView_LowRisk => Get(nameof(BloatwareManagerView_LowRisk));
    public static string BloatwareManagerView_NoBloatwareDetected => Get(nameof(BloatwareManagerView_NoBloatwareDetected));
    public static string BloatwareManagerView_OpenTheMostRecentRemovalReport => Get(nameof(BloatwareManagerView_OpenTheMostRecentRemovalReport));
    public static string BloatwareManagerView_Publisher => Get(nameof(BloatwareManagerView_Publisher));
    public static string BloatwareManagerView_RemoveAllLowRiskItems => Get(nameof(BloatwareManagerView_RemoveAllLowRiskItems));
    public static string BloatwareManagerView_Removed => Get(nameof(BloatwareManagerView_Removed));
    public static string BloatwareManagerView_RemoveLowRisk => Get(nameof(BloatwareManagerView_RemoveLowRisk));
    public static string BloatwareManagerView_RemovePreInstalledBloatwareToImproveSystem => Get(nameof(BloatwareManagerView_RemovePreInstalledBloatwareToImproveSystem));
    public static string BloatwareManagerView_RemoveSelected => Get(nameof(BloatwareManagerView_RemoveSelected));
    public static string BloatwareManagerView_RemoveSelectedItem => Get(nameof(BloatwareManagerView_RemoveSelectedItem));
    public static string BloatwareManagerView_RestoreSelectedItem => Get(nameof(BloatwareManagerView_RestoreSelectedItem));
    public static string BloatwareManagerView_ResultDetail => Get(nameof(BloatwareManagerView_ResultDetail));
    public static string BloatwareManagerView_Risk => Get(nameof(BloatwareManagerView_Risk));
    public static string BloatwareManagerView_ScanForBloatware => Get(nameof(BloatwareManagerView_ScanForBloatware));
    public static string BloatwareManagerView_SearchAppsPublishers => Get(nameof(BloatwareManagerView_SearchAppsPublishers));
    public static string BloatwareManagerView_SomeRemovalsMayRequireARestart => Get(nameof(BloatwareManagerView_SomeRemovalsMayRequireARestart));
    public static string BloatwareManagerView_ToggleAll => Get(nameof(BloatwareManagerView_ToggleAll));
    public static string BloatwareManagerView_ViewLastRemovalReport => Get(nameof(BloatwareManagerView_ViewLastRemovalReport));
    public static string BloatwareManagerView_ViewReport => Get(nameof(BloatwareManagerView_ViewReport));

    // Auto-extracted literals from ColorPickerDialog.xaml
    public static string ColorPickerDialog_CancelEsc => Get(nameof(ColorPickerDialog_CancelEsc));
    public static string ColorPickerDialog_Hex => Get(nameof(ColorPickerDialog_Hex));
    public static string ColorPickerDialog_SelectColor => Get(nameof(ColorPickerDialog_SelectColor));
    public static string ColorPickerDialog_SelectZoneColor => Get(nameof(ColorPickerDialog_SelectZoneColor));
    public static string ColorPickerDialog_Zone1Left => Get(nameof(ColorPickerDialog_Zone1Left));

    // Auto-extracted literals from DashboardView.xaml
    public static string DashboardView_Battery => Get(nameof(DashboardView_Battery));
    public static string DashboardView_BatteryChargeLevelAndPowerStatusKeepBatter => Get(nameof(DashboardView_BatteryChargeLevelAndPowerStatusKeepBatter));
    public static string DashboardView_BatteryLevelIndicator => Get(nameof(DashboardView_BatteryLevelIndicator));
    public static string DashboardView_CpuFan => Get(nameof(DashboardView_CpuFan));
    public static string DashboardView_CpuFrequencies => Get(nameof(DashboardView_CpuFrequencies));
    public static string DashboardView_CpuPeak => Get(nameof(DashboardView_CpuPeak));
    public static string DashboardView_CpuTemperatureAndUtilizationGreenOkYellowW => Get(nameof(DashboardView_CpuTemperatureAndUtilizationGreenOkYellowW));
    public static string DashboardView_Current => Get(nameof(DashboardView_Current));
    public static string DashboardView_CurrentFanSpeeds => Get(nameof(DashboardView_CurrentFanSpeeds));
    public static string DashboardView_FanCurve => Get(nameof(DashboardView_FanCurve));
    public static string DashboardView_FanMode => Get(nameof(DashboardView_FanMode));
    public static string DashboardView_GpuFan => Get(nameof(DashboardView_GpuFan));
    public static string DashboardView_GpuPeak => Get(nameof(DashboardView_GpuPeak));
    public static string DashboardView_GpuTemperatureLoadAndVramUsageGpusCanSafel => Get(nameof(DashboardView_GpuTemperatureLoadAndVramUsageGpusCanSafel));
    public static string DashboardView_GpuVoltageCurrent => Get(nameof(DashboardView_GpuVoltageCurrent));
    public static string DashboardView_Graphics => Get(nameof(DashboardView_Graphics));
    public static string DashboardView_HardwareSummaryInformationIsStillAvailable => Get(nameof(DashboardView_HardwareSummaryInformationIsStillAvailable));
    public static string DashboardView_History => Get(nameof(DashboardView_History));
    public static string DashboardView_LiveChartsAreDisabledToMinimizeMonitoringO => Get(nameof(DashboardView_LiveChartsAreDisabledToMinimizeMonitoringO));
    public static string DashboardView_LiveFanCurveShowingTemperatureVsFanSpeedRe => Get(nameof(DashboardView_LiveFanCurveShowingTemperatureVsFanSpeedRe));
    public static string DashboardView_MemoryUsageIndicator => Get(nameof(DashboardView_MemoryUsageIndicator));
    public static string DashboardView_Processor => Get(nameof(DashboardView_Processor));
    public static string DashboardView_RealTimeHardwareTelemetry => Get(nameof(DashboardView_RealTimeHardwareTelemetry));
    public static string DashboardView_ReducedCpuUsageMode => Get(nameof(DashboardView_ReducedCpuUsageMode));
    public static string DashboardView_Refresh => Get(nameof(DashboardView_Refresh));
    public static string DashboardView_RefreshDashboardReadings => Get(nameof(DashboardView_RefreshDashboardReadings));
    public static string DashboardView_ShowLast15MinutesOfHistory => Get(nameof(DashboardView_ShowLast15MinutesOfHistory));
    public static string DashboardView_ShowLast1MinuteOfHistory => Get(nameof(DashboardView_ShowLast1MinuteOfHistory));
    public static string DashboardView_ShowLast30MinutesOfHistory => Get(nameof(DashboardView_ShowLast30MinutesOfHistory));
    public static string DashboardView_ShowLast5MinutesOfHistory => Get(nameof(DashboardView_ShowLast5MinutesOfHistory));
    public static string DashboardView_SsdTemperatureAndDiskActivityOptimalSsdTem => Get(nameof(DashboardView_SsdTemperatureAndDiskActivityOptimalSsdTem));
    public static string DashboardView_SsdTemperatureIndicator => Get(nameof(DashboardView_SsdTemperatureIndicator));
    public static string DashboardView_Storage => Get(nameof(DashboardView_Storage));
    public static string DashboardView_SystemOverview => Get(nameof(DashboardView_SystemOverview));
    public static string DashboardView_SystemPowerConsumptionFromCpuAndGpuHigherV => Get(nameof(DashboardView_SystemPowerConsumptionFromCpuAndGpuHigherV));
    public static string DashboardView_SystemRamUsageHighUsageMayCauseSlowdownsCo => Get(nameof(DashboardView_SystemRamUsageHighUsageMayCauseSlowdownsCo));
    public static string DashboardView_TemperatureStatus => Get(nameof(DashboardView_TemperatureStatus));
    public static string DashboardView_Throttling => Get(nameof(DashboardView_Throttling));
    public static string DashboardView_TotalSystemPower => Get(nameof(DashboardView_TotalSystemPower));
    public static string DashboardView_Utilization => Get(nameof(DashboardView_Utilization));
    public static string DashboardView_Voltage => Get(nameof(DashboardView_Voltage));

    // Auto-extracted literals from DiagnosticExportControl.xaml
    public static string DiagnosticExportControl_AnonymizeSensitiveDataRemoveUsernamesPaths => Get(nameof(DiagnosticExportControl_AnonymizeSensitiveDataRemoveUsernamesPaths));
    public static string DiagnosticExportControl_ApplicationLogsAndErrorMessages => Get(nameof(DiagnosticExportControl_ApplicationLogsAndErrorMessages));
    public static string DiagnosticExportControl_ApplicationLogsLast1000Entries => Get(nameof(DiagnosticExportControl_ApplicationLogsLast1000Entries));
    public static string DiagnosticExportControl_CompressedZipSmallerFileSize => Get(nameof(DiagnosticExportControl_CompressedZipSmallerFileSize));
    public static string DiagnosticExportControl_ContainsSystemInfoHardwareDataLogsSettings => Get(nameof(DiagnosticExportControl_ContainsSystemInfoHardwareDataLogsSettings));
    public static string DiagnosticExportControl_CopyPath => Get(nameof(DiagnosticExportControl_CopyPath));
    public static string DiagnosticExportControl_CreateGithubIssue => Get(nameof(DiagnosticExportControl_CreateGithubIssue));
    public static string DiagnosticExportControl_CurrentSettingsAndConfiguration => Get(nameof(DiagnosticExportControl_CurrentSettingsAndConfiguration));
    public static string DiagnosticExportControl_DiagnosticExport => Get(nameof(DiagnosticExportControl_DiagnosticExport));
    public static string DiagnosticExportControl_ExportCompletedSuccessfully => Get(nameof(DiagnosticExportControl_ExportCompletedSuccessfully));
    public static string DiagnosticExportControl_ExportDiagnostics => Get(nameof(DiagnosticExportControl_ExportDiagnostics));
    public static string DiagnosticExportControl_ExportFormat => Get(nameof(DiagnosticExportControl_ExportFormat));
    public static string DiagnosticExportControl_ExportOptions => Get(nameof(DiagnosticExportControl_ExportOptions));
    public static string DiagnosticExportControl_ExportProgress => Get(nameof(DiagnosticExportControl_ExportProgress));
    public static string DiagnosticExportControl_ExportResults => Get(nameof(DiagnosticExportControl_ExportResults));
    public static string DiagnosticExportControl_ExportSystemDiagnosticsAndLogsToHelpTroubl => Get(nameof(DiagnosticExportControl_ExportSystemDiagnosticsAndLogsToHelpTroubl));
    public static string DiagnosticExportControl_FanCalibrationDataIfAvailable => Get(nameof(DiagnosticExportControl_FanCalibrationDataIfAvailable));
    public static string DiagnosticExportControl_FanMaxVerificationRunsAQuickVerificationOf => Get(nameof(DiagnosticExportControl_FanMaxVerificationRunsAQuickVerificationOf));
    public static string DiagnosticExportControl_FileSavedTo => Get(nameof(DiagnosticExportControl_FileSavedTo));
    public static string DiagnosticExportControl_FileSize => Get(nameof(DiagnosticExportControl_FileSize));
    public static string DiagnosticExportControl_GithubIssueIntegration => Get(nameof(DiagnosticExportControl_GithubIssueIntegration));
    public static string DiagnosticExportControl_HardwareMonitoringDataTemperaturesFanSpeed => Get(nameof(DiagnosticExportControl_HardwareMonitoringDataTemperaturesFanSpeed));
    public static string DiagnosticExportControl_HardwareMonitoringDataTemperaturesFanSpeed_2 => Get(nameof(DiagnosticExportControl_HardwareMonitoringDataTemperaturesFanSpeed_2));
    public static string DiagnosticExportControl_JsonFormatStructuredMachineReadable => Get(nameof(DiagnosticExportControl_JsonFormatStructuredMachineReadable));
    public static string DiagnosticExportControl_N1ExportDiagnosticsUsingTheButtonBelow => Get(nameof(DiagnosticExportControl_N1ExportDiagnosticsUsingTheButtonBelow));
    public static string DiagnosticExportControl_N2ClickCreateGithubIssueToOpenAPreFilledIss => Get(nameof(DiagnosticExportControl_N2ClickCreateGithubIssueToOpenAPreFilledIss));
    public static string DiagnosticExportControl_N3AttachTheExportedDiagnosticFileToTheIssue => Get(nameof(DiagnosticExportControl_N3AttachTheExportedDiagnosticFileToTheIssue));
    public static string DiagnosticExportControl_NoPersonalFilesCredentialsOrSensitiveInfor => Get(nameof(DiagnosticExportControl_NoPersonalFilesCredentialsOrSensitiveInfor));
    public static string DiagnosticExportControl_NoteDiagnosticDataIncludesSystemInformatio => Get(nameof(DiagnosticExportControl_NoteDiagnosticDataIncludesSystemInformatio));
    public static string DiagnosticExportControl_OpenFolder => Get(nameof(DiagnosticExportControl_OpenFolder));
    public static string DiagnosticExportControl_PerformanceModeAndFanCalibrationData => Get(nameof(DiagnosticExportControl_PerformanceModeAndFanCalibrationData));
    public static string DiagnosticExportControl_PerformanceModeData => Get(nameof(DiagnosticExportControl_PerformanceModeData));
    public static string DiagnosticExportControl_PreparingDiagnosticData => Get(nameof(DiagnosticExportControl_PreparingDiagnosticData));
    public static string DiagnosticExportControl_PrivacySecurity => Get(nameof(DiagnosticExportControl_PrivacySecurity));
    public static string DiagnosticExportControl_QuickGithubIssueCreation => Get(nameof(DiagnosticExportControl_QuickGithubIssueCreation));
    public static string DiagnosticExportControl_SettingsConfiguration => Get(nameof(DiagnosticExportControl_SettingsConfiguration));
    public static string DiagnosticExportControl_SystemInformationOsHardwareDrivers => Get(nameof(DiagnosticExportControl_SystemInformationOsHardwareDrivers));
    public static string DiagnosticExportControl_SystemInformationOsVersionHardwareSpecsDri => Get(nameof(DiagnosticExportControl_SystemInformationOsVersionHardwareSpecsDri));
    public static string DiagnosticExportControl_TextFormatHumanReadable => Get(nameof(DiagnosticExportControl_TextFormatHumanReadable));
    public static string DiagnosticExportControl_WhatGetsExported => Get(nameof(DiagnosticExportControl_WhatGetsExported));

    // Auto-extracted literals from DiagnosticsView.xaml
    public static string DiagnosticsView_AboutDiagnostics => Get(nameof(DiagnosticsView_AboutDiagnostics));
    public static string DiagnosticsView_AdvancedOptimusGpuSwitching => Get(nameof(DiagnosticsView_AdvancedOptimusGpuSwitching));
    public static string DiagnosticsView_AppliedAtStartupAfterAResumeAndWhenTheSupp => Get(nameof(DiagnosticsView_AppliedAtStartupAfterAResumeAndWhenTheSupp));
    public static string DiagnosticsView_Backend => Get(nameof(DiagnosticsView_Backend));
    public static string DiagnosticsView_CheckCurrentGpuPowerLimit => Get(nameof(DiagnosticsView_CheckCurrentGpuPowerLimit));
    public static string DiagnosticsView_ConnectedAdapter => Get(nameof(DiagnosticsView_ConnectedAdapter));
    public static string DiagnosticsView_ConsecutiveTimeouts => Get(nameof(DiagnosticsView_ConsecutiveTimeouts));
    public static string DiagnosticsView_CpuTempOverride => Get(nameof(DiagnosticsView_CpuTempOverride));
    public static string DiagnosticsView_CpuUndervolting => Get(nameof(DiagnosticsView_CpuUndervolting));
    public static string DiagnosticsView_CreateACopyOfTheTelemetryJsonForUpload => Get(nameof(DiagnosticsView_CreateACopyOfTheTelemetryJsonForUpload));
    public static string DiagnosticsView_CreateDiagnosticsBundleAndCopyModelInfoToC => Get(nameof(DiagnosticsView_CreateDiagnosticsBundleAndCopyModelInfoToC));
    public static string DiagnosticsView_CurrentGpuPowerLimit => Get(nameof(DiagnosticsView_CurrentGpuPowerLimit));
    public static string DiagnosticsView_CustomFanCurves => Get(nameof(DiagnosticsView_CustomFanCurves));
    public static string DiagnosticsView_DiagnosticToolsAreForTroubleshootingOnlyTh => Get(nameof(DiagnosticsView_DiagnosticToolsAreForTroubleshootingOnlyTh));
    public static string DiagnosticsView_DoThisOnItsOwn => Get(nameof(DiagnosticsView_DoThisOnItsOwn));
    public static string DiagnosticsView_DropTheAdapterPowerClamp => Get(nameof(DiagnosticsView_DropTheAdapterPowerClamp));
    public static string DiagnosticsView_DropTheClamp => Get(nameof(DiagnosticsView_DropTheClamp));
    public static string DiagnosticsView_ExperimentalPinsTwoEcBitsAndFiresOneWmiCom => Get(nameof(DiagnosticsView_ExperimentalPinsTwoEcBitsAndFiresOneWmiCom));
    public static string DiagnosticsView_ExportFullDiagnosticsReport => Get(nameof(DiagnosticsView_ExportFullDiagnosticsReport));
    public static string DiagnosticsView_ExportTelemetry => Get(nameof(DiagnosticsView_ExportTelemetry));
    public static string DiagnosticsView_ExportTelemetryJson => Get(nameof(DiagnosticsView_ExportTelemetryJson));
    public static string DiagnosticsView_FanDiagnostics => Get(nameof(DiagnosticsView_FanDiagnostics));
    public static string DiagnosticsView_FanDiagnostics_2 => Get(nameof(DiagnosticsView_FanDiagnostics_2));
    public static string DiagnosticsView_GpuMuxSwitch => Get(nameof(DiagnosticsView_GpuMuxSwitch));
    public static string DiagnosticsView_GpuPowerBoostTgp => Get(nameof(DiagnosticsView_GpuPowerBoostTgp));
    public static string DiagnosticsView_GpuPowerUnlockExperimentalBoard8d87Only => Get(nameof(DiagnosticsView_GpuPowerUnlockExperimentalBoard8d87Only));
    public static string DiagnosticsView_HardwareTestingAndValidationTools => Get(nameof(DiagnosticsView_HardwareTestingAndValidationTools));
    public static string DiagnosticsView_Health => Get(nameof(DiagnosticsView_Health));
    public static string DiagnosticsView_IndependentCpuGpuFanCurves => Get(nameof(DiagnosticsView_IndependentCpuGpuFanCurves));
    public static string DiagnosticsView_InferredNotYetVerified => Get(nameof(DiagnosticsView_InferredNotYetVerified));
    public static string DiagnosticsView_KeepTheCpuPowerLimitsRaised => Get(nameof(DiagnosticsView_KeepTheCpuPowerLimitsRaised));
    public static string DiagnosticsView_KeyboardDiagnostics => Get(nameof(DiagnosticsView_KeyboardDiagnostics));
    public static string DiagnosticsView_KeyboardLightingDiagnostics => Get(nameof(DiagnosticsView_KeyboardLightingDiagnostics));
    public static string DiagnosticsView_LastSampleAge => Get(nameof(DiagnosticsView_LastSampleAge));
    public static string DiagnosticsView_LightBar => Get(nameof(DiagnosticsView_LightBar));
    public static string DiagnosticsView_ModelCapabilities => Get(nameof(DiagnosticsView_ModelCapabilities));
    public static string DiagnosticsView_MonitoringDiagnostics => Get(nameof(DiagnosticsView_MonitoringDiagnostics));
    public static string DiagnosticsView_N4ZoneKeyboardRgb => Get(nameof(DiagnosticsView_N4ZoneKeyboardRgb));
    public static string DiagnosticsView_NotConfirmedOnRealHardwareForcingThisOnAnU => Get(nameof(DiagnosticsView_NotConfirmedOnRealHardwareForcingThisOnAnU));
    public static string DiagnosticsView_NotPresent => Get(nameof(DiagnosticsView_NotPresent));
    public static string DiagnosticsView_NotSupported => Get(nameof(DiagnosticsView_NotSupported));
    public static string DiagnosticsView_OnlyWhenTheDriverHasParkedTheGpuAndTheDisp => Get(nameof(DiagnosticsView_OnlyWhenTheDriverHasParkedTheGpuAndTheDisp));
    public static string DiagnosticsView_OpenAppLogsFolder => Get(nameof(DiagnosticsView_OpenAppLogsFolder));
    public static string DiagnosticsView_OpenOmencoreDataFolder => Get(nameof(DiagnosticsView_OpenOmencoreDataFolder));
    public static string DiagnosticsView_OpenTheFolderContainingOmencoreSConfigAndS => Get(nameof(DiagnosticsView_OpenTheFolderContainingOmencoreSConfigAndS));
    public static string DiagnosticsView_OpenTheFolderContainingOmencoreSLogFilesIn => Get(nameof(DiagnosticsView_OpenTheFolderContainingOmencoreSLogFilesIn));
    public static string DiagnosticsView_OverboostMode => Get(nameof(DiagnosticsView_OverboostMode));
    public static string DiagnosticsView_PerKeyRgb => Get(nameof(DiagnosticsView_PerKeyRgb));
    public static string DiagnosticsView_PowerAdapter => Get(nameof(DiagnosticsView_PowerAdapter));
    public static string DiagnosticsView_PowerLimitPl1Pl2Adjustment => Get(nameof(DiagnosticsView_PowerLimitPl1Pl2Adjustment));
    public static string DiagnosticsView_Present => Get(nameof(DiagnosticsView_Present));
    public static string DiagnosticsView_ProductId => Get(nameof(DiagnosticsView_ProductId));
    public static string DiagnosticsView_QuickActions => Get(nameof(DiagnosticsView_QuickActions));
    public static string DiagnosticsView_ReadFromTheBiosEachTimeThisTabIsOpenedThis => Get(nameof(DiagnosticsView_ReadFromTheBiosEachTimeThisTabIsOpenedThis));
    public static string DiagnosticsView_ReadsEveryCpuTemperatureSourceOmencoreKnow => Get(nameof(DiagnosticsView_ReadsEveryCpuTemperatureSourceOmencoreKnow));
    public static string DiagnosticsView_ReadTheGpuSCurrentPowerLimitDirectlyFromTh => Get(nameof(DiagnosticsView_ReadTheGpuSCurrentPowerLimitDirectlyFromTh));
    public static string DiagnosticsView_ReadWmiBiosAcpiAndLibrehardwaremonitorCpuT => Get(nameof(DiagnosticsView_ReadWmiBiosAcpiAndLibrehardwaremonitorCpuT));
    public static string DiagnosticsView_RealFanRpmReadback => Get(nameof(DiagnosticsView_RealFanRpmReadback));
    public static string DiagnosticsView_ReleaseTheAmdSmuPowerLimitsOmencoreIsCurre => Get(nameof(DiagnosticsView_ReleaseTheAmdSmuPowerLimitsOmencoreIsCurre));
    public static string DiagnosticsView_ReportModel => Get(nameof(DiagnosticsView_ReportModel));
    public static string DiagnosticsView_ReportThisModelToTheMaintainer => Get(nameof(DiagnosticsView_ReportThisModelToTheMaintainer));
    public static string DiagnosticsView_RestartsTheGpuDeviceToForceTheDriverToReRe => Get(nameof(DiagnosticsView_RestartsTheGpuDeviceToForceTheDriverToReRe));
    public static string DiagnosticsView_RestartTheGpuToDropItsClampButOnlyWhileItI => Get(nameof(DiagnosticsView_RestartTheGpuToDropItsClampButOnlyWhileItI));
    public static string DiagnosticsView_RunTemperatureSourceComparison => Get(nameof(DiagnosticsView_RunTemperatureSourceComparison));
    public static string DiagnosticsView_SaveTheFullCoreControlReadinessReportToAFi => Get(nameof(DiagnosticsView_SaveTheFullCoreControlReadinessReportToAFi));
    public static string DiagnosticsView_ShippedWith => Get(nameof(DiagnosticsView_ShippedWith));
    public static string DiagnosticsView_Source => Get(nameof(DiagnosticsView_Source));
    public static string DiagnosticsView_StopHoldingTheCpuLimits => Get(nameof(DiagnosticsView_StopHoldingTheCpuLimits));
    public static string DiagnosticsView_StopHoldingTheCpuPowerLimits => Get(nameof(DiagnosticsView_StopHoldingTheCpuPowerLimits));
    public static string DiagnosticsView_Supported => Get(nameof(DiagnosticsView_Supported));
    public static string DiagnosticsView_SystemDiagnostics => Get(nameof(DiagnosticsView_SystemDiagnostics));
    public static string DiagnosticsView_TemperatureSourceComparison => Get(nameof(DiagnosticsView_TemperatureSourceComparison));
    public static string DiagnosticsView_TestIndividualKeyBacklightingVerifyRgbZone => Get(nameof(DiagnosticsView_TestIndividualKeyBacklightingVerifyRgbZone));
    public static string DiagnosticsView_TheFirmwareTellsTheGpuDriverAboutTheAdapte => Get(nameof(DiagnosticsView_TheFirmwareTellsTheGpuDriverAboutTheAdapte));
    public static string DiagnosticsView_TheSameSupplyClampsTheCpuByADifferentRoute => Get(nameof(DiagnosticsView_TheSameSupplyClampsTheCpuByADifferentRoute));
    public static string DiagnosticsView_ThisBoardSGpuIsHeldUnderItsFull175WRatingB => Get(nameof(DiagnosticsView_ThisBoardSGpuIsHeldUnderItsFull175WRatingB));
    public static string DiagnosticsView_ThisReflectsWhatOmencoreSModelDatabaseKnow => Get(nameof(DiagnosticsView_ThisReflectsWhatOmencoreSModelDatabaseKnow));
    public static string DiagnosticsView_TryTheGpuPowerUnlock => Get(nameof(DiagnosticsView_TryTheGpuPowerUnlock));
    public static string DiagnosticsView_TryTheGpuPowerUnlockExperimental => Get(nameof(DiagnosticsView_TryTheGpuPowerUnlockExperimental));
    public static string DiagnosticsView_UserVerifiedProfile => Get(nameof(DiagnosticsView_UserVerifiedProfile));
    public static string DiagnosticsView_VerifyFanControlResponsivenessTestDifferen => Get(nameof(DiagnosticsView_VerifyFanControlResponsivenessTestDifferen));
    public static string DiagnosticsView_WorkerTimeouts => Get(nameof(DiagnosticsView_WorkerTimeouts));
    public static string DiagnosticsView_YourScreensWillGoBlackForAFewSecondsAndAny => Get(nameof(DiagnosticsView_YourScreensWillGoBlackForAFewSecondsAndAny));

    // Auto-extracted literals from FanCalibrationControl.xaml
    public static string FanCalibrationControl_AppliedLevel => Get(nameof(FanCalibrationControl_AppliedLevel));
    public static string FanCalibrationControl_CalibrateFanRpmCurvesForAccurateSpeedContr => Get(nameof(FanCalibrationControl_CalibrateFanRpmCurvesForAccurateSpeedContr));
    public static string FanCalibrationControl_CalibrationCompletedSuccessfully => Get(nameof(FanCalibrationControl_CalibrationCompletedSuccessfully));
    public static string FanCalibrationControl_CalibrationDataIsStoredLocallyAndCanBeShar => Get(nameof(FanCalibrationControl_CalibrationDataIsStoredLocallyAndCanBeShar));
    public static string FanCalibrationControl_CalibrationMayTake510MinutesPerFan => Get(nameof(FanCalibrationControl_CalibrationMayTake510MinutesPerFan));
    public static string FanCalibrationControl_CalibrationProgress => Get(nameof(FanCalibrationControl_CalibrationProgress));
    public static string FanCalibrationControl_CalibrationResults => Get(nameof(FanCalibrationControl_CalibrationResults));
    public static string FanCalibrationControl_ClearData => Get(nameof(FanCalibrationControl_ClearData));
    public static string FanCalibrationControl_CpuFanFan0 => Get(nameof(FanCalibrationControl_CpuFanFan0));
    public static string FanCalibrationControl_DetectingLaptopModel => Get(nameof(FanCalibrationControl_DetectingLaptopModel));
    public static string FanCalibrationControl_DoNotInterruptTheCalibrationProcess => Get(nameof(FanCalibrationControl_DoNotInterruptTheCalibrationProcess));
    public static string FanCalibrationControl_Duration => Get(nameof(FanCalibrationControl_Duration));
    public static string FanCalibrationControl_FanCalibration => Get(nameof(FanCalibrationControl_FanCalibration));
    public static string FanCalibrationControl_FanSelection => Get(nameof(FanCalibrationControl_FanSelection));
    public static string FanCalibrationControl_FansWillRunAtVariousSpeedsEnsureProperVent => Get(nameof(FanCalibrationControl_FansWillRunAtVariousSpeedsEnsureProperVent));
    public static string FanCalibrationControl_GpuFanFan1 => Get(nameof(FanCalibrationControl_GpuFanFan1));
    public static string FanCalibrationControl_ImportantNotes => Get(nameof(FanCalibrationControl_ImportantNotes));
    public static string FanCalibrationControl_LoadExisting => Get(nameof(FanCalibrationControl_LoadExisting));
    public static string FanCalibrationControl_MeasuredRpm => Get(nameof(FanCalibrationControl_MeasuredRpm));
    public static string FanCalibrationControl_ModelInformation => Get(nameof(FanCalibrationControl_ModelInformation));
    public static string FanCalibrationControl_NoCalibrationDataFound => Get(nameof(FanCalibrationControl_NoCalibrationDataFound));
    public static string FanCalibrationControl_OmencoreRestoresBiosAutoFanControlWhenCali => Get(nameof(FanCalibrationControl_OmencoreRestoresBiosAutoFanControlWhenCali));
    public static string FanCalibrationControl_PreparingCalibration => Get(nameof(FanCalibrationControl_PreparingCalibration));
    public static string FanCalibrationControl_ReleaseCalibrationManualFanOwnershipAndRet => Get(nameof(FanCalibrationControl_ReleaseCalibrationManualFanOwnershipAndRet));
    public static string FanCalibrationControl_RestoreAuto => Get(nameof(FanCalibrationControl_RestoreAuto));
    public static string FanCalibrationControl_RpmRange => Get(nameof(FanCalibrationControl_RpmRange));
    public static string FanCalibrationControl_SaveResults => Get(nameof(FanCalibrationControl_SaveResults));
    public static string FanCalibrationControl_SelectAFanToSeeCurrentStatus => Get(nameof(FanCalibrationControl_SelectAFanToSeeCurrentStatus));
    public static string FanCalibrationControl_SelectWhichFanToCalibrate => Get(nameof(FanCalibrationControl_SelectWhichFanToCalibrate));
    public static string FanCalibrationControl_StartCalibration => Get(nameof(FanCalibrationControl_StartCalibration));
    public static string FanCalibrationControl_Target => Get(nameof(FanCalibrationControl_Target));
    public static string FanCalibrationControl_ThisProcessWillTestFanSpeedsFrom0To100AndM => Get(nameof(FanCalibrationControl_ThisProcessWillTestFanSpeedsFrom0To100AndM));

    // Auto-extracted literals from FanControlView.xaml
    public static string FanControlView_AddANewCurvePointMax10Points => Get(nameof(FanControlView_AddANewCurvePointMax10Points));
    public static string FanControlView_AddCpuCurvePoint => Get(nameof(FanControlView_AddCpuCurvePoint));
    public static string FanControlView_AddCurvePoint => Get(nameof(FanControlView_AddCurvePoint));
    public static string FanControlView_AddGpuCurvePoint => Get(nameof(FanControlView_AddGpuCurvePoint));
    public static string FanControlView_AggressiveFanCurveThatRampsUpQuicklyGoodFo => Get(nameof(FanControlView_AggressiveFanCurveThatRampsUpQuicklyGoodFo));
    public static string FanControlView_ApplyDirectFanLevel => Get(nameof(FanControlView_ApplyDirectFanLevel));
    public static string FanControlView_ApplyTheSelectedDirectFanLevelNow => Get(nameof(FanControlView_ApplyTheSelectedDirectFanLevelNow));
    public static string FanControlView_AutoFanPresetBiosControlledBalancedCurve => Get(nameof(FanControlView_AutoFanPresetBiosControlledBalancedCurve));
    public static string FanControlView_BiosControlledBalancedCurveRecommendedForE => Get(nameof(FanControlView_BiosControlledBalancedCurveRecommendedForE));
    public static string FanControlView_BlueLineShowsCurrentTemperatureMaxOfCpuGpu => Get(nameof(FanControlView_BlueLineShowsCurrentTemperatureMaxOfCpuGpu));
    public static string FanControlView_BypassSmoothingOnSaveApply => Get(nameof(FanControlView_BypassSmoothingOnSaveApply));
    public static string FanControlView_CalibrationWizard => Get(nameof(FanControlView_CalibrationWizard));
    public static string FanControlView_ClearOmencoreFanOwnershipAndReturnControlT => Get(nameof(FanControlView_ClearOmencoreFanOwnershipAndReturnControlT));
    public static string FanControlView_ClickEmptyAreaToAddNewPoints => Get(nameof(FanControlView_ClickEmptyAreaToAddNewPoints));
    public static string FanControlView_CopyCode => Get(nameof(FanControlView_CopyCode));
    public static string FanControlView_CopyCpuCurveToGpu => Get(nameof(FanControlView_CopyCpuCurveToGpu));
    public static string FanControlView_CopyCurveAsShareCode => Get(nameof(FanControlView_CopyCurveAsShareCode));
    public static string FanControlView_CopyThisCurveAsAShareCodeYouCanPasteIntoDi => Get(nameof(FanControlView_CopyThisCurveAsAShareCodeYouCanPasteIntoDi));
    public static string FanControlView_Cpu => Get(nameof(FanControlView_Cpu));
    public static string FanControlView_CurrentPoints => Get(nameof(FanControlView_CurrentPoints));
    public static string FanControlView_CurveActive => Get(nameof(FanControlView_CurveActive));
    public static string FanControlView_CustomFanPresetUserDefinedCurve => Get(nameof(FanControlView_CustomFanPresetUserDefinedCurve));
    public static string FanControlView_CustomPresetName => Get(nameof(FanControlView_CustomPresetName));
    public static string FanControlView_DeleteSelectedPreset => Get(nameof(FanControlView_DeleteSelectedPreset));
    public static string FanControlView_DeleteTheSelectedCustomPreset => Get(nameof(FanControlView_DeleteTheSelectedCustomPreset));
    public static string FanControlView_DirectFanLevel => Get(nameof(FanControlView_DirectFanLevel));
    public static string FanControlView_DirectFanLevelPercentage => Get(nameof(FanControlView_DirectFanLevelPercentage));
    public static string FanControlView_DirectFixedFanLevelPreset => Get(nameof(FanControlView_DirectFixedFanLevelPreset));
    public static string FanControlView_Dismiss => Get(nameof(FanControlView_Dismiss));
    public static string FanControlView_DismissFanHardwareIssueWarning => Get(nameof(FanControlView_DismissFanHardwareIssueWarning));
    public static string FanControlView_DismissFanPerformanceDecouplingNotice => Get(nameof(FanControlView_DismissFanPerformanceDecouplingNotice));
    public static string FanControlView_DismissStartupRestoreHint => Get(nameof(FanControlView_DismissStartupRestoreHint));
    public static string FanControlView_DragPointsToAdjustTemperatureFanSpeedMappi => Get(nameof(FanControlView_DragPointsToAdjustTemperatureFanSpeedMappi));
    public static string FanControlView_DragToRequestAFixedFanLevel => Get(nameof(FanControlView_DragToRequestAFixedFanLevel));
    public static string FanControlView_ExportPresetsToAJsonFile => Get(nameof(FanControlView_ExportPresetsToAJsonFile));
    public static string FanControlView_ExportPresetsToFile => Get(nameof(FanControlView_ExportPresetsToFile));
    public static string FanControlView_Extreme => Get(nameof(FanControlView_Extreme));
    public static string FanControlView_ExtremeFanPresetHighestNonMaxCoolingCurve => Get(nameof(FanControlView_ExtremeFanPresetHighestNonMaxCoolingCurve));
    public static string FanControlView_FanControlOwner => Get(nameof(FanControlView_FanControlOwner));
    public static string FanControlView_FanCurveByCpuTemp => Get(nameof(FanControlView_FanCurveByCpuTemp));
    public static string FanControlView_FanCurveByGpuTemp => Get(nameof(FanControlView_FanCurveByGpuTemp));
    public static string FanControlView_FanPerformanceDecouplingIsNowTheDefault => Get(nameof(FanControlView_FanPerformanceDecouplingIsNowTheDefault));
    public static string FanControlView_FanProfiles => Get(nameof(FanControlView_FanProfiles));
    public static string FanControlView_FanThermalControl => Get(nameof(FanControlView_FanThermalControl));
    public static string FanControlView_ForcesAllFansToMaximumRpmUseForHeavyGaming => Get(nameof(FanControlView_ForcesAllFansToMaximumRpmUseForHeavyGaming));
    public static string FanControlView_GamingFanPresetAggressiveRampUpCurve => Get(nameof(FanControlView_GamingFanPresetAggressiveRampUpCurve));
    public static string FanControlView_Gpu => Get(nameof(FanControlView_Gpu));
    public static string FanControlView_GpuFanCurveSupportStatus => Get(nameof(FanControlView_GpuFanCurveSupportStatus));
    public static string FanControlView_GpuFanWritesAreModelDependentUnifiedCurveM => Get(nameof(FanControlView_GpuFanWritesAreModelDependentUnifiedCurveM));
    public static string FanControlView_HighestNonMaxCoolingCurveReserves100FanDut => Get(nameof(FanControlView_HighestNonMaxCoolingCurveReserves100FanDut));
    public static string FanControlView_HowToUse => Get(nameof(FanControlView_HowToUse));
    public static string FanControlView_ImportAFanCurveFromAShareCodeOnTheClipboar => Get(nameof(FanControlView_ImportAFanCurveFromAShareCodeOnTheClipboar));
    public static string FanControlView_ImportCurveFromClipboardShareCode => Get(nameof(FanControlView_ImportCurveFromClipboardShareCode));
    public static string FanControlView_ImportPresetsFromAJsonFile => Get(nameof(FanControlView_ImportPresetsFromAJsonFile));
    public static string FanControlView_ImportPresetsFromFile => Get(nameof(FanControlView_ImportPresetsFromFile));
    public static string FanControlView_IndependentCpuGpu => Get(nameof(FanControlView_IndependentCpuGpu));
    public static string FanControlView_InstantApply => Get(nameof(FanControlView_InstantApply));
    public static string FanControlView_LivePreview => Get(nameof(FanControlView_LivePreview));
    public static string FanControlView_ManageYourLaptopSCoolingPerformanceAllFanC => Get(nameof(FanControlView_ManageYourLaptopSCoolingPerformanceAllFanC));
    public static string FanControlView_MaxFanPresetForcesAllFansTo100 => Get(nameof(FanControlView_MaxFanPresetForcesAllFansTo100));
    public static string FanControlView_NameForYourCustomPreset => Get(nameof(FanControlView_NameForYourCustomPreset));
    public static string FanControlView_OmencoreUsesYourLaptopSStandardFirmwareInt => Get(nameof(FanControlView_OmencoreUsesYourLaptopSStandardFirmwareInt));
    public static string FanControlView_OpenFanCalibrationWizard => Get(nameof(FanControlView_OpenFanCalibrationWizard));
    public static string FanControlView_PasteCode => Get(nameof(FanControlView_PasteCode));
    public static string FanControlView_PossibleFanHardwareIssueDetected => Get(nameof(FanControlView_PossibleFanHardwareIssueDetected));
    public static string FanControlView_PrioritizesQuietOperationFansStayLowUntilT => Get(nameof(FanControlView_PrioritizesQuietOperationFansStayLowUntilT));
    public static string FanControlView_RealTimeStatus => Get(nameof(FanControlView_RealTimeStatus));
    public static string FanControlView_Remove => Get(nameof(FanControlView_Remove));
    public static string FanControlView_RemoveLastCpuCurvePoint => Get(nameof(FanControlView_RemoveLastCpuCurvePoint));
    public static string FanControlView_RemoveLastCurvePoint => Get(nameof(FanControlView_RemoveLastCurvePoint));
    public static string FanControlView_RemoveLastGpuCurvePoint => Get(nameof(FanControlView_RemoveLastGpuCurvePoint));
    public static string FanControlView_RemoveTheLastCurvePointMin2Points => Get(nameof(FanControlView_RemoveTheLastCurvePointMin2Points));
    public static string FanControlView_Reset => Get(nameof(FanControlView_Reset));
    public static string FanControlView_ResetCpuCurveToDefault => Get(nameof(FanControlView_ResetCpuCurveToDefault));
    public static string FanControlView_ResetCurveToDefault => Get(nameof(FanControlView_ResetCurveToDefault));
    public static string FanControlView_ResetGpuCurveToDefault => Get(nameof(FanControlView_ResetGpuCurveToDefault));
    public static string FanControlView_RestoreOemAuto => Get(nameof(FanControlView_RestoreOemAuto));
    public static string FanControlView_RestoreOemAutoFanControl => Get(nameof(FanControlView_RestoreOemAutoFanControl));
    public static string FanControlView_RightClickAPointToRemoveIt => Get(nameof(FanControlView_RightClickAPointToRemoveIt));
    public static string FanControlView_SaveCurrentCurveAsACustomPreset => Get(nameof(FanControlView_SaveCurrentCurveAsACustomPreset));
    public static string FanControlView_SaveCustomPreset => Get(nameof(FanControlView_SaveCustomPreset));
    public static string FanControlView_Saved => Get(nameof(FanControlView_Saved));
    public static string FanControlView_SavedCustomPresets => Get(nameof(FanControlView_SavedCustomPresets));
    public static string FanControlView_SelectASavedCustomPresetToLoad => Get(nameof(FanControlView_SelectASavedCustomPresetToLoad));
    public static string FanControlView_SilentFanPresetPrioritizesQuietOperation => Get(nameof(FanControlView_SilentFanPresetPrioritizesQuietOperation));
    public static string FanControlView_Smoothing => Get(nameof(FanControlView_Smoothing));
    public static string FanControlView_SmoothingDurationInMilliseconds => Get(nameof(FanControlView_SmoothingDurationInMilliseconds));
    public static string FanControlView_SmoothingStepInMilliseconds => Get(nameof(FanControlView_SmoothingStepInMilliseconds));
    public static string FanControlView_Step => Get(nameof(FanControlView_Step));
    public static string FanControlView_TemperatureHistory => Get(nameof(FanControlView_TemperatureHistory));
    public static string FanControlView_ThermalProtectionActive => Get(nameof(FanControlView_ThermalProtectionActive));
    public static string FanControlView_ThisWonTSurviveARestartYet => Get(nameof(FanControlView_ThisWonTSurviveARestartYet));
    public static string FanControlView_VisualCurveEditor => Get(nameof(FanControlView_VisualCurveEditor));

    // Auto-extracted literals from FanCurveChart.xaml
    public static string FanCurveChart_FanSpeedVsTemperature => Get(nameof(FanCurveChart_FanSpeedVsTemperature));
    public static string FanCurveChart_LoadingFanCurveData => Get(nameof(FanCurveChart_LoadingFanCurveData));

    // Auto-extracted literals from FanCurveEditor.xaml
    public static string FanCurveEditor_ClickToAddCurvePointsDragToAdjust => Get(nameof(FanCurveEditor_ClickToAddCurvePointsDragToAdjust));
    public static string FanCurveEditor_DragPointsToAdjust => Get(nameof(FanCurveEditor_DragPointsToAdjust));
    public static string FanCurveEditor_FanSpeed => Get(nameof(FanCurveEditor_FanSpeed));
    public static string FanCurveEditor_TemperatureC => Get(nameof(FanCurveEditor_TemperatureC));

    // Auto-extracted literals from FanDiagnosticsView.xaml
    public static string FanDiagnosticsView_ApplyVerify => Get(nameof(FanDiagnosticsView_ApplyVerify));
    public static string FanDiagnosticsView_CopyResults => Get(nameof(FanDiagnosticsView_CopyResults));
    public static string FanDiagnosticsView_CurrentState => Get(nameof(FanDiagnosticsView_CurrentState));
    public static string FanDiagnosticsView_Evidence => Get(nameof(FanDiagnosticsView_Evidence));
    public static string FanDiagnosticsView_GuidedFanVerification => Get(nameof(FanDiagnosticsView_GuidedFanVerification));
    public static string FanDiagnosticsView_History => Get(nameof(FanDiagnosticsView_History));
    public static string FanDiagnosticsView_Level => Get(nameof(FanDiagnosticsView_Level));
    public static string FanDiagnosticsView_Readback => Get(nameof(FanDiagnosticsView_Readback));
    public static string FanDiagnosticsView_Requested => Get(nameof(FanDiagnosticsView_Requested));
    public static string FanDiagnosticsView_Rpm => Get(nameof(FanDiagnosticsView_Rpm));
    public static string FanDiagnosticsView_RunGuidedFanVerification => Get(nameof(FanDiagnosticsView_RunGuidedFanVerification));
    public static string FanDiagnosticsView_SelectedFan => Get(nameof(FanDiagnosticsView_SelectedFan));
    public static string FanDiagnosticsView_SourceOfRpmDataEcDirectEcReadHwmonLibrehar => Get(nameof(FanDiagnosticsView_SourceOfRpmDataEcDirectEcReadHwmonLibrehar));
    public static string FanDiagnosticsView_TargetPercent => Get(nameof(FanDiagnosticsView_TargetPercent));
    public static string FanDiagnosticsView_Tests3060100 => Get(nameof(FanDiagnosticsView_Tests3060100));
    public static string FanDiagnosticsView_VerificationResults => Get(nameof(FanDiagnosticsView_VerificationResults));

    // Auto-extracted literals from GameLibraryView.xaml
    public static string GameLibraryView_AddGame => Get(nameof(GameLibraryView_AddGame));
    public static string GameLibraryView_AutomaticallyDetectedGamesFromSteamEpicGog => Get(nameof(GameLibraryView_AutomaticallyDetectedGamesFromSteamEpicGog));
    public static string GameLibraryView_CreatePerformanceProfileForSelectedGame => Get(nameof(GameLibraryView_CreatePerformanceProfileForSelectedGame));
    public static string GameLibraryView_CreateProfile => Get(nameof(GameLibraryView_CreateProfile));
    public static string GameLibraryView_EditExistingProfile => Get(nameof(GameLibraryView_EditExistingProfile));
    public static string GameLibraryView_EditProfile => Get(nameof(GameLibraryView_EditProfile));
    public static string GameLibraryView_GameLibrary => Get(nameof(GameLibraryView_GameLibrary));
    public static string GameLibraryView_LaunchSelectedGame => Get(nameof(GameLibraryView_LaunchSelectedGame));
    public static string GameLibraryView_ManuallyAddAGameExecutable => Get(nameof(GameLibraryView_ManuallyAddAGameExecutable));
    public static string GameLibraryView_RescanAllGamePlatforms => Get(nameof(GameLibraryView_RescanAllGamePlatforms));
    public static string GameLibraryView_ScanningGameLibraries => Get(nameof(GameLibraryView_ScanningGameLibraries));
    public static string GameLibraryView_SearchGames => Get(nameof(GameLibraryView_SearchGames));

    // Auto-extracted literals from GameProfileManagerView.xaml
    public static string GameProfileManagerView_AdvancedSettings => Get(nameof(GameProfileManagerView_AdvancedSettings));
    public static string GameProfileManagerView_AlsoApplyToOptionalCommaSeparatedExeNames => Get(nameof(GameProfileManagerView_AlsoApplyToOptionalCommaSeparatedExeNames));
    public static string GameProfileManagerView_AutomaticallySwitchSettingsWhenGamesLaunch => Get(nameof(GameProfileManagerView_AutomaticallySwitchSettingsWhenGamesLaunch));
    public static string GameProfileManagerView_Browse => Get(nameof(GameProfileManagerView_Browse));
    public static string GameProfileManagerView_CleanMemoryRamWhenThisGameLaunches => Get(nameof(GameProfileManagerView_CleanMemoryRamWhenThisGameLaunches));
    public static string GameProfileManagerView_CpuCoreUndervoltMv => Get(nameof(GameProfileManagerView_CpuCoreUndervoltMv));
    public static string GameProfileManagerView_Created => Get(nameof(GameProfileManagerView_Created));
    public static string GameProfileManagerView_Duplicate => Get(nameof(GameProfileManagerView_Duplicate));
    public static string GameProfileManagerView_EnableAutoSwitch => Get(nameof(GameProfileManagerView_EnableAutoSwitch));
    public static string GameProfileManagerView_ExecutableNameEGRocketleagueExe => Get(nameof(GameProfileManagerView_ExecutableNameEGRocketleagueExe));
    public static string GameProfileManagerView_ExecutablePathOptionalForSpecificInstalls => Get(nameof(GameProfileManagerView_ExecutablePathOptionalForSpecificInstalls));
    public static string GameProfileManagerView_ExportProfiles => Get(nameof(GameProfileManagerView_ExportProfiles));
    public static string GameProfileManagerView_FanPreset => Get(nameof(GameProfileManagerView_FanPreset));
    public static string GameProfileManagerView_GameProfileManager => Get(nameof(GameProfileManagerView_GameProfileManager));
    public static string GameProfileManagerView_GpuMode => Get(nameof(GameProfileManagerView_GpuMode));
    public static string GameProfileManagerView_ImportProfiles => Get(nameof(GameProfileManagerView_ImportProfiles));
    public static string GameProfileManagerView_KeyboardLightingProfile => Get(nameof(GameProfileManagerView_KeyboardLightingProfile));
    public static string GameProfileManagerView_LaunchCount => Get(nameof(GameProfileManagerView_LaunchCount));
    public static string GameProfileManagerView_LightingSettings => Get(nameof(GameProfileManagerView_LightingSettings));
    public static string GameProfileManagerView_Modified => Get(nameof(GameProfileManagerView_Modified));
    public static string GameProfileManagerView_NewProfile => Get(nameof(GameProfileManagerView_NewProfile));
    public static string GameProfileManagerView_OtherGamesThatShouldShareThisProfileSSetti => Get(nameof(GameProfileManagerView_OtherGamesThatShouldShareThisProfileSSetti));
    public static string GameProfileManagerView_PerformanceSettings => Get(nameof(GameProfileManagerView_PerformanceSettings));
    public static string GameProfileManagerView_PeripheralLightingProfile => Get(nameof(GameProfileManagerView_PeripheralLightingProfile));
    public static string GameProfileManagerView_PriorityExactExecutablePathsWinFirstHigher => Get(nameof(GameProfileManagerView_PriorityExactExecutablePathsWinFirstHigher));
    public static string GameProfileManagerView_ProfileName => Get(nameof(GameProfileManagerView_ProfileName));
    public static string GameProfileManagerView_ProfileSettings => Get(nameof(GameProfileManagerView_ProfileSettings));
    public static string GameProfileManagerView_RestoreDefaultFanPerformanceSettingsWhenGa => Get(nameof(GameProfileManagerView_RestoreDefaultFanPerformanceSettingsWhenGa));
    public static string GameProfileManagerView_SaveClose => Get(nameof(GameProfileManagerView_SaveClose));
    public static string GameProfileManagerView_SearchProfiles => Get(nameof(GameProfileManagerView_SearchProfiles));
    public static string GameProfileManagerView_Statistics => Get(nameof(GameProfileManagerView_Statistics));
    public static string GameProfileManagerView_TotalPlaytime => Get(nameof(GameProfileManagerView_TotalPlaytime));

    // Auto-extracted literals from GeneralView.xaml
    public static string GeneralView_Active => Get(nameof(GeneralView_Active));
    public static string GeneralView_AdvancedControl => Get(nameof(GeneralView_AdvancedControl));
    public static string GeneralView_AutoFans => Get(nameof(GeneralView_AutoFans));
    public static string GeneralView_BalancedProfileDefaultHpPowerAndThermals => Get(nameof(GeneralView_BalancedProfileDefaultHpPowerAndThermals));
    public static string GeneralView_ClickToActivate => Get(nameof(GeneralView_ClickToActivate));
    public static string GeneralView_CustomProfileUserDefinedFanCurvesAndPowerS => Get(nameof(GeneralView_CustomProfileUserDefinedFanCurvesAndPowerS));
    public static string GeneralView_CustomTab => Get(nameof(GeneralView_CustomTab));
    public static string GeneralView_Default => Get(nameof(GeneralView_Default));
    public static string GeneralView_DefaultHpProfileBalancedPowerAndThermalsFo => Get(nameof(GeneralView_DefaultHpProfileBalancedPowerAndThermalsFo));
    public static string GeneralView_Fan => Get(nameof(GeneralView_Fan));
    public static string GeneralView_FanModeCustom => Get(nameof(GeneralView_FanModeCustom));
    public static string GeneralView_FanModeMax => Get(nameof(GeneralView_FanModeMax));
    public static string GeneralView_FanModeProfileControlledAutomatic => Get(nameof(GeneralView_FanModeProfileControlledAutomatic));
    public static string GeneralView_Fans => Get(nameof(GeneralView_Fans));
    public static string GeneralView_FansFollowTheActivePerformanceProfileBiosA => Get(nameof(GeneralView_FansFollowTheActivePerformanceProfileBiosA));
    public static string GeneralView_FansRunAtMaximumSpeedRegardlessOfTheActive => Get(nameof(GeneralView_FansRunAtMaximumSpeedRegardlessOfTheActive));
    public static string GeneralView_LowFans => Get(nameof(GeneralView_LowFans));
    public static string GeneralView_MaximumCpuGpuPowerLimitsAndAggressiveFanCu => Get(nameof(GeneralView_MaximumCpuGpuPowerLimitsAndAggressiveFanCu));
    public static string GeneralView_MaximumOutput => Get(nameof(GeneralView_MaximumOutput));
    public static string GeneralView_Mode => Get(nameof(GeneralView_Mode));
    public static string GeneralView_OpenTheCustomTabToSetAManualFanCurve => Get(nameof(GeneralView_OpenTheCustomTabToSetAManualFanCurve));
    public static string GeneralView_PerformanceCooling => Get(nameof(GeneralView_PerformanceCooling));
    public static string GeneralView_PerformanceProfileMaximumCpuGpuPowerAndAgg => Get(nameof(GeneralView_PerformanceProfileMaximumCpuGpuPowerAndAgg));
    public static string GeneralView_PowerSave => Get(nameof(GeneralView_PowerSave));
    public static string GeneralView_ProfileControlledAuto => Get(nameof(GeneralView_ProfileControlledAuto));
    public static string GeneralView_QuickProfiles => Get(nameof(GeneralView_QuickProfiles));
    public static string GeneralView_QuickProfileSelectionForOptimalSystemTunin => Get(nameof(GeneralView_QuickProfileSelectionForOptimalSystemTunin));
    public static string GeneralView_QuietProfileReducedPowerAndMinimalFanSpeed => Get(nameof(GeneralView_QuietProfileReducedPowerAndMinimalFanSpeed));
    public static string GeneralView_ReducedPowerLimitsAndMinimalFanSpeedForSil => Get(nameof(GeneralView_ReducedPowerLimitsAndMinimalFanSpeedForSil));
    public static string GeneralView_TemperatureCriticalFansRunningAtMaxQuietCu => Get(nameof(GeneralView_TemperatureCriticalFansRunningAtMaxQuietCu));
    public static string GeneralView_ThermalSafetyActive => Get(nameof(GeneralView_ThermalSafetyActive));
    public static string GeneralView_UserDefinedFanCurvesAndPowerSettingsViaThe => Get(nameof(GeneralView_UserDefinedFanCurvesAndPowerSettingsViaThe));

    // Auto-extracted literals from GpuVcChart.xaml
    public static string GpuVcChart_GpuCurrent => Get(nameof(GpuVcChart_GpuCurrent));
    public static string GpuVcChart_GpuVoltage => Get(nameof(GpuVcChart_GpuVoltage));

    // Auto-extracted literals from HardwareMonitoringDashboard.xaml
    public static string HardwareMonitoringDashboard_ActiveAlerts => Get(nameof(HardwareMonitoringDashboard_ActiveAlerts));
    public static string HardwareMonitoringDashboard_AverageCoreClock => Get(nameof(HardwareMonitoringDashboard_AverageCoreClock));
    public static string HardwareMonitoringDashboard_BatteryCapacity => Get(nameof(HardwareMonitoringDashboard_BatteryCapacity));
    public static string HardwareMonitoringDashboard_BatteryCharge => Get(nameof(HardwareMonitoringDashboard_BatteryCharge));
    public static string HardwareMonitoringDashboard_BatteryStatus => Get(nameof(HardwareMonitoringDashboard_BatteryStatus));
    public static string HardwareMonitoringDashboard_Condition => Get(nameof(HardwareMonitoringDashboard_Condition));
    public static string HardwareMonitoringDashboard_CpuClock => Get(nameof(HardwareMonitoringDashboard_CpuClock));
    public static string HardwareMonitoringDashboard_CpuLoad => Get(nameof(HardwareMonitoringDashboard_CpuLoad));
    public static string HardwareMonitoringDashboard_CpuTemperature => Get(nameof(HardwareMonitoringDashboard_CpuTemperature));
    public static string HardwareMonitoringDashboard_CurrentMetrics => Get(nameof(HardwareMonitoringDashboard_CurrentMetrics));
    public static string HardwareMonitoringDashboard_EfficiencyMetrics => Get(nameof(HardwareMonitoringDashboard_EfficiencyMetrics));
    public static string HardwareMonitoringDashboard_ExportData => Get(nameof(HardwareMonitoringDashboard_ExportData));
    public static string HardwareMonitoringDashboard_ExportSensorHistoryToACsvFile => Get(nameof(HardwareMonitoringDashboard_ExportSensorHistoryToACsvFile));
    public static string HardwareMonitoringDashboard_Fan68Efficient => Get(nameof(HardwareMonitoringDashboard_Fan68Efficient));
    public static string HardwareMonitoringDashboard_FanSpeeds => Get(nameof(HardwareMonitoringDashboard_FanSpeeds));
    public static string HardwareMonitoringDashboard_GpuLoad => Get(nameof(HardwareMonitoringDashboard_GpuLoad));
    public static string HardwareMonitoringDashboard_GpuTemperature => Get(nameof(HardwareMonitoringDashboard_GpuTemperature));
    public static string HardwareMonitoringDashboard_HardwareMonitoringDashboard => Get(nameof(HardwareMonitoringDashboard_HardwareMonitoringDashboard));
    public static string HardwareMonitoringDashboard_HistoricalTrends => Get(nameof(HardwareMonitoringDashboard_HistoricalTrends));
    public static string HardwareMonitoringDashboard_N30YearsRemaining => Get(nameof(HardwareMonitoringDashboard_N30YearsRemaining));
    public static string HardwareMonitoringDashboard_NoActiveAlerts => Get(nameof(HardwareMonitoringDashboard_NoActiveAlerts));
    public static string HardwareMonitoringDashboard_Optimal => Get(nameof(HardwareMonitoringDashboard_Optimal));
    public static string HardwareMonitoringDashboard_PowerConsumption => Get(nameof(HardwareMonitoringDashboard_PowerConsumption));
    public static string HardwareMonitoringDashboard_PowerConsumption_2 => Get(nameof(HardwareMonitoringDashboard_PowerConsumption_2));
    public static string HardwareMonitoringDashboard_PowerEfficiency => Get(nameof(HardwareMonitoringDashboard_PowerEfficiency));
    public static string HardwareMonitoringDashboard_RamUsage => Get(nameof(HardwareMonitoringDashboard_RamUsage));
    public static string HardwareMonitoringDashboard_RealTimeMonitoringOfPowerConsumptionBatter => Get(nameof(HardwareMonitoringDashboard_RealTimeMonitoringOfPowerConsumptionBatter));
    public static string HardwareMonitoringDashboard_RefreshAllHardwareMonitoringCharts => Get(nameof(HardwareMonitoringDashboard_RefreshAllHardwareMonitoringCharts));
    public static string HardwareMonitoringDashboard_RefreshCharts => Get(nameof(HardwareMonitoringDashboard_RefreshCharts));
    public static string HardwareMonitoringDashboard_Stable => Get(nameof(HardwareMonitoringDashboard_Stable));
    public static string HardwareMonitoringDashboard_SystemActivity => Get(nameof(HardwareMonitoringDashboard_SystemActivity));
    public static string HardwareMonitoringDashboard_SystemAlerts => Get(nameof(HardwareMonitoringDashboard_SystemAlerts));
    public static string HardwareMonitoringDashboard_Temperature => Get(nameof(HardwareMonitoringDashboard_Temperature));
    public static string HardwareMonitoringDashboard_ThermalStatus => Get(nameof(HardwareMonitoringDashboard_ThermalStatus));
    public static string HardwareMonitoringDashboard_Unavailable => Get(nameof(HardwareMonitoringDashboard_Unavailable));
    public static string HardwareMonitoringDashboard_Used => Get(nameof(HardwareMonitoringDashboard_Used));

    // Auto-extracted literals from HotkeyOsdWindow.xaml
    public static string HotkeyOsdWindow_OmencoreOsd => Get(nameof(HotkeyOsdWindow_OmencoreOsd));
    public static string HotkeyOsdWindow_ViaHotkeyCtrlShiftF => Get(nameof(HotkeyOsdWindow_ViaHotkeyCtrlShiftF));
    public static string HotkeyOsdWindow_Widthandheight => Get(nameof(HotkeyOsdWindow_Widthandheight));

    // Auto-extracted literals from KeyboardDiagnosticsView.xaml
    public static string KeyboardDiagnosticsView_CheckDeviceConnectionsAndTryRePluggingUsbD => Get(nameof(KeyboardDiagnosticsView_CheckDeviceConnectionsAndTryRePluggingUsbD));
    public static string KeyboardDiagnosticsView_CheckWindowsEventViewerForSdkInitializatio => Get(nameof(KeyboardDiagnosticsView_CheckWindowsEventViewerForSdkInitializatio));
    public static string KeyboardDiagnosticsView_ClearTestPattern => Get(nameof(KeyboardDiagnosticsView_ClearTestPattern));
    public static string KeyboardDiagnosticsView_CollectLogs => Get(nameof(KeyboardDiagnosticsView_CollectLogs));
    public static string KeyboardDiagnosticsView_CorsairIcue => Get(nameof(KeyboardDiagnosticsView_CorsairIcue));
    public static string KeyboardDiagnosticsView_DetectDevices => Get(nameof(KeyboardDiagnosticsView_DetectDevices));
    public static string KeyboardDiagnosticsView_DetectedDevices => Get(nameof(KeyboardDiagnosticsView_DetectedDevices));
    public static string KeyboardDiagnosticsView_DiagnosticLogs => Get(nameof(KeyboardDiagnosticsView_DiagnosticLogs));
    public static string KeyboardDiagnosticsView_EnsureIcueGHubOrSynapseSoftwareIsRunningFo => Get(nameof(KeyboardDiagnosticsView_EnsureIcueGHubOrSynapseSoftwareIsRunningFo));
    public static string KeyboardDiagnosticsView_ExperimentalTries5ColourCommandLayoutsForK => Get(nameof(KeyboardDiagnosticsView_ExperimentalTries5ColourCommandLayoutsForK));
    public static string KeyboardDiagnosticsView_ForHpOmenKeyboardEnsureWmiBiosSupportIsAva => Get(nameof(KeyboardDiagnosticsView_ForHpOmenKeyboardEnsureWmiBiosSupportIsAva));
    public static string KeyboardDiagnosticsView_HpOmenKeyboard => Get(nameof(KeyboardDiagnosticsView_HpOmenKeyboard));
    public static string KeyboardDiagnosticsView_KeyboardLightingTelemetry => Get(nameof(KeyboardDiagnosticsView_KeyboardLightingTelemetry));
    public static string KeyboardDiagnosticsView_LogitechGHub => Get(nameof(KeyboardDiagnosticsView_LogitechGHub));
    public static string KeyboardDiagnosticsView_RazerChroma => Get(nameof(KeyboardDiagnosticsView_RazerChroma));
    public static string KeyboardDiagnosticsView_RestartTheApplicationAfterInstallingUpdati => Get(nameof(KeyboardDiagnosticsView_RestartTheApplicationAfterInstallingUpdati));
    public static string KeyboardDiagnosticsView_RgbPayloadProbeAdvanced => Get(nameof(KeyboardDiagnosticsView_RgbPayloadProbeAdvanced));
    public static string KeyboardDiagnosticsView_RunTestPattern => Get(nameof(KeyboardDiagnosticsView_RunTestPattern));
    public static string KeyboardDiagnosticsView_ServiceStatus => Get(nameof(KeyboardDiagnosticsView_ServiceStatus));
    public static string KeyboardDiagnosticsView_TroubleshootingTips => Get(nameof(KeyboardDiagnosticsView_TroubleshootingTips));

    // Auto-extracted literals from KeyboardMapEditor.xaml
    public static string KeyboardMapEditor_AllBlack => Get(nameof(KeyboardMapEditor_AllBlack));
    public static string KeyboardMapEditor_AppliesThePictureThenStoresItInTheKeyboard => Get(nameof(KeyboardMapEditor_AppliesThePictureThenStoresItInTheKeyboard));
    public static string KeyboardMapEditor_ApplyPerKeyColoursToTheKeyboard => Get(nameof(KeyboardMapEditor_ApplyPerKeyColoursToTheKeyboard));
    public static string KeyboardMapEditor_ApplyToKeyboard => Get(nameof(KeyboardMapEditor_ApplyToKeyboard));
    public static string KeyboardMapEditor_BrightnessOfTheSelectedKeysAndLightBarZone => Get(nameof(KeyboardMapEditor_BrightnessOfTheSelectedKeysAndLightBarZone));
    public static string KeyboardMapEditor_Brush => Get(nameof(KeyboardMapEditor_Brush));
    public static string KeyboardMapEditor_ChooseAColour => Get(nameof(KeyboardMapEditor_ChooseAColour));
    public static string KeyboardMapEditor_ChooseBrushColour => Get(nameof(KeyboardMapEditor_ChooseBrushColour));
    public static string KeyboardMapEditor_ClearSelection => Get(nameof(KeyboardMapEditor_ClearSelection));
    public static string KeyboardMapEditor_ClickAKeyToSelectOrDeselectItDragABoxToSel => Get(nameof(KeyboardMapEditor_ClickAKeyToSelectOrDeselectItDragABoxToSel));
    public static string KeyboardMapEditor_MasterBrightnessForTheWholeKeyboardAndLigh => Get(nameof(KeyboardMapEditor_MasterBrightnessForTheWholeKeyboardAndLigh));
    public static string KeyboardMapEditor_OpenDynamicLightingSettings => Get(nameof(KeyboardMapEditor_OpenDynamicLightingSettings));
    public static string KeyboardMapEditor_OpenTheWindowsDynamicLightingSettingsPage => Get(nameof(KeyboardMapEditor_OpenTheWindowsDynamicLightingSettingsPage));
    public static string KeyboardMapEditor_PaintSelected => Get(nameof(KeyboardMapEditor_PaintSelected));
    public static string KeyboardMapEditor_PickColour => Get(nameof(KeyboardMapEditor_PickColour));
    public static string KeyboardMapEditor_SaveThePictureIntoTheKeyboardSoItSurvivesA => Get(nameof(KeyboardMapEditor_SaveThePictureIntoTheKeyboardSoItSurvivesA));
    public static string KeyboardMapEditor_SaveToKeyboard => Get(nameof(KeyboardMapEditor_SaveToKeyboard));
    public static string KeyboardMapEditor_SelectAll => Get(nameof(KeyboardMapEditor_SelectAll));
    public static string KeyboardMapEditor_Selected => Get(nameof(KeyboardMapEditor_Selected));
    public static string KeyboardMapEditor_WholeKeyboard => Get(nameof(KeyboardMapEditor_WholeKeyboard));

    // Auto-extracted literals from LightingView.xaml
    public static string LightingView_ActiveProfile => Get(nameof(LightingView_ActiveProfile));
    public static string LightingView_AddToCycle => Get(nameof(LightingView_AddToCycle));
    public static string LightingView_AdvancedLightingEffects => Get(nameof(LightingView_AdvancedLightingEffects));
    public static string LightingView_AllSameColor => Get(nameof(LightingView_AllSameColor));
    public static string LightingView_AmbientScreenLighting => Get(nameof(LightingView_AmbientScreenLighting));
    public static string LightingView_ApplyASafeBrightProbeColorThroughTheActive => Get(nameof(LightingView_ApplyASafeBrightProbeColorThroughTheActive));
    public static string LightingView_ApplyColor => Get(nameof(LightingView_ApplyColor));
    public static string LightingView_ApplyColorsOnStartup => Get(nameof(LightingView_ApplyColorsOnStartup));
    public static string LightingView_ApplyColour => Get(nameof(LightingView_ApplyColour));
    public static string LightingView_ApplyDpiSettings => Get(nameof(LightingView_ApplyDpiSettings));
    public static string LightingView_ApplyEffect => Get(nameof(LightingView_ApplyEffect));
    public static string LightingView_ApplyKeyboardColorsOnStartup => Get(nameof(LightingView_ApplyKeyboardColorsOnStartup));
    public static string LightingView_ApplyPerKeyColorsToKeyboard => Get(nameof(LightingView_ApplyPerKeyColorsToKeyboard));
    public static string LightingView_ApplyProfile => Get(nameof(LightingView_ApplyProfile));
    public static string LightingView_ApplySelectedDpiProfileToCurrentStages => Get(nameof(LightingView_ApplySelectedDpiProfileToCurrentStages));
    public static string LightingView_ApplyStatic => Get(nameof(LightingView_ApplyStatic));
    public static string LightingView_ApplyToAllSystemRgb => Get(nameof(LightingView_ApplyToAllSystemRgb));
    public static string LightingView_ApplyToSystem => Get(nameof(LightingView_ApplyToSystem));
    public static string LightingView_ApplyZone1ColorToAllZones => Get(nameof(LightingView_ApplyZone1ColorToAllZones));
    public static string LightingView_ApplyZoneColorsToKeyboard => Get(nameof(LightingView_ApplyZoneColorsToKeyboard));
    public static string LightingView_AudioReactiveLighting => Get(nameof(LightingView_AudioReactiveLighting));
    public static string LightingView_AudioReactiveSensitivity => Get(nameof(LightingView_AudioReactiveSensitivity));
    public static string LightingView_AudioReactiveVisualizationMode => Get(nameof(LightingView_AudioReactiveVisualizationMode));
    public static string LightingView_AutomaticallyRestoreSavedKeyboardColorsWhe => Get(nameof(LightingView_AutomaticallyRestoreSavedKeyboardColorsWhe));
    public static string LightingView_Breathing => Get(nameof(LightingView_Breathing));
    public static string LightingView_Brightness => Get(nameof(LightingView_Brightness));
    public static string LightingView_ChooseTheFirstColour => Get(nameof(LightingView_ChooseTheFirstColour));
    public static string LightingView_ChooseTheFirstCustomEffectColour => Get(nameof(LightingView_ChooseTheFirstCustomEffectColour));
    public static string LightingView_ChooseTheLightBarColour => Get(nameof(LightingView_ChooseTheLightBarColour));
    public static string LightingView_ChooseTheSecondColour => Get(nameof(LightingView_ChooseTheSecondColour));
    public static string LightingView_ChooseTheSecondCustomEffectColour => Get(nameof(LightingView_ChooseTheSecondCustomEffectColour));
    public static string LightingView_Color => Get(nameof(LightingView_Color));
    public static string LightingView_ColorHex => Get(nameof(LightingView_ColorHex));
    public static string LightingView_ControlOwnership => Get(nameof(LightingView_ControlOwnership));
    public static string LightingView_ControlsAnyRgbDeviceSupportedByOpenrgbMoth => Get(nameof(LightingView_ControlsAnyRgbDeviceSupportedByOpenrgbMoth));
    public static string LightingView_CorsairDevices => Get(nameof(LightingView_CorsairDevices));
    public static string LightingView_CustomColours => Get(nameof(LightingView_CustomColours));
    public static string LightingView_CustomRgbColor => Get(nameof(LightingView_CustomRgbColor));
    public static string LightingView_DeleteTheSelectedDpiProfile => Get(nameof(LightingView_DeleteTheSelectedDpiProfile));
    public static string LightingView_Direction => Get(nameof(LightingView_Direction));
    public static string LightingView_DiscoverDevices => Get(nameof(LightingView_DiscoverDevices));
    public static string LightingView_DownloadOpenrgbFromOpenrgbOrgAndEnableTheS => Get(nameof(LightingView_DownloadOpenrgbFromOpenrgbOrgAndEnableTheS));
    public static string LightingView_DpiConfiguration => Get(nameof(LightingView_DpiConfiguration));
    public static string LightingView_DpiStages => Get(nameof(LightingView_DpiStages));
    public static string LightingView_EnableCorsairDevicesCard => Get(nameof(LightingView_EnableCorsairDevicesCard));
    public static string LightingView_EnableHpOmenKeyboardCard => Get(nameof(LightingView_EnableHpOmenKeyboardCard));
    public static string LightingView_EnableLogitechDevicesCard => Get(nameof(LightingView_EnableLogitechDevicesCard));
    public static string LightingView_EnableOrDisableCorsairControlsInThisCard => Get(nameof(LightingView_EnableOrDisableCorsairControlsInThisCard));
    public static string LightingView_EnableOrDisableHpOmenKeyboardControlsInThi => Get(nameof(LightingView_EnableOrDisableHpOmenKeyboardControlsInThi));
    public static string LightingView_EnableOrDisableLogitechControlsInThisCard => Get(nameof(LightingView_EnableOrDisableLogitechControlsInThisCard));
    public static string LightingView_EnableOrDisableRazerControlsInThisCard => Get(nameof(LightingView_EnableOrDisableRazerControlsInThisCard));
    public static string LightingView_EnableOrDisableSceneQuickSelectControlsInT => Get(nameof(LightingView_EnableOrDisableSceneQuickSelectControlsInT));
    public static string LightingView_EnablePerformanceModeSyncedLighting => Get(nameof(LightingView_EnablePerformanceModeSyncedLighting));
    public static string LightingView_EnableRazerDevicesCard => Get(nameof(LightingView_EnableRazerDevicesCard));
    public static string LightingView_EnableSceneQuickSelectCard => Get(nameof(LightingView_EnableSceneQuickSelectCard));
    public static string LightingView_EnableTemperatureResponsiveLighting => Get(nameof(LightingView_EnableTemperatureResponsiveLighting));
    public static string LightingView_EnableThrottlingIndicatorLighting => Get(nameof(LightingView_EnableThrottlingIndicatorLighting));
    public static string LightingView_FillAll => Get(nameof(LightingView_FillAll));
    public static string LightingView_FlashDeviceLedsToIdentifyThisDevice => Get(nameof(LightingView_FlashDeviceLedsToIdentifyThisDevice));
    public static string LightingView_FlashRedOrangeWhenCpuGpuThermalOrPowerThro => Get(nameof(LightingView_FlashRedOrangeWhenCpuGpuThermalOrPowerThro));
    public static string LightingView_Fn1Fn2Cycle => Get(nameof(LightingView_Fn1Fn2Cycle));
    public static string LightingView_HighTemp => Get(nameof(LightingView_HighTemp));
    public static string LightingView_HpOmenKeyboard => Get(nameof(LightingView_HpOmenKeyboard));
    public static string LightingView_Identify => Get(nameof(LightingView_Identify));
    public static string LightingView_KeyboardEffects => Get(nameof(LightingView_KeyboardEffects));
    public static string LightingView_KeyboardLightBarAndRgbDevicesChangeColorBa => Get(nameof(LightingView_KeyboardLightBarAndRgbDevicesChangeColorBa));
    public static string LightingView_KeyboardLightBarAndRgbDevicesChangeColorBa_2 => Get(nameof(LightingView_KeyboardLightBarAndRgbDevicesChangeColorBa_2));
    public static string LightingView_KeyboardLightingPresetSelection => Get(nameof(LightingView_KeyboardLightingPresetSelection));
    public static string LightingView_KeyboardQuickColorBlue => Get(nameof(LightingView_KeyboardQuickColorBlue));
    public static string LightingView_KeyboardQuickColorCyan => Get(nameof(LightingView_KeyboardQuickColorCyan));
    public static string LightingView_KeyboardQuickColorGreen => Get(nameof(LightingView_KeyboardQuickColorGreen));
    public static string LightingView_KeyboardQuickColorOff => Get(nameof(LightingView_KeyboardQuickColorOff));
    public static string LightingView_KeyboardQuickColorOmenRed => Get(nameof(LightingView_KeyboardQuickColorOmenRed));
    public static string LightingView_KeyboardQuickColorPink => Get(nameof(LightingView_KeyboardQuickColorPink));
    public static string LightingView_KeyboardQuickColorPurple => Get(nameof(LightingView_KeyboardQuickColorPurple));
    public static string LightingView_KeyboardQuickColorWhite => Get(nameof(LightingView_KeyboardQuickColorWhite));
    public static string LightingView_KeyboardQuickColorYellow => Get(nameof(LightingView_KeyboardQuickColorYellow));
    public static string LightingView_KeyboardZonesClickZoneToChangeColor => Get(nameof(LightingView_KeyboardZonesClickZoneToChangeColor));
    public static string LightingView_LeaveTheKeyboardShowingThisOneAfterWriting => Get(nameof(LightingView_LeaveTheKeyboardShowingThisOneAfterWriting));
    public static string LightingView_LightingControl => Get(nameof(LightingView_LightingControl));
    public static string LightingView_LightingPreset => Get(nameof(LightingView_LightingPreset));
    public static string LightingView_LoadProfile => Get(nameof(LightingView_LoadProfile));
    public static string LightingView_LogitechColorBlueValue => Get(nameof(LightingView_LogitechColorBlueValue));
    public static string LightingView_LogitechColorGreenValue => Get(nameof(LightingView_LogitechColorGreenValue));
    public static string LightingView_LogitechColorRedValue => Get(nameof(LightingView_LogitechColorRedValue));
    public static string LightingView_LogitechDevices => Get(nameof(LightingView_LogitechDevices));
    public static string LightingView_LowTemp => Get(nameof(LightingView_LowTemp));
    public static string LightingView_MacroProfiles => Get(nameof(LightingView_MacroProfiles));
    public static string LightingView_MacroProfileSelection => Get(nameof(LightingView_MacroProfileSelection));
    public static string LightingView_MeasuredLayoutEveryKeyAtThePositionTheKeyb => Get(nameof(LightingView_MeasuredLayoutEveryKeyAtThePositionTheKeyb));
    public static string LightingView_MediumTemp => Get(nameof(LightingView_MediumTemp));
    public static string LightingView_ObservedRgbSurface => Get(nameof(LightingView_ObservedRgbSurface));
    public static string LightingView_OmenKeyboard => Get(nameof(LightingView_OmenKeyboard));
    public static string LightingView_OmenMax => Get(nameof(LightingView_OmenMax));
    public static string LightingView_OmenRed => Get(nameof(LightingView_OmenRed));
    public static string LightingView_OpenrgbDevices => Get(nameof(LightingView_OpenrgbDevices));
    public static string LightingView_OpenrgbIntegration => Get(nameof(LightingView_OpenrgbIntegration));
    public static string LightingView_OpenrgbQuickColorBlue => Get(nameof(LightingView_OpenrgbQuickColorBlue));
    public static string LightingView_OpenrgbQuickColorGreen => Get(nameof(LightingView_OpenrgbQuickColorGreen));
    public static string LightingView_OpenrgbQuickColorOff => Get(nameof(LightingView_OpenrgbQuickColorOff));
    public static string LightingView_OpenrgbQuickColorOrange => Get(nameof(LightingView_OpenrgbQuickColorOrange));
    public static string LightingView_OpenrgbQuickColorPurple => Get(nameof(LightingView_OpenrgbQuickColorPurple));
    public static string LightingView_OpenrgbQuickColorRed => Get(nameof(LightingView_OpenrgbQuickColorRed));
    public static string LightingView_OpenrgbQuickColorWhite => Get(nameof(LightingView_OpenrgbQuickColorWhite));
    public static string LightingView_OpenrgbQuickColorYellow => Get(nameof(LightingView_OpenrgbQuickColorYellow));
    public static string LightingView_OptionalGenericRgbControlViaOpenrgbServerF => Get(nameof(LightingView_OptionalGenericRgbControlViaOpenrgbServerF));
    public static string LightingView_PerformanceModeColors => Get(nameof(LightingView_PerformanceModeColors));
    public static string LightingView_PerformanceModeSyncedLighting => Get(nameof(LightingView_PerformanceModeSyncedLighting));
    public static string LightingView_Preset => Get(nameof(LightingView_Preset));
    public static string LightingView_PulsesWithSystemAudioOutputViaWasapiLoopba => Get(nameof(LightingView_PulsesWithSystemAudioOutputViaWasapiLoopba));
    public static string LightingView_QuickColors => Get(nameof(LightingView_QuickColors));
    public static string LightingView_QuickColorsClickToApplyToAllZones => Get(nameof(LightingView_QuickColorsClickToApplyToAllZones));
    public static string LightingView_RazerColorBlueValue => Get(nameof(LightingView_RazerColorBlueValue));
    public static string LightingView_RazerColorGreenValue => Get(nameof(LightingView_RazerColorGreenValue));
    public static string LightingView_RazerColorRedValue => Get(nameof(LightingView_RazerColorRedValue));
    public static string LightingView_RazerDevices => Get(nameof(LightingView_RazerDevices));
    public static string LightingView_RazerSupportIsLimitedToDetectedDeviceStatu => Get(nameof(LightingView_RazerSupportIsLimitedToDetectedDeviceStatu));
    public static string LightingView_ReapplySavedKeyboardColorsThroughTheActive => Get(nameof(LightingView_ReapplySavedKeyboardColorsThroughTheActive));
    public static string LightingView_RecordThePhysicalRgbOrBacklightSurfaceThat => Get(nameof(LightingView_RecordThePhysicalRgbOrBacklightSurfaceThat));
    public static string LightingView_RequiresRazerSynapseToBeInstalledAndRunnin => Get(nameof(LightingView_RequiresRazerSynapseToBeInstalledAndRunnin));
    public static string LightingView_RestoreDefaults => Get(nameof(LightingView_RestoreDefaults));
    public static string LightingView_RestoreKeyboard => Get(nameof(LightingView_RestoreKeyboard));
    public static string LightingView_RestoreKeyboardLighting => Get(nameof(LightingView_RestoreKeyboardLighting));
    public static string LightingView_RgbLightingPeripherals => Get(nameof(LightingView_RgbLightingPeripherals));
    public static string LightingView_RgbSurfaceObservation => Get(nameof(LightingView_RgbSurfaceObservation));
    public static string LightingView_RgbSyncsWithScreenEdgeColors => Get(nameof(LightingView_RgbSyncsWithScreenEdgeColors));
    public static string LightingView_RunAnimation => Get(nameof(LightingView_RunAnimation));
    public static string LightingView_RunByTheKeyboardSOwnControllerTheseKeepRun => Get(nameof(LightingView_RunByTheKeyboardSOwnControllerTheseKeepRun));
    public static string LightingView_RunRgbSurfaceProbe => Get(nameof(LightingView_RunRgbSurfaceProbe));
    public static string LightingView_SaveCurrentDpiStagesAsAProfile => Get(nameof(LightingView_SaveCurrentDpiStagesAsAProfile));
    public static string LightingView_SavedDpiProfileSelection => Get(nameof(LightingView_SavedDpiProfileSelection));
    public static string LightingView_SaveObservedRgbSurface => Get(nameof(LightingView_SaveObservedRgbSurface));
    public static string LightingView_SaveProfile => Get(nameof(LightingView_SaveProfile));
    public static string LightingView_SaveTheSelectedObservedRgbSurfaceForDiagno => Get(nameof(LightingView_SaveTheSelectedObservedRgbSurfaceForDiagno));
    public static string LightingView_SceneQuickSelect => Get(nameof(LightingView_SceneQuickSelect));
    public static string LightingView_SelectASavedDpiProfileSaveWithSameNameWill => Get(nameof(LightingView_SelectASavedDpiProfileSaveWithSameNameWill));
    public static string LightingView_SendEveryStagedProfileToTheKeyboard => Get(nameof(LightingView_SendEveryStagedProfileToTheKeyboard));
    public static string LightingView_Sensitivity => Get(nameof(LightingView_Sensitivity));
    public static string LightingView_Showing => Get(nameof(LightingView_Showing));
    public static string LightingView_SpectrumCycle => Get(nameof(LightingView_SpectrumCycle));
    public static string LightingView_StageTheEffectColoursSpeedAndDirectionSetA => Get(nameof(LightingView_StageTheEffectColoursSpeedAndDirectionSetA));
    public static string LightingView_StaticColor => Get(nameof(LightingView_StaticColor));
    public static string LightingView_SyncAllRgb => Get(nameof(LightingView_SyncAllRgb));
    public static string LightingView_SyncAllRgbDevices => Get(nameof(LightingView_SyncAllRgbDevices));
    public static string LightingView_SyncColorAcrossAllDetectedLogitechDevices => Get(nameof(LightingView_SyncColorAcrossAllDetectedLogitechDevices));
    public static string LightingView_SyncCurrentColorAcrossAllConnectedRgbDevic => Get(nameof(LightingView_SyncCurrentColorAcrossAllConnectedRgbDevic));
    public static string LightingView_TakeThisOffTheListItStaysOnTheKeyboard => Get(nameof(LightingView_TakeThisOffTheListItStaysOnTheKeyboard));
    public static string LightingView_TemperatureResponsiveLighting => Get(nameof(LightingView_TemperatureResponsiveLighting));
    public static string LightingView_TemperatureThresholdsC => Get(nameof(LightingView_TemperatureThresholdsC));
    public static string LightingView_TheKeyboardRemembersAListOfEffectsAndSteps => Get(nameof(LightingView_TheKeyboardRemembersAListOfEffectsAndSteps));
    public static string LightingView_TheStripOnTheFrontOfTheChassisFourZonesLef => Get(nameof(LightingView_TheStripOnTheFrontOfTheChassisFourZonesLef));
    public static string LightingView_ThisFeatureRequiresASupportedHpOmenLaptopW => Get(nameof(LightingView_ThisFeatureRequiresASupportedHpOmenLaptopW));
    public static string LightingView_ThrottlingAlert => Get(nameof(LightingView_ThrottlingAlert));
    public static string LightingView_ThrottlingIndicators => Get(nameof(LightingView_ThrottlingIndicators));
    public static string LightingView_ThrottlingWarningColor => Get(nameof(LightingView_ThrottlingWarningColor));
    public static string LightingView_UseCorsairIcueOrLogitechGHubSoftwareForAdv => Get(nameof(LightingView_UseCorsairIcueOrLogitechGHubSoftwareForAdv));
    public static string LightingView_WhatSOnItNow => Get(nameof(LightingView_WhatSOnItNow));
    public static string LightingView_WriteToKeyboard => Get(nameof(LightingView_WriteToKeyboard));
    public static string LightingView_YourLaptopSBuiltIn4ZoneRgbKeyboard => Get(nameof(LightingView_YourLaptopSBuiltIn4ZoneRgbKeyboard));

    // Auto-extracted literals from LoadChart.xaml
    public static string LoadChart_CpuUsage => Get(nameof(LoadChart_CpuUsage));
    public static string LoadChart_GpuUsage => Get(nameof(LoadChart_GpuUsage));

    // Auto-extracted literals from LoadingOverlay.xaml
    public static string LoadingOverlay_ThisMayTakeAFewMoments => Get(nameof(LoadingOverlay_ThisMayTakeAFewMoments));

    // Auto-extracted literals from MainWindow.xaml
    public static string MainWindow_ApplyRgbLighting => Get(nameof(MainWindow_ApplyRgbLighting));
    public static string MainWindow_ApplySavedFanPreset => Get(nameof(MainWindow_ApplySavedFanPreset));
    public static string MainWindow_ApplyYourSavedFanPreset => Get(nameof(MainWindow_ApplyYourSavedFanPreset));
    public static string MainWindow_CheckForUpdates => Get(nameof(MainWindow_CheckForUpdates));
    public static string MainWindow_DismissTheStandardProfileNotice => Get(nameof(MainWindow_DismissTheStandardProfileNotice));
    public static string MainWindow_EnableGamingOptimizations => Get(nameof(MainWindow_EnableGamingOptimizations));
    public static string MainWindow_ExportYourSettings => Get(nameof(MainWindow_ExportYourSettings));
    public static string MainWindow_HideThisNoticeForThisLaptop => Get(nameof(MainWindow_HideThisNoticeForThisLaptop));
    public static string MainWindow_ImportSettingsFile => Get(nameof(MainWindow_ImportSettingsFile));
    public static string MainWindow_InstallUpdate => Get(nameof(MainWindow_InstallUpdate));
    public static string MainWindow_ManagePerGameSettings => Get(nameof(MainWindow_ManagePerGameSettings));
    public static string MainWindow_ManuallyCheckForANewVersion => Get(nameof(MainWindow_ManuallyCheckForANewVersion));
    public static string MainWindow_NobodyWithThisExactModelHasConfirmedEveryF => Get(nameof(MainWindow_NobodyWithThisExactModelHasConfirmedEveryF));
    public static string MainWindow_OmencoreIsBuiltForHpOmenAndVictusGamingLap => Get(nameof(MainWindow_OmencoreIsBuiltForHpOmenAndVictusGamingLap));
    public static string MainWindow_Profiles => Get(nameof(MainWindow_Profiles));
    public static string MainWindow_RecentActivity => Get(nameof(MainWindow_RecentActivity));
    public static string MainWindow_ReleaseNotes => Get(nameof(MainWindow_ReleaseNotes));
    public static string MainWindow_RestoreDefaultsResetFanCurvesLightingAndPe => Get(nameof(MainWindow_RestoreDefaultsResetFanCurvesLightingAndPe));
    public static string MainWindow_StandardProfileNotice => Get(nameof(MainWindow_StandardProfileNotice));
    public static string MainWindow_SwitchPerformanceProfile => Get(nameof(MainWindow_SwitchPerformanceProfile));
    public static string MainWindow_SystemLog => Get(nameof(MainWindow_SystemLog));
    public static string MainWindow_UpdateReady => Get(nameof(MainWindow_UpdateReady));
    public static string MainWindow_WarningUnsupportedSystemDetected => Get(nameof(MainWindow_WarningUnsupportedSystemDetected));
    public static string MainWindow_YourLaptopIsSupportedWithItsFamilySStandar => Get(nameof(MainWindow_YourLaptopIsSupportedWithItsFamilySStandar));

    // Auto-extracted literals from MemoryOptimizerView.xaml
    public static string MemoryOptimizerView_AdaptiveProfile => Get(nameof(MemoryOptimizerView_AdaptiveProfile));
    public static string MemoryOptimizerView_AdditionallyPurgesAllStandbyPagesFlushesTh => Get(nameof(MemoryOptimizerView_AdditionallyPurgesAllStandbyPagesFlushesTh));
    public static string MemoryOptimizerView_AddToWorkingSetExclusions => Get(nameof(MemoryOptimizerView_AddToWorkingSetExclusions));
    public static string MemoryOptimizerView_AggressiveCleanAllOperationsIncludingStand => Get(nameof(MemoryOptimizerView_AggressiveCleanAllOperationsIncludingStand));
    public static string MemoryOptimizerView_AutoClean => Get(nameof(MemoryOptimizerView_AutoClean));
    public static string MemoryOptimizerView_AutomaticMemoryCleaning => Get(nameof(MemoryOptimizerView_AutomaticMemoryCleaning));
    public static string MemoryOptimizerView_Available => Get(nameof(MemoryOptimizerView_Available));
    public static string MemoryOptimizerView_CleanInterval => Get(nameof(MemoryOptimizerView_CleanInterval));
    public static string MemoryOptimizerView_CombineMemoryPages => Get(nameof(MemoryOptimizerView_CombineMemoryPages));
    public static string MemoryOptimizerView_CompressedMemory => Get(nameof(MemoryOptimizerView_CompressedMemory));
    public static string MemoryOptimizerView_CopiesTheLatestMemoryCleanSummaryToTheClip => Get(nameof(MemoryOptimizerView_CopiesTheLatestMemoryCleanSummaryToTheClip));
    public static string MemoryOptimizerView_CopyLastMemoryCleanResult => Get(nameof(MemoryOptimizerView_CopyLastMemoryCleanResult));
    public static string MemoryOptimizerView_CopyLastResult => Get(nameof(MemoryOptimizerView_CopyLastResult));
    public static string MemoryOptimizerView_CopyProcessName => Get(nameof(MemoryOptimizerView_CopyProcessName));
    public static string MemoryOptimizerView_DeepClean => Get(nameof(MemoryOptimizerView_DeepClean));
    public static string MemoryOptimizerView_ExcludedProcessesAreSkippedDuringWorkingSe => Get(nameof(MemoryOptimizerView_ExcludedProcessesAreSkippedDuringWorkingSe));
    public static string MemoryOptimizerView_FlushesTheSystemFileCache => Get(nameof(MemoryOptimizerView_FlushesTheSystemFileCache));
    public static string MemoryOptimizerView_FlushFileCache => Get(nameof(MemoryOptimizerView_FlushFileCache));
    public static string MemoryOptimizerView_FlushModifiedPages => Get(nameof(MemoryOptimizerView_FlushModifiedPages));
    public static string MemoryOptimizerView_GameAwareQuietWindow => Get(nameof(MemoryOptimizerView_GameAwareQuietWindow));
    public static string MemoryOptimizerView_Handles => Get(nameof(MemoryOptimizerView_Handles));
    public static string MemoryOptimizerView_HowItWorks => Get(nameof(MemoryOptimizerView_HowItWorks));
    public static string MemoryOptimizerView_IndividualOperations => Get(nameof(MemoryOptimizerView_IndividualOperations));
    public static string MemoryOptimizerView_KernelNonPaged => Get(nameof(MemoryOptimizerView_KernelNonPaged));
    public static string MemoryOptimizerView_MediumDiskIOBurst => Get(nameof(MemoryOptimizerView_MediumDiskIOBurst));
    public static string MemoryOptimizerView_MediumMayCauseBriefStutter => Get(nameof(MemoryOptimizerView_MediumMayCauseBriefStutter));
    public static string MemoryOptimizerView_MediumMaySlowFileAccessTemporarily => Get(nameof(MemoryOptimizerView_MediumMaySlowFileAccessTemporarily));
    public static string MemoryOptimizerView_MemoryCompression => Get(nameof(MemoryOptimizerView_MemoryCompression));
    public static string MemoryOptimizerView_MemoryOptimizer => Get(nameof(MemoryOptimizerView_MemoryOptimizer));
    public static string MemoryOptimizerView_MergesIdenticalMemoryPagesWindows10 => Get(nameof(MemoryOptimizerView_MergesIdenticalMemoryPagesWindows10));
    public static string MemoryOptimizerView_MinimumGapBetweenCleans => Get(nameof(MemoryOptimizerView_MinimumGapBetweenCleans));
    public static string MemoryOptimizerView_ModifiedPages => Get(nameof(MemoryOptimizerView_ModifiedPages));
    public static string MemoryOptimizerView_MonitorAndOptimizeSystemMemoryInRealTime => Get(nameof(MemoryOptimizerView_MonitorAndOptimizeSystemMemoryInRealTime));
    public static string MemoryOptimizerView_N30MinMemoryTrend => Get(nameof(MemoryOptimizerView_N30MinMemoryTrend));
    public static string MemoryOptimizerView_OpenFileLocation => Get(nameof(MemoryOptimizerView_OpenFileLocation));
    public static string MemoryOptimizerView_PageFile => Get(nameof(MemoryOptimizerView_PageFile));
    public static string MemoryOptimizerView_PeriodicMemoryCleaning => Get(nameof(MemoryOptimizerView_PeriodicMemoryCleaning));
    public static string MemoryOptimizerView_Private => Get(nameof(MemoryOptimizerView_Private));
    public static string MemoryOptimizerView_Process => Get(nameof(MemoryOptimizerView_Process));
    public static string MemoryOptimizerView_Processes => Get(nameof(MemoryOptimizerView_Processes));
    public static string MemoryOptimizerView_PurgeStandbyList => Get(nameof(MemoryOptimizerView_PurgeStandbyList));
    public static string MemoryOptimizerView_Ram => Get(nameof(MemoryOptimizerView_Ram));
    public static string MemoryOptimizerView_ReleasesUnusedMemoryFromAllProcesses => Get(nameof(MemoryOptimizerView_ReleasesUnusedMemoryFromAllProcesses));
    public static string MemoryOptimizerView_RemovesCachedPagesFromStandbyMemory => Get(nameof(MemoryOptimizerView_RemovesCachedPagesFromStandbyMemory));
    public static string MemoryOptimizerView_RunsSmartCleanAutomaticallyEveryNMinutes => Get(nameof(MemoryOptimizerView_RunsSmartCleanAutomaticallyEveryNMinutes));
    public static string MemoryOptimizerView_RunsSmartCleanWhenRamUsageExceedsThreshold => Get(nameof(MemoryOptimizerView_RunsSmartCleanWhenRamUsageExceedsThreshold));
    public static string MemoryOptimizerView_SafeCleanTrimsWorkingSetsPurgesLowPriority => Get(nameof(MemoryOptimizerView_SafeCleanTrimsWorkingSetsPurgesLowPriority));
    public static string MemoryOptimizerView_SetTo0ToUseTheSelectedProfileDefaultCooldo => Get(nameof(MemoryOptimizerView_SetTo0ToUseTheSelectedProfileDefaultCooldo));
    public static string MemoryOptimizerView_SmartClean => Get(nameof(MemoryOptimizerView_SmartClean));
    public static string MemoryOptimizerView_StandbyList => Get(nameof(MemoryOptimizerView_StandbyList));
    public static string MemoryOptimizerView_SystemCache => Get(nameof(MemoryOptimizerView_SystemCache));
    public static string MemoryOptimizerView_SystemInformation => Get(nameof(MemoryOptimizerView_SystemInformation));
    public static string MemoryOptimizerView_Threads => Get(nameof(MemoryOptimizerView_Threads));
    public static string MemoryOptimizerView_TipRightClickAProcessInTopMemoryHogsToAddG => Get(nameof(MemoryOptimizerView_TipRightClickAProcessInTopMemoryHogsToAddG));
    public static string MemoryOptimizerView_TopMemoryHogs => Get(nameof(MemoryOptimizerView_TopMemoryHogs));
    public static string MemoryOptimizerView_TrimsProcessWorkingSetsPurgesLowPrioritySt => Get(nameof(MemoryOptimizerView_TrimsProcessWorkingSetsPurgesLowPrioritySt));
    public static string MemoryOptimizerView_TrimWorkingSets => Get(nameof(MemoryOptimizerView_TrimWorkingSets));
    public static string MemoryOptimizerView_UsageThreshold => Get(nameof(MemoryOptimizerView_UsageThreshold));
    public static string MemoryOptimizerView_WorkingSet => Get(nameof(MemoryOptimizerView_WorkingSet));
    public static string MemoryOptimizerView_WorkingSetExclusions => Get(nameof(MemoryOptimizerView_WorkingSetExclusions));
    public static string MemoryOptimizerView_WritesModifiedPagesToDiskAndFreesMemory => Get(nameof(MemoryOptimizerView_WritesModifiedPagesToDiskAndFreesMemory));

    // Auto-extracted literals from OnboardingWindow.xaml
    public static string OnboardingWindow_ApplyAFanPreset => Get(nameof(OnboardingWindow_ApplyAFanPreset));
    public static string OnboardingWindow_Back => Get(nameof(OnboardingWindow_Back));
    public static string OnboardingWindow_Checking => Get(nameof(OnboardingWindow_Checking));
    public static string OnboardingWindow_ConfigureSettings => Get(nameof(OnboardingWindow_ConfigureSettings));
    public static string OnboardingWindow_CustomFanCurvesWithTemperatureBasedControl => Get(nameof(OnboardingWindow_CustomFanCurvesWithTemperatureBasedControl));
    public static string OnboardingWindow_CustomiseYourLighting => Get(nameof(OnboardingWindow_CustomiseYourLighting));
    public static string OnboardingWindow_Detecting => Get(nameof(OnboardingWindow_Detecting));
    public static string OnboardingWindow_EnableStartWithWindowsSetAPollingIntervalA => Get(nameof(OnboardingWindow_EnableStartWithWindowsSetAPollingIntervalA));
    public static string OnboardingWindow_FanControlBackend => Get(nameof(OnboardingWindow_FanControlBackend));
    public static string OnboardingWindow_GetStarted => Get(nameof(OnboardingWindow_GetStarted));
    public static string OnboardingWindow_HeadToFanControlAndTryTheAutoOrGamingPrese => Get(nameof(OnboardingWindow_HeadToFanControlAndTryTheAutoOrGamingPrese));
    public static string OnboardingWindow_MonitoringSource => Get(nameof(OnboardingWindow_MonitoringSource));
    public static string OnboardingWindow_N4ZoneRgbKeyboardLightingControl => Get(nameof(OnboardingWindow_N4ZoneRgbKeyboardLightingControl));
    public static string OnboardingWindow_Next => Get(nameof(OnboardingWindow_Next));
    public static string OnboardingWindow_OmencoreDetectedTheFollowingOnYourSystem => Get(nameof(OnboardingWindow_OmencoreDetectedTheFollowingOnYourSystem));
    public static string OnboardingWindow_OmencoreIsACommunityBuiltFanLightingAndPer => Get(nameof(OnboardingWindow_OmencoreIsACommunityBuiltFanLightingAndPer));
    public static string OnboardingWindow_OmencoreSetup => Get(nameof(OnboardingWindow_OmencoreSetup));
    public static string OnboardingWindow_OpenLightingAndClickAnyKeyboardZoneToChang => Get(nameof(OnboardingWindow_OpenLightingAndClickAnyKeyboardZoneToChang));
    public static string OnboardingWindow_PawnioDriver => Get(nameof(OnboardingWindow_PawnioDriver));
    public static string OnboardingWindow_PerformanceAndPowerModeSwitching => Get(nameof(OnboardingWindow_PerformanceAndPowerModeSwitching));
    public static string OnboardingWindow_QuickStart => Get(nameof(OnboardingWindow_QuickStart));
    public static string OnboardingWindow_RealTimeCpuGpuTemperatureAndLoadMonitoring => Get(nameof(OnboardingWindow_RealTimeCpuGpuTemperatureAndLoadMonitoring));
    public static string OnboardingWindow_ThisQuickSetupTakesAbout30SecondsAndHelpsO => Get(nameof(OnboardingWindow_ThisQuickSetupTakesAbout30SecondsAndHelpsO));
    public static string OnboardingWindow_Welcome => Get(nameof(OnboardingWindow_Welcome));
    public static string OnboardingWindow_WelcomeToOmencore => Get(nameof(OnboardingWindow_WelcomeToOmencore));
    public static string OnboardingWindow_WhatOmencoreDoes => Get(nameof(OnboardingWindow_WhatOmencoreDoes));
    public static string OnboardingWindow_YourHardware => Get(nameof(OnboardingWindow_YourHardware));

    // Auto-extracted literals from OsdOverlayWindow.xaml
    public static string OsdOverlayWindow_CpuClk => Get(nameof(OsdOverlayWindow_CpuClk));
    public static string OsdOverlayWindow_Dn => Get(nameof(OsdOverlayWindow_Dn));
    public static string OsdOverlayWindow_FpsViaRtss => Get(nameof(OsdOverlayWindow_FpsViaRtss));
    public static string OsdOverlayWindow_GpuClk => Get(nameof(OsdOverlayWindow_GpuClk));
    public static string OsdOverlayWindow_ThermalThrottle => Get(nameof(OsdOverlayWindow_ThermalThrottle));
    public static string OsdOverlayWindow_Up => Get(nameof(OsdOverlayWindow_Up));

    // Auto-extracted literals from QuickPopupWindow.xaml
    public static string QuickPopupWindow_CleanRam => Get(nameof(QuickPopupWindow_CleanRam));
    public static string QuickPopupWindow_Dashboard => Get(nameof(QuickPopupWindow_Dashboard));
    public static string QuickPopupWindow_DisplayOff => Get(nameof(QuickPopupWindow_DisplayOff));
    public static string QuickPopupWindow_FanPerfDecoupled => Get(nameof(QuickPopupWindow_FanPerfDecoupled));
    public static string QuickPopupWindow_MonitoringUnknown => Get(nameof(QuickPopupWindow_MonitoringUnknown));
    public static string QuickPopupWindow_OmencoreQuickAccess => Get(nameof(QuickPopupWindow_OmencoreQuickAccess));
    public static string QuickPopupWindow_OpenFullDashboard => Get(nameof(QuickPopupWindow_OpenFullDashboard));
    public static string QuickPopupWindow_Perform => Get(nameof(QuickPopupWindow_Perform));
    public static string QuickPopupWindow_QuickAccess => Get(nameof(QuickPopupWindow_QuickAccess));
    public static string QuickPopupWindow_QuickProfile => Get(nameof(QuickPopupWindow_QuickProfile));
    public static string QuickPopupWindow_RunASafeRamClean => Get(nameof(QuickPopupWindow_RunASafeRamClean));
    public static string QuickPopupWindow_ToggleRefreshRate => Get(nameof(QuickPopupWindow_ToggleRefreshRate));
    public static string QuickPopupWindow_TurnOffDisplay => Get(nameof(QuickPopupWindow_TurnOffDisplay));

    // Auto-extracted literals from SettingsView.xaml
    public static string SettingsView_AccentTheme => Get(nameof(SettingsView_AccentTheme));
    public static string SettingsView_AcPowerFanPreset => Get(nameof(SettingsView_AcPowerFanPreset));
    public static string SettingsView_AcPowerPerformanceMode => Get(nameof(SettingsView_AcPowerPerformanceMode));
    public static string SettingsView_AcPowerState => Get(nameof(SettingsView_AcPowerState));
    public static string SettingsView_AddAutomationRule => Get(nameof(SettingsView_AddAutomationRule));
    public static string SettingsView_AddAutomationRule_2 => Get(nameof(SettingsView_AddAutomationRule_2));
    public static string SettingsView_AddRule => Get(nameof(SettingsView_AddRule));
    public static string SettingsView_AddRule_2 => Get(nameof(SettingsView_AddRule_2));
    public static string SettingsView_Advanced => Get(nameof(SettingsView_Advanced));
    public static string SettingsView_AlertWhenCpuGpuTemperatureExceedsThreshold => Get(nameof(SettingsView_AlertWhenCpuGpuTemperatureExceedsThreshold));
    public static string SettingsView_AllowASavedMaxPresetToStartFansAtFullSpeed => Get(nameof(SettingsView_AllowASavedMaxPresetToStartFansAtFullSpeed));
    public static string SettingsView_AllowStartupRestoreOnOmen16Victus => Get(nameof(SettingsView_AllowStartupRestoreOnOmen16Victus));
    public static string SettingsView_AmbientLighting => Get(nameof(SettingsView_AmbientLighting));
    public static string SettingsView_AmbientLightingFps => Get(nameof(SettingsView_AmbientLightingFps));
    public static string SettingsView_AmbientLightingSaturation => Get(nameof(SettingsView_AmbientLightingSaturation));
    public static string SettingsView_AnOmenGamingHubServiceOrProcessIsRunningTh => Get(nameof(SettingsView_AnOmenGamingHubServiceOrProcessIsRunningTh));
    public static string SettingsView_Appearance => Get(nameof(SettingsView_Appearance));
    public static string SettingsView_ArgumentsOptional => Get(nameof(SettingsView_ArgumentsOptional));
    public static string SettingsView_Assistant => Get(nameof(SettingsView_Assistant));
    public static string SettingsView_ASystemRestorePointWillBeCreatedAutomatica => Get(nameof(SettingsView_ASystemRestorePointWillBeCreatedAutomatica));
    public static string SettingsView_ASystemRestorePointWillBeCreatedAutomatica_2 => Get(nameof(SettingsView_ASystemRestorePointWillBeCreatedAutomatica_2));
    public static string SettingsView_AutoBoostFansWhenTempsExceedThresholdInclu => Get(nameof(SettingsView_AutoBoostFansWhenTempsExceedThresholdInclu));
    public static string SettingsView_AutomaticallyApplyFanPresetsAndPerformance => Get(nameof(SettingsView_AutomaticallyApplyFanPresetsAndPerformance));
    public static string SettingsView_AutomaticallyApplyProfilesWhenGamesLaunch => Get(nameof(SettingsView_AutomaticallyApplyProfilesWhenGamesLaunch));
    public static string SettingsView_AutomaticallyApplyTheMatchingFanPresetWhen => Get(nameof(SettingsView_AutomaticallyApplyTheMatchingFanPresetWhen));
    public static string SettingsView_AutomaticallySwitchProfilesWhenAcPowerIsCo => Get(nameof(SettingsView_AutomaticallySwitchProfilesWhenAcPowerIsCo));
    public static string SettingsView_AutomaticMonitoringCadence => Get(nameof(SettingsView_AutomaticMonitoringCadence));
    public static string SettingsView_AutomaticSystemRestorePoint => Get(nameof(SettingsView_AutomaticSystemRestorePoint));
    public static string SettingsView_AutomationRuleName => Get(nameof(SettingsView_AutomationRuleName));
    public static string SettingsView_AutomationRules => Get(nameof(SettingsView_AutomationRules));
    public static string SettingsView_AutoSwitchOnPowerChange => Get(nameof(SettingsView_AutoSwitchOnPowerChange));
    public static string SettingsView_AutoSwitchProfilesWhenPluggingUnpluggingAc => Get(nameof(SettingsView_AutoSwitchProfilesWhenPluggingUnpluggingAc));
    public static string SettingsView_AutoWmiBiosWmiEcSetSpecificIfAutoDetection => Get(nameof(SettingsView_AutoWmiBiosWmiEcSetSpecificIfAutoDetection));
    public static string SettingsView_Battery => Get(nameof(SettingsView_Battery));
    public static string SettingsView_BatteryCare => Get(nameof(SettingsView_BatteryCare));
    public static string SettingsView_BatteryChargeThresholdPercent => Get(nameof(SettingsView_BatteryChargeThresholdPercent));
    public static string SettingsView_BatteryCondition => Get(nameof(SettingsView_BatteryCondition));
    public static string SettingsView_BatteryPowerFanPreset => Get(nameof(SettingsView_BatteryPowerFanPreset));
    public static string SettingsView_BatteryPowerPerformanceMode => Get(nameof(SettingsView_BatteryPowerPerformanceMode));
    public static string SettingsView_BatteryThreshold => Get(nameof(SettingsView_BatteryThreshold));
    public static string SettingsView_BiosInformation => Get(nameof(SettingsView_BiosInformation));
    public static string SettingsView_BiosWmiReliability => Get(nameof(SettingsView_BiosWmiReliability));
    public static string SettingsView_BoostModePerformance => Get(nameof(SettingsView_BoostModePerformance));
    public static string SettingsView_CadenceTiers => Get(nameof(SettingsView_CadenceTiers));
    public static string SettingsView_CaptureOmenKeyToShowOmencoreInsteadOfHpGam => Get(nameof(SettingsView_CaptureOmenKeyToShowOmencoreInsteadOfHpGam));
    public static string SettingsView_ChargeThreshold => Get(nameof(SettingsView_ChargeThreshold));
    public static string SettingsView_ChartHistory => Get(nameof(SettingsView_ChartHistory));
    public static string SettingsView_CheckForUpdates => Get(nameof(SettingsView_CheckForUpdates));
    public static string SettingsView_CheckForUpdatesAutomatically => Get(nameof(SettingsView_CheckForUpdatesAutomatically));
    public static string SettingsView_CheckHpForAvailableBiosFirmwareUpdates => Get(nameof(SettingsView_CheckHpForAvailableBiosFirmwareUpdates));
    public static string SettingsView_CheckInterval => Get(nameof(SettingsView_CheckInterval));
    public static string SettingsView_CleanHpOmenRegistryEntries => Get(nameof(SettingsView_CleanHpOmenRegistryEntries));
    public static string SettingsView_ClockTime => Get(nameof(SettingsView_ClockTime));
    public static string SettingsView_CompactBarAtScreenEdge => Get(nameof(SettingsView_CompactBarAtScreenEdge));
    public static string SettingsView_CompactPopup => Get(nameof(SettingsView_CompactPopup));
    public static string SettingsView_Confidence => Get(nameof(SettingsView_Confidence));
    public static string SettingsView_ConfigureApplicationBehaviorStartupOptions => Get(nameof(SettingsView_ConfigureApplicationBehaviorStartupOptions));
    public static string SettingsView_ConsecutiveMonitorTimeoutsRequiredBeforeSt => Get(nameof(SettingsView_ConsecutiveMonitorTimeoutsRequiredBeforeSt));
    public static string SettingsView_ContinuousFanSpeedAdjustmentDisableForBios => Get(nameof(SettingsView_ContinuousFanSpeedAdjustmentDisableForBios));
    public static string SettingsView_ControlHybridDiscreteOptimusModes => Get(nameof(SettingsView_ControlHybridDiscreteOptimusModes));
    public static string SettingsView_CopySummary => Get(nameof(SettingsView_CopySummary));
    public static string SettingsView_CorsairIcueIntegration => Get(nameof(SettingsView_CorsairIcueIntegration));
    public static string SettingsView_CpuLoad => Get(nameof(SettingsView_CpuLoad));
    public static string SettingsView_CpuPowerW => Get(nameof(SettingsView_CpuPowerW));
    public static string SettingsView_CpuTemp => Get(nameof(SettingsView_CpuTemp));
    public static string SettingsView_CpuUndervoltControls => Get(nameof(SettingsView_CpuUndervoltControls));
    public static string SettingsView_CreateAWindowsSystemRestoreSnapshotBeforeR => Get(nameof(SettingsView_CreateAWindowsSystemRestoreSnapshotBeforeR));
    public static string SettingsView_CreateManualRestorePoint => Get(nameof(SettingsView_CreateManualRestorePoint));
    public static string SettingsView_CtrlShiftB => Get(nameof(SettingsView_CtrlShiftB));
    public static string SettingsView_CtrlShiftE => Get(nameof(SettingsView_CtrlShiftE));
    public static string SettingsView_CtrlShiftF => Get(nameof(SettingsView_CtrlShiftF));
    public static string SettingsView_CtrlShiftO => Get(nameof(SettingsView_CtrlShiftO));
    public static string SettingsView_CtrlShiftP => Get(nameof(SettingsView_CtrlShiftP));
    public static string SettingsView_CtrlShiftQ => Get(nameof(SettingsView_CtrlShiftQ));
    public static string SettingsView_CurrentBios => Get(nameof(SettingsView_CurrentBios));
    public static string SettingsView_CurrentStatus => Get(nameof(SettingsView_CurrentStatus));
    public static string SettingsView_CurrentTier => Get(nameof(SettingsView_CurrentTier));
    public static string SettingsView_CycleFanModes => Get(nameof(SettingsView_CycleFanModes));
    public static string SettingsView_CycleGeneralProfiles => Get(nameof(SettingsView_CycleGeneralProfiles));
    public static string SettingsView_CyclePerformanceModes => Get(nameof(SettingsView_CyclePerformanceModes));
    public static string SettingsView_DataStorage => Get(nameof(SettingsView_DataStorage));
    public static string SettingsView_DefaultHotkeys => Get(nameof(SettingsView_DefaultHotkeys));
    public static string SettingsView_DiagnosticModeAcquiresExclusiveEcAccessToD => Get(nameof(SettingsView_DiagnosticModeAcquiresExclusiveEcAccessToD));
    public static string SettingsView_DisableHpServiceDependentFeaturesPerforman => Get(nameof(SettingsView_DisableHpServiceDependentFeaturesPerforman));
    public static string SettingsView_DonateViaPaypal => Get(nameof(SettingsView_DonateViaPaypal));
    public static string SettingsView_DoNotPinAFanLevelWhenReturningToAuto => Get(nameof(SettingsView_DoNotPinAFanLevelWhenReturningToAuto));
    public static string SettingsView_DoNotPinAFanLevelWhenReturningToAutoExperi => Get(nameof(SettingsView_DoNotPinAFanLevelWhenReturningToAutoExperi));
    public static string SettingsView_DownloadAndInstallPawnioASecureBootCompati => Get(nameof(SettingsView_DownloadAndInstallPawnioASecureBootCompati));
    public static string SettingsView_DownloadAndLaunchTheBiosUpdateInstaller => Get(nameof(SettingsView_DownloadAndLaunchTheBiosUpdateInstaller));
    public static string SettingsView_Downloads => Get(nameof(SettingsView_Downloads));
    public static string SettingsView_DownloadUpdate => Get(nameof(SettingsView_DownloadUpdate));
    public static string SettingsView_DownloadUpdate_2 => Get(nameof(SettingsView_DownloadUpdate_2));
    public static string SettingsView_EmergencyMaxFanThreshold => Get(nameof(SettingsView_EmergencyMaxFanThreshold));
    public static string SettingsView_EmergencyMaxFanThresholdInCelsius => Get(nameof(SettingsView_EmergencyMaxFanThresholdInCelsius));
    public static string SettingsView_EnableChargeLimit => Get(nameof(SettingsView_EnableChargeLimit));
    public static string SettingsView_EnableDisableFeaturesToReduceResourceUsage => Get(nameof(SettingsView_EnableDisableFeaturesToReduceResourceUsage));
    public static string SettingsView_EnableForOmenMax16LightBarZonesRunRightToL => Get(nameof(SettingsView_EnableForOmenMax16LightBarZonesRunRightToL));
    public static string SettingsView_EnableGlobalHotkeys => Get(nameof(SettingsView_EnableGlobalHotkeys));
    public static string SettingsView_EnableHysteresis => Get(nameof(SettingsView_EnableHysteresis));
    public static string SettingsView_EnableNotifications => Get(nameof(SettingsView_EnableNotifications));
    public static string SettingsView_EnableOnlyIfWmiKeyboardControlDoesnTWorkOn => Get(nameof(SettingsView_EnableOnlyIfWmiKeyboardControlDoesnTWorkOn));
    public static string SettingsView_EnableOrphanTimeout => Get(nameof(SettingsView_EnableOrphanTimeout));
    public static string SettingsView_EnableOsdOverlay => Get(nameof(SettingsView_EnableOsdOverlay));
    public static string SettingsView_EnableThisAutomationRule => Get(nameof(SettingsView_EnableThisAutomationRule));
    public static string SettingsView_EnableThisScheduleRule => Get(nameof(SettingsView_EnableThisScheduleRule));
    public static string SettingsView_EndTime => Get(nameof(SettingsView_EndTime));
    public static string SettingsView_Every6Hours => Get(nameof(SettingsView_Every6Hours));
    public static string SettingsView_EveryHour => Get(nameof(SettingsView_EveryHour));
    public static string SettingsView_ExclusiveEcDiagnostics => Get(nameof(SettingsView_ExclusiveEcDiagnostics));
    public static string SettingsView_ExecuteTheSelectedWindowsSystemCleanupTask => Get(nameof(SettingsView_ExecuteTheSelectedWindowsSystemCleanupTask));
    public static string SettingsView_ExperimentalDirectEcKeyboardControl => Get(nameof(SettingsView_ExperimentalDirectEcKeyboardControl));
    public static string SettingsView_ExperimentalFirmwareFnPProfileCycle => Get(nameof(SettingsView_ExperimentalFirmwareFnPProfileCycle));
    public static string SettingsView_ExportDiagnostics => Get(nameof(SettingsView_ExportDiagnostics));
    public static string SettingsView_ExportLogsAndSystemInfoForTroubleshooting => Get(nameof(SettingsView_ExportLogsAndSystemInfoForTroubleshooting));
    public static string SettingsView_ExportProfile => Get(nameof(SettingsView_ExportProfile));
    public static string SettingsView_ExportProfile_2 => Get(nameof(SettingsView_ExportProfile_2));
    public static string SettingsView_ExtendBatteryLifespanWithSmartChargeLimiti => Get(nameof(SettingsView_ExtendBatteryLifespanWithSmartChargeLimiti));
    public static string SettingsView_FactoryFanCurveInPerformanceMode => Get(nameof(SettingsView_FactoryFanCurveInPerformanceMode));
    public static string SettingsView_FactoryFanCurveInPerformanceModeExperiment => Get(nameof(SettingsView_FactoryFanCurveInPerformanceModeExperiment));
    public static string SettingsView_Fan => Get(nameof(SettingsView_Fan));
    public static string SettingsView_FanBoost => Get(nameof(SettingsView_FanBoost));
    public static string SettingsView_FanHysteresis => Get(nameof(SettingsView_FanHysteresis));
    public static string SettingsView_FanHysteresisDeadZone => Get(nameof(SettingsView_FanHysteresisDeadZone));
    public static string SettingsView_FanHysteresisRampDownDelay => Get(nameof(SettingsView_FanHysteresisRampDownDelay));
    public static string SettingsView_FanHysteresisRampUpDelay => Get(nameof(SettingsView_FanHysteresisRampUpDelay));
    public static string SettingsView_FanPresetAction => Get(nameof(SettingsView_FanPresetAction));
    public static string SettingsView_FansForcedTo100ImmediatelyAboveThis9099CAl => Get(nameof(SettingsView_FansForcedTo100ImmediatelyAboveThis9099CAl));
    public static string SettingsView_FanSpeedRpm => Get(nameof(SettingsView_FanSpeedRpm));
    public static string SettingsView_FansRampUpWhenTempsExceedThis7095C => Get(nameof(SettingsView_FansRampUpWhenTempsExceedThis7095C));
    public static string SettingsView_FansWonTChangeUnlessTempMovesBeyondThis => Get(nameof(SettingsView_FansWonTChangeUnlessTempMovesBeyondThis));
    public static string SettingsView_FeatureModules => Get(nameof(SettingsView_FeatureModules));
    public static string SettingsView_ForceEcKeyboardWrites => Get(nameof(SettingsView_ForceEcKeyboardWrites));
    public static string SettingsView_ForLaptopsWhoseFansStayNear2000RpmAtHighTe => Get(nameof(SettingsView_ForLaptopsWhoseFansStayNear2000RpmAtHighTe));
    public static string SettingsView_FpsRtss => Get(nameof(SettingsView_FpsRtss));
    public static string SettingsView_FrametimeMs => Get(nameof(SettingsView_FrametimeMs));
    public static string SettingsView_GameProfileAutoSwitching => Get(nameof(SettingsView_GameProfileAutoSwitching));
    public static string SettingsView_GameProfileNotifications => Get(nameof(SettingsView_GameProfileNotifications));
    public static string SettingsView_General => Get(nameof(SettingsView_General));
    public static string SettingsView_GetNotifiedAboutBetaAndPreviewVersions => Get(nameof(SettingsView_GetNotifiedAboutBetaAndPreviewVersions));
    public static string SettingsView_GetPawnioRecommended => Get(nameof(SettingsView_GetPawnioRecommended));
    public static string SettingsView_GetPawnioRecommended_2 => Get(nameof(SettingsView_GetPawnioRecommended_2));
    public static string SettingsView_Github => Get(nameof(SettingsView_Github));
    public static string SettingsView_GpuClock => Get(nameof(SettingsView_GpuClock));
    public static string SettingsView_GpuLoad => Get(nameof(SettingsView_GpuLoad));
    public static string SettingsView_GpuModeSwitching => Get(nameof(SettingsView_GpuModeSwitching));
    public static string SettingsView_GpuPowerW => Get(nameof(SettingsView_GpuPowerW));
    public static string SettingsView_GpuTemp => Get(nameof(SettingsView_GpuTemp));
    public static string SettingsView_HardwareDriver => Get(nameof(SettingsView_HardwareDriver));
    public static string SettingsView_HardwareWorker => Get(nameof(SettingsView_HardwareWorker));
    public static string SettingsView_HardwareWorkerOrphanTimeoutMinutes => Get(nameof(SettingsView_HardwareWorkerOrphanTimeoutMinutes));
    public static string SettingsView_HistoryCount => Get(nameof(SettingsView_HistoryCount));
    public static string SettingsView_HorizontalLayout => Get(nameof(SettingsView_HorizontalLayout));
    public static string SettingsView_HotkeyOsdVisuals => Get(nameof(SettingsView_HotkeyOsdVisuals));
    public static string SettingsView_Hotkeys => Get(nameof(SettingsView_Hotkeys));
    public static string SettingsView_HowLongAfterStartupTheGuardCanAutoActivate => Get(nameof(SettingsView_HowLongAfterStartupTheGuardCanAutoActivate));
    public static string SettingsView_HowOftenToCheckForUpdates => Get(nameof(SettingsView_HowOftenToCheckForUpdates));
    public static string SettingsView_HpBloatwareRemoval => Get(nameof(SettingsView_HpBloatwareRemoval));
    public static string SettingsView_HpDriverDownloads => Get(nameof(SettingsView_HpDriverDownloads));
    public static string SettingsView_HpDriverSupportResources => Get(nameof(SettingsView_HpDriverSupportResources));
    public static string SettingsView_HpModel => Get(nameof(SettingsView_HpModel));
    public static string SettingsView_HpOmenGamingHubCleanup => Get(nameof(SettingsView_HpOmenGamingHubCleanup));
    public static string SettingsView_HpSOfficialControlSoftwareProvidesWmiAcces => Get(nameof(SettingsView_HpSOfficialControlSoftwareProvidesWmiAcces));
    public static string SettingsView_HpSupport => Get(nameof(SettingsView_HpSupport));
    public static string SettingsView_HpSupportAssistant => Get(nameof(SettingsView_HpSupportAssistant));
    public static string SettingsView_IdleMinutes => Get(nameof(SettingsView_IdleMinutes));
    public static string SettingsView_ImportProfile => Get(nameof(SettingsView_ImportProfile));
    public static string SettingsView_ImportProfile_2 => Get(nameof(SettingsView_ImportProfile_2));
    public static string SettingsView_IncludePreReleases => Get(nameof(SettingsView_IncludePreReleases));
    public static string SettingsView_InGameOsd => Get(nameof(SettingsView_InGameOsd));
    public static string SettingsView_InvertRgbZoneOrder => Get(nameof(SettingsView_InvertRgbZoneOrder));
    public static string SettingsView_KeyboardBackend => Get(nameof(SettingsView_KeyboardBackend));
    public static string SettingsView_KeyboardBacklightControl => Get(nameof(SettingsView_KeyboardBacklightControl));
    public static string SettingsView_KeyboardProfile => Get(nameof(SettingsView_KeyboardProfile));
    public static string SettingsView_KeyboardSource => Get(nameof(SettingsView_KeyboardSource));
    public static string SettingsView_KillRunningHpOmenProcesses => Get(nameof(SettingsView_KillRunningHpOmenProcesses));
    public static string SettingsView_LatestBios => Get(nameof(SettingsView_LatestBios));
    public static string SettingsView_LidState => Get(nameof(SettingsView_LidState));
    public static string SettingsView_LinkFanToPerformanceMode => Get(nameof(SettingsView_LinkFanToPerformanceMode));
    public static string SettingsView_LiveCadenceStatus => Get(nameof(SettingsView_LiveCadenceStatus));
    public static string SettingsView_LoadFanCurvesPerformanceModesAndGpuOcSetti => Get(nameof(SettingsView_LoadFanCurvesPerformanceModesAndGpuOcSetti));
    public static string SettingsView_LogitechGHubIntegration => Get(nameof(SettingsView_LogitechGHubIntegration));
    public static string SettingsView_LowerNumbersRunFirst => Get(nameof(SettingsView_LowerNumbersRunFirst));
    public static string SettingsView_LowOverheadMode => Get(nameof(SettingsView_LowOverheadMode));
    public static string SettingsView_MaxFanAutoReturnTimerMinutes => Get(nameof(SettingsView_MaxFanAutoReturnTimerMinutes));
    public static string SettingsView_MaxFanLevelOverride => Get(nameof(SettingsView_MaxFanLevelOverride));
    public static string SettingsView_MayInterfereWithOmenGamingHubOrOtherFanCon => Get(nameof(SettingsView_MayInterfereWithOmenGamingHubOrOtherFanCon));
    public static string SettingsView_MetricGroups => Get(nameof(SettingsView_MetricGroups));
    public static string SettingsView_MetricsToDisplay => Get(nameof(SettingsView_MetricsToDisplay));
    public static string SettingsView_Minutes => Get(nameof(SettingsView_Minutes));
    public static string SettingsView_MinutesToWaitForReconnectionBeforeWorkerEx => Get(nameof(SettingsView_MinutesToWaitForReconnectionBeforeWorkerEx));
    public static string SettingsView_Mode => Get(nameof(SettingsView_Mode));
    public static string SettingsView_ModeChangeNotifications => Get(nameof(SettingsView_ModeChangeNotifications));
    public static string SettingsView_ModelIdentityResolution => Get(nameof(SettingsView_ModelIdentityResolution));
    public static string SettingsView_Monitoring => Get(nameof(SettingsView_Monitoring));
    public static string SettingsView_N1sWhenTheUiOrOsdNeedsLiveData5sInStandardB => Get(nameof(SettingsView_N1sWhenTheUiOrOsdNeedsLiveData5sInStandardB));
    public static string SettingsView_NetDownload => Get(nameof(SettingsView_NetDownload));
    public static string SettingsView_NetUpload => Get(nameof(SettingsView_NetUpload));
    public static string SettingsView_Network => Get(nameof(SettingsView_Network));
    public static string SettingsView_NetworkPing => Get(nameof(SettingsView_NetworkPing));
    public static string SettingsView_Notifications => Get(nameof(SettingsView_Notifications));
    public static string SettingsView_NotifyWhenFanOrPerformanceModesChangeViaHo => Get(nameof(SettingsView_NotifyWhenFanOrPerformanceModesChangeViaHo));
    public static string SettingsView_NotifyWhenGameProfilesAreActivatedDeactiva => Get(nameof(SettingsView_NotifyWhenGameProfilesAreActivatedDeactiva));
    public static string SettingsView_NumberOfDataPointsToDisplayInCharts => Get(nameof(SettingsView_NumberOfDataPointsToDisplayInCharts));
    public static string SettingsView_Official => Get(nameof(SettingsView_Official));
    public static string SettingsView_OffKeepsMaxUntilYouChangeItCooldownHandsTh => Get(nameof(SettingsView_OffKeepsMaxUntilYouChangeItCooldownHandsTh));
    public static string SettingsView_OmencoreIsFreeAndOpenSourceIfYouFindItUsef => Get(nameof(SettingsView_OmencoreIsFreeAndOpenSourceIfYouFindItUsef));
    public static string SettingsView_OmencoreNowOwnsCadenceAutomaticallyActiveW => Get(nameof(SettingsView_OmencoreNowOwnsCadenceAutomaticallyActiveW));
    public static string SettingsView_OmenGamingHub => Get(nameof(SettingsView_OmenGamingHub));
    public static string SettingsView_OmenGamingHubOfficial => Get(nameof(SettingsView_OmenGamingHubOfficial));
    public static string SettingsView_OmenHub => Get(nameof(SettingsView_OmenHub));
    public static string SettingsView_OmenKeyAction => Get(nameof(SettingsView_OmenKeyAction));
    public static string SettingsView_OmenKeyArguments => Get(nameof(SettingsView_OmenKeyArguments));
    public static string SettingsView_OmenKeyboard4ZoneRgbLighting => Get(nameof(SettingsView_OmenKeyboard4ZoneRgbLighting));
    public static string SettingsView_OmenKeyInterception => Get(nameof(SettingsView_OmenKeyInterception));
    public static string SettingsView_OmenKeyInterceptionEnabled => Get(nameof(SettingsView_OmenKeyInterceptionEnabled));
    public static string SettingsView_OmenKeyProgramOrScript => Get(nameof(SettingsView_OmenKeyProgramOrScript));
    public static string SettingsView_OnlyEnableHotkeysWhenWindowIsFocused => Get(nameof(SettingsView_OnlyEnableHotkeysWhenWindowIsFocused));
    public static string SettingsView_OnScreenOverlayShowingSystemStatsDuringGam => Get(nameof(SettingsView_OnScreenOverlayShowingSystemStatsDuringGam));
    public static string SettingsView_OnThisBoardTheFirmwareSOwnAutoParksTheFans => Get(nameof(SettingsView_OnThisBoardTheFirmwareSOwnAutoParksTheFans));
    public static string SettingsView_OpenBloatwareManager => Get(nameof(SettingsView_OpenBloatwareManager));
    public static string SettingsView_OpenBloatwareManager_2 => Get(nameof(SettingsView_OpenBloatwareManager_2));
    public static string SettingsView_OpenConfigFolder => Get(nameof(SettingsView_OpenConfigFolder));
    public static string SettingsView_OpenConfigFolder_2 => Get(nameof(SettingsView_OpenConfigFolder_2));
    public static string SettingsView_OpenLocalappdataOmencoreWhereSettingsAndPr => Get(nameof(SettingsView_OpenLocalappdataOmencoreWhereSettingsAndPr));
    public static string SettingsView_OpenLogFolder => Get(nameof(SettingsView_OpenLogFolder));
    public static string SettingsView_OpenLogFolder_2 => Get(nameof(SettingsView_OpenLogFolder_2));
    public static string SettingsView_OpenTheFolderContainingOmencoreDiagnosticL => Get(nameof(SettingsView_OpenTheFolderContainingOmencoreDiagnosticL));
    public static string SettingsView_OpenTheGithubIssueTrackerToReportABugOrReq => Get(nameof(SettingsView_OpenTheGithubIssueTrackerToReportABugOrReq));
    public static string SettingsView_OpenTheOmencoreGithubRepositoryInYourBrows => Get(nameof(SettingsView_OpenTheOmencoreGithubRepositoryInYourBrows));
    public static string SettingsView_OrphanTimeoutDuration => Get(nameof(SettingsView_OrphanTimeoutDuration));
    public static string SettingsView_OsdHotkeyToastDurationMs => Get(nameof(SettingsView_OsdHotkeyToastDurationMs));
    public static string SettingsView_OsdHotkeyToastScalePercent => Get(nameof(SettingsView_OsdHotkeyToastScalePercent));
    public static string SettingsView_OverlayDensity => Get(nameof(SettingsView_OverlayDensity));
    public static string SettingsView_OverrideAutoDetectedMaxFanLevelSetTo0ForAu => Get(nameof(SettingsView_OverrideAutoDetectedMaxFanLevelSetTo0ForAu));
    public static string SettingsView_OverridesTheExtraGuardrailForModelsThatHav => Get(nameof(SettingsView_OverridesTheExtraGuardrailForModelsThatHav));
    public static string SettingsView_PawnioOnlyMode => Get(nameof(SettingsView_PawnioOnlyMode));
    public static string SettingsView_PerfMode => Get(nameof(SettingsView_PerfMode));
    public static string SettingsView_PerformanceModeAction => Get(nameof(SettingsView_PerformanceModeAction));
    public static string SettingsView_PeriodicallyCheckGithubForNewReleases => Get(nameof(SettingsView_PeriodicallyCheckGithubForNewReleases));
    public static string SettingsView_PopupDuration => Get(nameof(SettingsView_PopupDuration));
    public static string SettingsView_PopupSize => Get(nameof(SettingsView_PopupSize));
    public static string SettingsView_Position => Get(nameof(SettingsView_Position));
    public static string SettingsView_PostResumeRecovery => Get(nameof(SettingsView_PostResumeRecovery));
    public static string SettingsView_PowerAutomation => Get(nameof(SettingsView_PowerAutomation));
    public static string SettingsView_PowerMemory => Get(nameof(SettingsView_PowerMemory));
    public static string SettingsView_PowerSourceAutomation => Get(nameof(SettingsView_PowerSourceAutomation));
    public static string SettingsView_PreventFanSpeedOscillationWithDeadZoneAndD => Get(nameof(SettingsView_PreventFanSpeedOscillationWithDeadZoneAndD));
    public static string SettingsView_PreventsConflictsByDeactivatingHotkeysWhen => Get(nameof(SettingsView_PreventsConflictsByDeactivatingHotkeysWhen));
    public static string SettingsView_PriorityLowerNumbersRunFirst => Get(nameof(SettingsView_PriorityLowerNumbersRunFirst));
    public static string SettingsView_ProcessName => Get(nameof(SettingsView_ProcessName));
    public static string SettingsView_ProcessNameEGGameExe => Get(nameof(SettingsView_ProcessNameEGGameExe));
    public static string SettingsView_ProfileManagement => Get(nameof(SettingsView_ProfileManagement));
    public static string SettingsView_ProfileScheduler => Get(nameof(SettingsView_ProfileScheduler));
    public static string SettingsView_ProgramOrScriptToRun => Get(nameof(SettingsView_ProgramOrScriptToRun));
    public static string SettingsView_PromotedTriggersForShippedAutomationTimeWi => Get(nameof(SettingsView_PromotedTriggersForShippedAutomationTimeWi));
    public static string SettingsView_QuietMode => Get(nameof(SettingsView_QuietMode));
    public static string SettingsView_RampDownDelay => Get(nameof(SettingsView_RampDownDelay));
    public static string SettingsView_RampUpDelay => Get(nameof(SettingsView_RampUpDelay));
    public static string SettingsView_ReapplyConfirmedUndervoltTccCpuPowerAndGpu => Get(nameof(SettingsView_ReapplyConfirmedUndervoltTccCpuPowerAndGpu));
    public static string SettingsView_ReapplyPerformanceModeAndGpuPowerBoost => Get(nameof(SettingsView_ReapplyPerformanceModeAndGpuPowerBoost));
    public static string SettingsView_ReapplySavedFanPresetOrCurveAtStartup => Get(nameof(SettingsView_ReapplySavedFanPresetOrCurveAtStartup));
    public static string SettingsView_ReapplyTheSavedFanPresetPerformanceModeGpu => Get(nameof(SettingsView_ReapplyTheSavedFanPresetPerformanceModeGpu));
    public static string SettingsView_Reason => Get(nameof(SettingsView_Reason));
    public static string SettingsView_ReCheckDriverAndEcBackendStatus => Get(nameof(SettingsView_ReCheckDriverAndEcBackendStatus));
    public static string SettingsView_ReduceCpuUsageByLimitingSensorPolling => Get(nameof(SettingsView_ReduceCpuUsageByLimitingSensorPolling));
    public static string SettingsView_RefreshStatus => Get(nameof(SettingsView_RefreshStatus));
    public static string SettingsView_RefreshStatus_2 => Get(nameof(SettingsView_RefreshStatus_2));
    public static string SettingsView_ReleaseNotes => Get(nameof(SettingsView_ReleaseNotes));
    public static string SettingsView_RemoveAutomationRule => Get(nameof(SettingsView_RemoveAutomationRule));
    public static string SettingsView_RemoveHpOmenGamingHubAndRelatedServicesThi => Get(nameof(SettingsView_RemoveHpOmenGamingHubAndRelatedServicesThi));
    public static string SettingsView_RemoveHpOmenLegacyInstallersWingetAppx => Get(nameof(SettingsView_RemoveHpOmenLegacyInstallersWingetAppx));
    public static string SettingsView_RemoveHpOmenServicesOmenserviceLightingser => Get(nameof(SettingsView_RemoveHpOmenServicesOmenserviceLightingser));
    public static string SettingsView_RemoveResidualHpOmenFilesAndFolders => Get(nameof(SettingsView_RemoveResidualHpOmenFilesAndFolders));
    public static string SettingsView_RemoveScheduleRule => Get(nameof(SettingsView_RemoveScheduleRule));
    public static string SettingsView_RemoveUnnecessaryHpPreInstalledApps => Get(nameof(SettingsView_RemoveUnnecessaryHpPreInstalledApps));
    public static string SettingsView_ReportIssue => Get(nameof(SettingsView_ReportIssue));
    public static string SettingsView_ReportIssue_2 => Get(nameof(SettingsView_ReportIssue_2));
    public static string SettingsView_ResetAllSettingsToDefaultValues => Get(nameof(SettingsView_ResetAllSettingsToDefaultValues));
    public static string SettingsView_ResetEc => Get(nameof(SettingsView_ResetEc));
    public static string SettingsView_ResetEc_2 => Get(nameof(SettingsView_ResetEc_2));
    public static string SettingsView_ResetEcToDefaults => Get(nameof(SettingsView_ResetEcToDefaults));
    public static string SettingsView_ResetToDefaults => Get(nameof(SettingsView_ResetToDefaults));
    public static string SettingsView_ResetToDefaults_2 => Get(nameof(SettingsView_ResetToDefaults_2));
    public static string SettingsView_ResolutionSource => Get(nameof(SettingsView_ResolutionSource));
    public static string SettingsView_ResolvedModel => Get(nameof(SettingsView_ResolvedModel));
    public static string SettingsView_RestartRecommendedToCompleteCleanup => Get(nameof(SettingsView_RestartRecommendedToCompleteCleanup));
    public static string SettingsView_RestoreLastHardwareSettingsOnStartup => Get(nameof(SettingsView_RestoreLastHardwareSettingsOnStartup));
    public static string SettingsView_RestoreSavedFanMax => Get(nameof(SettingsView_RestoreSavedFanMax));
    public static string SettingsView_RestoreSavedKeyboardLightingColorsWhenRgbS => Get(nameof(SettingsView_RestoreSavedKeyboardLightingColorsWhenRgbS));
    public static string SettingsView_RestoresBiosFanControlUseIfBiosShowsStuckI => Get(nameof(SettingsView_RestoresBiosFanControlUseIfBiosShowsStuckI));
    public static string SettingsView_ResumeRecoveryTimeline => Get(nameof(SettingsView_ResumeRecoveryTimeline));
    public static string SettingsView_ReturnMaxFanModeToAuto => Get(nameof(SettingsView_ReturnMaxFanModeToAuto));
    public static string SettingsView_RuleName => Get(nameof(SettingsView_RuleName));
    public static string SettingsView_RunCleanup => Get(nameof(SettingsView_RunCleanup));
    public static string SettingsView_RunCleanup_2 => Get(nameof(SettingsView_RunCleanup_2));
    public static string SettingsView_RunFansAtMaximumSpeedFor60SecondsToClearDu => Get(nameof(SettingsView_RunFansAtMaximumSpeedFor60SecondsToClearDu));
    public static string SettingsView_SafeAlternativeUseHpOmenGamingHubForKeyboa => Get(nameof(SettingsView_SafeAlternativeUseHpOmenGamingHubForKeyboa));
    public static string SettingsView_SafeModeStartupWindow => Get(nameof(SettingsView_SafeModeStartupWindow));
    public static string SettingsView_SamplesColorsFromScreenEdgesAndAppliesToRg => Get(nameof(SettingsView_SamplesColorsFromScreenEdgesAndAppliesToRg));
    public static string SettingsView_Saturation => Get(nameof(SettingsView_Saturation));
    public static string SettingsView_SaveAndLoadYourCompleteOmencoreConfigurati => Get(nameof(SettingsView_SaveAndLoadYourCompleteOmencoreConfigurati));
    public static string SettingsView_SaveYourCurrentConfigurationToShareOrBacku => Get(nameof(SettingsView_SaveYourCurrentConfigurationToShareOrBacku));
    public static string SettingsView_Scheduler => Get(nameof(SettingsView_Scheduler));
    public static string SettingsView_SearchResults => Get(nameof(SettingsView_SearchResults));
    public static string SettingsView_SecureBoot => Get(nameof(SettingsView_SecureBoot));
    public static string SettingsView_SecureBootCompatibleDriverForEcMsrAccess => Get(nameof(SettingsView_SecureBootCompatibleDriverForEcMsrAccess));
    public static string SettingsView_SetMaximumBatteryChargeLevel => Get(nameof(SettingsView_SetMaximumBatteryChargeLevel));
    public static string SettingsView_Settings => Get(nameof(SettingsView_Settings));
    public static string SettingsView_SettingsSearchQuery => Get(nameof(SettingsView_SettingsSearchQuery));
    public static string SettingsView_ShareAnonymousPidSuccessFailureCountsToHel => Get(nameof(SettingsView_ShareAnonymousPidSuccessFailureCountsToHel));
    public static string SettingsView_ShowHideOmencoreWindow => Get(nameof(SettingsView_ShowHideOmencoreWindow));
    public static string SettingsView_ShowOmencoreWindow => Get(nameof(SettingsView_ShowOmencoreWindow));
    public static string SettingsView_ShowWindowsToastNotificationsForEvents => Get(nameof(SettingsView_ShowWindowsToastNotificationsForEvents));
    public static string SettingsView_SmallerCardLessSecondaryText => Get(nameof(SettingsView_SmallerCardLessSecondaryText));
    public static string SettingsView_SmoothOutFanSpeedChangesToReduceNoiseFluct => Get(nameof(SettingsView_SmoothOutFanSpeedChangesToReduceNoiseFluct));
    public static string SettingsView_SomeComponentsMayStillBeRunningUntilYouRes => Get(nameof(SettingsView_SomeComponentsMayStillBeRunningUntilYouRes));
    public static string SettingsView_StandaloneStatus => Get(nameof(SettingsView_StandaloneStatus));
    public static string SettingsView_StartFanBoost => Get(nameof(SettingsView_StartFanBoost));
    public static string SettingsView_StartFanBoost_2 => Get(nameof(SettingsView_StartFanBoost_2));
    public static string SettingsView_StartTime => Get(nameof(SettingsView_StartTime));
    public static string SettingsView_StartupRestore => Get(nameof(SettingsView_StartupRestore));
    public static string SettingsView_StartupRestoreCategories => Get(nameof(SettingsView_StartupRestoreCategories));
    public static string SettingsView_StartupSafeModeGuard => Get(nameof(SettingsView_StartupSafeModeGuard));
    public static string SettingsView_StartupSafeModeTimeoutThreshold => Get(nameof(SettingsView_StartupSafeModeTimeoutThreshold));
    public static string SettingsView_StartupSafeModeWindowSeconds => Get(nameof(SettingsView_StartupSafeModeWindowSeconds));
    public static string SettingsView_StopChargingAtACustomThresholdToPreserveBa => Get(nameof(SettingsView_StopChargingAtACustomThresholdToPreserveBa));
    public static string SettingsView_StopTheFanCleaningCycleAndReturnFansToNorm => Get(nameof(SettingsView_StopTheFanCleaningCycleAndReturnFansToNorm));
    public static string SettingsView_SupportDevelopment => Get(nameof(SettingsView_SupportDevelopment));
    public static string SettingsView_SupportForCorsairKeyboardsMiceHeadsets => Get(nameof(SettingsView_SupportForCorsairKeyboardsMiceHeadsets));
    public static string SettingsView_SupportForLogitechGPeripherals => Get(nameof(SettingsView_SupportForLogitechGPeripherals));
    public static string SettingsView_SwitchToTheBloatwareManagerTab => Get(nameof(SettingsView_SwitchToTheBloatwareManagerTab));
    public static string SettingsView_SyncKeyboardPeripheralsWithScreenColorsAmb => Get(nameof(SettingsView_SyncKeyboardPeripheralsWithScreenColorsAmb));
    public static string SettingsView_SystemStatus => Get(nameof(SettingsView_SystemStatus));
    public static string SettingsView_TargetMonitor => Get(nameof(SettingsView_TargetMonitor));
    public static string SettingsView_TemperatureCondition => Get(nameof(SettingsView_TemperatureCondition));
    public static string SettingsView_TemperatureDeadZone => Get(nameof(SettingsView_TemperatureDeadZone));
    public static string SettingsView_TemperatureLoad => Get(nameof(SettingsView_TemperatureLoad));
    public static string SettingsView_TemperatureSensor => Get(nameof(SettingsView_TemperatureSensor));
    public static string SettingsView_TemperatureThreshold => Get(nameof(SettingsView_TemperatureThreshold));
    public static string SettingsView_TemperatureThresholdC => Get(nameof(SettingsView_TemperatureThresholdC));
    public static string SettingsView_TemperatureWarnings => Get(nameof(SettingsView_TemperatureWarnings));
    public static string SettingsView_TemporarilyBlocksTrayWriteActionsIfMonitor => Get(nameof(SettingsView_TemporarilyBlocksTrayWriteActionsIfMonitor));
    public static string SettingsView_TheMethodUsedToControlFansOnYourSystem => Get(nameof(SettingsView_TheMethodUsedToControlFansOnYourSystem));
    public static string SettingsView_ThermalProtection => Get(nameof(SettingsView_ThermalProtection));
    public static string SettingsView_ThermalProtectionRampThresholdInCelsius => Get(nameof(SettingsView_ThermalProtectionRampThresholdInCelsius));
    public static string SettingsView_ThermalProtectionThreshold => Get(nameof(SettingsView_ThermalProtectionThreshold));
    public static string SettingsView_Thermals => Get(nameof(SettingsView_Thermals));
    public static string SettingsView_ThisMayHardCrashYourSystemRequiringForcedR => Get(nameof(SettingsView_ThisMayHardCrashYourSystemRequiringForcedR));
    public static string SettingsView_Time => Get(nameof(SettingsView_Time));
    public static string SettingsView_TimeoutThreshold => Get(nameof(SettingsView_TimeoutThreshold));
    public static string SettingsView_TipYouCanAlsoManuallyCreateARestorePointUs => Get(nameof(SettingsView_TipYouCanAlsoManuallyCreateARestorePointUs));
    public static string SettingsView_ToggleHotkey => Get(nameof(SettingsView_ToggleHotkey));
    public static string SettingsView_TriggerType => Get(nameof(SettingsView_TriggerType));
    public static string SettingsView_UltraLowBlockers => Get(nameof(SettingsView_UltraLowBlockers));
    public static string SettingsView_UninstallHpOmenGamingHubApplication => Get(nameof(SettingsView_UninstallHpOmenGamingHubApplication));
    public static string SettingsView_UpdateCheckInterval => Get(nameof(SettingsView_UpdateCheckInterval));
    public static string SettingsView_UpdateRate => Get(nameof(SettingsView_UpdateRate));
    public static string SettingsView_Updates => Get(nameof(SettingsView_Updates));
    public static string SettingsView_UseBiosWmiEventMappingToCyclePerformancePr => Get(nameof(SettingsView_UseBiosWmiEventMappingToCyclePerformancePr));
    public static string SettingsView_UseKeyboardShortcutsEvenWhenAppIsInBackgro => Get(nameof(SettingsView_UseKeyboardShortcutsEvenWhenAppIsInBackgro));
    public static string SettingsView_Version => Get(nameof(SettingsView_Version));
    public static string SettingsView_ViewTheFullChangelogForThisRelease => Get(nameof(SettingsView_ViewTheFullChangelogForThisRelease));
    public static string SettingsView_VoltageOffsetAdjustmentRequiresCompatibleC => Get(nameof(SettingsView_VoltageOffsetAdjustmentRequiresCompatibleC));
    public static string SettingsView_VramUsage => Get(nameof(SettingsView_VramUsage));
    public static string SettingsView_WaitBeforeDecreasingFanSpeed => Get(nameof(SettingsView_WaitBeforeDecreasingFanSpeed));
    public static string SettingsView_WaitBeforeIncreasingFanSpeed => Get(nameof(SettingsView_WaitBeforeIncreasingFanSpeed));
    public static string SettingsView_WarningEcWritesCausedCrashesOnOmen17Ck2xxx => Get(nameof(SettingsView_WarningEcWritesCausedCrashesOnOmen17Ck2xxx));
    public static string SettingsView_WhenDisabledNoOsdBackgroundProcessRunsAtAl => Get(nameof(SettingsView_WhenDisabledNoOsdBackgroundProcessRunsAtAl));
    public static string SettingsView_WhenOnAcPower => Get(nameof(SettingsView_WhenOnAcPower));
    public static string SettingsView_WhenOnBattery => Get(nameof(SettingsView_WhenOnBattery));
    public static string SettingsView_WifiNetworkNameSsid => Get(nameof(SettingsView_WifiNetworkNameSsid));
    public static string SettingsView_WifiSsid => Get(nameof(SettingsView_WifiSsid));
    public static string SettingsView_WinF12 => Get(nameof(SettingsView_WinF12));
    public static string SettingsView_WorkerProcessExitsAfterMainAppClosesIfNoRe => Get(nameof(SettingsView_WorkerProcessExitsAfterMainAppClosesIfNoRe));

    // Auto-extracted literals from SplashWindow.xaml
    public static string SplashWindow_HardwareControlSuite => Get(nameof(SplashWindow_HardwareControlSuite));
    public static string SplashWindow_Initializing => Get(nameof(SplashWindow_Initializing));
    public static string SplashWindow_V200Alpha1 => Get(nameof(SplashWindow_V200Alpha1));

    // Auto-extracted literals from SystemOptimizerView.xaml
    public static string SystemOptimizerView_ApplyAll => Get(nameof(SystemOptimizerView_ApplyAll));
    public static string SystemOptimizerView_ApplyAllOptimizationsForMaximumGamingPerfo => Get(nameof(SystemOptimizerView_ApplyAllOptimizationsForMaximumGamingPerfo));
    public static string SystemOptimizerView_Applying => Get(nameof(SystemOptimizerView_Applying));
    public static string SystemOptimizerView_ApplyRecommendedOptimizationsOnly => Get(nameof(SystemOptimizerView_ApplyRecommendedOptimizationsOnly));
    public static string SystemOptimizerView_ExportOptimizerStateAndDriftDiagnosticsRep => Get(nameof(SystemOptimizerView_ExportOptimizerStateAndDriftDiagnosticsRep));
    public static string SystemOptimizerView_ExportReport => Get(nameof(SystemOptimizerView_ExportReport));
    public static string SystemOptimizerView_FineTuneWindowsForGamingPerformance => Get(nameof(SystemOptimizerView_FineTuneWindowsForGamingPerformance));
    public static string SystemOptimizerView_GamingMax => Get(nameof(SystemOptimizerView_GamingMax));
    public static string SystemOptimizerView_ImportantNotes => Get(nameof(SystemOptimizerView_ImportantNotes));
    public static string SystemOptimizerView_InputGaming => Get(nameof(SystemOptimizerView_InputGaming));
    public static string SystemOptimizerView_ManualUndo => Get(nameof(SystemOptimizerView_ManualUndo));
    public static string SystemOptimizerView_NetworkLatency => Get(nameof(SystemOptimizerView_NetworkLatency));
    public static string SystemOptimizerView_PowerPerformance => Get(nameof(SystemOptimizerView_PowerPerformance));
    public static string SystemOptimizerView_Preflight => Get(nameof(SystemOptimizerView_Preflight));
    public static string SystemOptimizerView_RefreshRiskTierPreflightSummary => Get(nameof(SystemOptimizerView_RefreshRiskTierPreflightSummary));
    public static string SystemOptimizerView_RegistryKeys => Get(nameof(SystemOptimizerView_RegistryKeys));
    public static string SystemOptimizerView_RestoreAllSettingsToWindowsDefaults => Get(nameof(SystemOptimizerView_RestoreAllSettingsToWindowsDefaults));
    public static string SystemOptimizerView_RevertAll => Get(nameof(SystemOptimizerView_RevertAll));
    public static string SystemOptimizerView_ServicesOrCommands => Get(nameof(SystemOptimizerView_ServicesOrCommands));
    public static string SystemOptimizerView_StorageDisk => Get(nameof(SystemOptimizerView_StorageDisk));
    public static string SystemOptimizerView_SystemOptimizer => Get(nameof(SystemOptimizerView_SystemOptimizer));
    public static string SystemOptimizerView_VisualEffects => Get(nameof(SystemOptimizerView_VisualEffects));
    public static string SystemOptimizerView_WhatChanges => Get(nameof(SystemOptimizerView_WhatChanges));
    public static string SystemOptimizerView_WindowsServices => Get(nameof(SystemOptimizerView_WindowsServices));

    // Auto-extracted literals from ThermalChart.xaml
    public static string ThermalChart_LoadingTemperatureHistory => Get(nameof(ThermalChart_LoadingTemperatureHistory));

    // Auto-extracted literals from TuningView.xaml
    public static string TuningView_AdjustCpuSustainedPl1AndBurstPl2PowerLimit => Get(nameof(TuningView_AdjustCpuSustainedPl1AndBurstPl2PowerLimit));
    public static string TuningView_AdvancedPerformanceTuningUndervoltOvercloc => Get(nameof(TuningView_AdvancedPerformanceTuningUndervoltOvercloc));
    public static string TuningView_AdvancedTuning => Get(nameof(TuningView_AdvancedTuning));
    public static string TuningView_AmdCpuPowerLimits => Get(nameof(TuningView_AmdCpuPowerLimits));
    public static string TuningView_AmdGpuOverclockingAdl2 => Get(nameof(TuningView_AmdGpuOverclockingAdl2));
    public static string TuningView_AmdRadeonGpuOverclockingIsAvailableViaAmdD => Get(nameof(TuningView_AmdRadeonGpuOverclockingIsAvailableViaAmdD));
    public static string TuningView_ApplyAmdCpuPowerLimits => Get(nameof(TuningView_ApplyAmdCpuPowerLimits));
    public static string TuningView_ApplyAmdGpuOc => Get(nameof(TuningView_ApplyAmdGpuOc));
    public static string TuningView_ApplyAmdGpuOverclock => Get(nameof(TuningView_ApplyAmdGpuOverclock));
    public static string TuningView_ApplyAmdLimits => Get(nameof(TuningView_ApplyAmdLimits));
    public static string TuningView_ApplyCpuPowerLimits => Get(nameof(TuningView_ApplyCpuPowerLimits));
    public static string TuningView_ApplyCpuThermalLimitOffset => Get(nameof(TuningView_ApplyCpuThermalLimitOffset));
    public static string TuningView_ApplyCpuUndervoltOffset => Get(nameof(TuningView_ApplyCpuUndervoltOffset));
    public static string TuningView_ApplyCurrent => Get(nameof(TuningView_ApplyCurrent));
    public static string TuningView_ApplyGpuSettings => Get(nameof(TuningView_ApplyGpuSettings));
    public static string TuningView_ApplyLimit => Get(nameof(TuningView_ApplyLimit));
    public static string TuningView_ApplyMaxRestart => Get(nameof(TuningView_ApplyMaxRestart));
    public static string TuningView_ApplyNvidiaGpuOverclock => Get(nameof(TuningView_ApplyNvidiaGpuOverclock));
    public static string TuningView_ApplyPowerLimits => Get(nameof(TuningView_ApplyPowerLimits));
    public static string TuningView_ApplyTheAmdGpuClockAndPowerOffsetsAboveAnU => Get(nameof(TuningView_ApplyTheAmdGpuClockAndPowerOffsetsAboveAnU));
    public static string TuningView_ApplyTheAmdStapmTemperatureLimitsAbove => Get(nameof(TuningView_ApplyTheAmdStapmTemperatureLimitsAbove));
    public static string TuningView_ApplyTheCpuPl1Pl2PowerLimitsAboveHigherLim => Get(nameof(TuningView_ApplyTheCpuPl1Pl2PowerLimitsAboveHigherLim));
    public static string TuningView_ApplyTheCpuThermalThrottleLimitAboveTheCpu => Get(nameof(TuningView_ApplyTheCpuThermalThrottleLimitAboveTheCpu));
    public static string TuningView_ApplyTheCpuVoltageOffsetAboveAnOffsetThatI => Get(nameof(TuningView_ApplyTheCpuVoltageOffsetAboveAnOffsetThatI));
    public static string TuningView_ApplyTheGpuClockMemoryAndPowerOffsetsAbove => Get(nameof(TuningView_ApplyTheGpuClockMemoryAndPowerOffsetsAbove));
    public static string TuningView_ApplyTheseGpuSettingsTemporarilySoYouCanCh => Get(nameof(TuningView_ApplyTheseGpuSettingsTemporarilySoYouCanCh));
    public static string TuningView_BurstPowerForTurboHigherMorePerformanceHea => Get(nameof(TuningView_BurstPowerForTurboHigherMorePerformanceHea));
    public static string TuningView_ClockOffsetsNotAvailable => Get(nameof(TuningView_ClockOffsetsNotAvailable));
    public static string TuningView_ConfirmTheSettingsYouAreCurrentlyTestingAn => Get(nameof(TuningView_ConfirmTheSettingsYouAreCurrentlyTestingAn));
    public static string TuningView_ControlAmdRyzenStapmPowerAndTctlTemperatur => Get(nameof(TuningView_ControlAmdRyzenStapmPowerAndTctlTemperatur));
    public static string TuningView_ControlsSustainedPackagePowerWhile => Get(nameof(TuningView_ControlsSustainedPackagePowerWhile));
    public static string TuningView_CoreClock => Get(nameof(TuningView_CoreClock));
    public static string TuningView_CoreClockOffsetMhz => Get(nameof(TuningView_CoreClockOffsetMhz));
    public static string TuningView_CoreOffset => Get(nameof(TuningView_CoreOffset));
    public static string TuningView_CpuGpuTuning => Get(nameof(TuningView_CpuGpuTuning));
    public static string TuningView_CpuPowerLimitsPl1Pl2 => Get(nameof(TuningView_CpuPowerLimitsPl1Pl2));
    public static string TuningView_CpuTuning => Get(nameof(TuningView_CpuTuning));
    public static string TuningView_CpuUndervoltIsNotAvailable => Get(nameof(TuningView_CpuUndervoltIsNotAvailable));
    public static string TuningView_CpuUndervoltIsNotAvailableOnThisMachine => Get(nameof(TuningView_CpuUndervoltIsNotAvailableOnThisMachine));
    public static string TuningView_CReduction => Get(nameof(TuningView_CReduction));
    public static string TuningView_CurrentlyEnforced => Get(nameof(TuningView_CurrentlyEnforced));
    public static string TuningView_Default100CTjmax => Get(nameof(TuningView_Default100CTjmax));
    public static string TuningView_DetectedGpuGuardrails => Get(nameof(TuningView_DetectedGpuGuardrails));
    public static string TuningView_Driver => Get(nameof(TuningView_Driver));
    public static string TuningView_EnableNvidiaPowerUnlock => Get(nameof(TuningView_EnableNvidiaPowerUnlock));
    public static string TuningView_ForcesLowerVoltageAtEachFrequencyOrUse => Get(nameof(TuningView_ForcesLowerVoltageAtEachFrequencyOrUse));
    public static string TuningView_ForClockOcUseMsiAfterburnerOrHpOmenHub => Get(nameof(TuningView_ForClockOcUseMsiAfterburnerOrHpOmenHub));
    public static string TuningView_FromFile => Get(nameof(TuningView_FromFile));
    public static string TuningView_GpuPowerPolicyReadOnly => Get(nameof(TuningView_GpuPowerPolicyReadOnly));
    public static string TuningView_GpuTuning => Get(nameof(TuningView_GpuTuning));
    public static string TuningView_GpuUndervolting => Get(nameof(TuningView_GpuUndervolting));
    public static string TuningView_ImproperValuesMayCauseCrashesOrFreezesStar => Get(nameof(TuningView_ImproperValuesMayCauseCrashesOrFreezesStar));
    public static string TuningView_KeepTestedGpuOverclock => Get(nameof(TuningView_KeepTestedGpuOverclock));
    public static string TuningView_LongTermTdp => Get(nameof(TuningView_LongTermTdp));
    public static string TuningView_ManualApplyOnlyAmdOffsetsAreNotSavedAndDoN => Get(nameof(TuningView_ManualApplyOnlyAmdOffsetsAreNotSavedAndDoN));
    public static string TuningView_MemClock => Get(nameof(TuningView_MemClock));
    public static string TuningView_MemoryClockOffsetMhz => Get(nameof(TuningView_MemoryClockOffsetMhz));
    public static string TuningView_ModernNvidiaGpusAutoManageVoltageToReduceP => Get(nameof(TuningView_ModernNvidiaGpusAutoManageVoltageToReduceP));
    public static string TuningView_MsrRegistersAreWriteLockedUntilRebootMayRe => Get(nameof(TuningView_MsrRegistersAreWriteLockedUntilRebootMayRe));
    public static string TuningView_N1ResolveVbios => Get(nameof(TuningView_N1ResolveVbios));
    public static string TuningView_NegativeCoreOffset => Get(nameof(TuningView_NegativeCoreOffset));
    public static string TuningView_NotesDoNotDisablePpabDynamicBoostTheAdapte => Get(nameof(TuningView_NotesDoNotDisablePpabDynamicBoostTheAdapte));
    public static string TuningView_NoTuningFeaturesAvailable => Get(nameof(TuningView_NoTuningFeaturesAvailable));
    public static string TuningView_NvidiaPowerUnlockAdvancedOffByDefault => Get(nameof(TuningView_NvidiaPowerUnlockAdvancedOffByDefault));
    public static string TuningView_Order1ResolveVbios2ApplyMax3RestartWindows => Get(nameof(TuningView_Order1ResolveVbios2ApplyMax3RestartWindows));
    public static string TuningView_Pl1Sustained => Get(nameof(TuningView_Pl1Sustained));
    public static string TuningView_Pl2Burst => Get(nameof(TuningView_Pl2Burst));
    public static string TuningView_PowerLimit => Get(nameof(TuningView_PowerLimit));
    public static string TuningView_PowerLimit_2 => Get(nameof(TuningView_PowerLimit_2));
    public static string TuningView_PowerLimit_3 => Get(nameof(TuningView_PowerLimit_3));
    public static string TuningView_PowerLimitLocked => Get(nameof(TuningView_PowerLimitLocked));
    public static string TuningView_PowerLimitsLockedByBios => Get(nameof(TuningView_PowerLimitsLockedByBios));
    public static string TuningView_ReadsTheVbiosPowerTableNeedsNvflash64ExeNv => Get(nameof(TuningView_ReadsTheVbiosPowerTableNeedsNvflash64ExeNv));
    public static string TuningView_ReadTheFourPowerLimitsBackOutOfTheSmuDoesN => Get(nameof(TuningView_ReadTheFourPowerLimitsBackOutOfTheSmuDoesN));
    public static string TuningView_ReduceCpuTjmaxThrottlingTemperature => Get(nameof(TuningView_ReduceCpuTjmaxThrottlingTemperature));
    public static string TuningView_ReducesBoostClocksLessVoltageNeededForVFCu => Get(nameof(TuningView_ReducesBoostClocksLessVoltageNeededForVFCu));
    public static string TuningView_Refresh => Get(nameof(TuningView_Refresh));
    public static string TuningView_RefreshCpuPowerLimitReadings => Get(nameof(TuningView_RefreshCpuPowerLimitReadings));
    public static string TuningView_RefreshEnforcedAmdCpuPowerLimits => Get(nameof(TuningView_RefreshEnforcedAmdCpuPowerLimits));
    public static string TuningView_RemoveCpuThermalLimitOffset => Get(nameof(TuningView_RemoveCpuThermalLimitOffset));
    public static string TuningView_RemoveLimit => Get(nameof(TuningView_RemoveLimit));
    public static string TuningView_RemoveMaxOverride => Get(nameof(TuningView_RemoveMaxOverride));
    public static string TuningView_RemoveTheCpuVoltageOffsetReturningTheCpuTo => Get(nameof(TuningView_RemoveTheCpuVoltageOffsetReturningTheCpuTo));
    public static string TuningView_RemoveTheThermalThrottleLimitReturningTheC => Get(nameof(TuningView_RemoveTheThermalThrottleLimitReturningTheC));
    public static string TuningView_ReReadTheCurrentCpuPowerLimitsFromTheHardw => Get(nameof(TuningView_ReReadTheCurrentCpuPowerLimitsFromTheHardw));
    public static string TuningView_ResetAmdCpuPowerLimitsToDefaults => Get(nameof(TuningView_ResetAmdCpuPowerLimitsToDefaults));
    public static string TuningView_ResetAmdGpuOverclockToDefaults => Get(nameof(TuningView_ResetAmdGpuOverclockToDefaults));
    public static string TuningView_ResetCpuPowerLimitsToDefaults => Get(nameof(TuningView_ResetCpuPowerLimitsToDefaults));
    public static string TuningView_ResetCpuUndervoltTo0Millivolts => Get(nameof(TuningView_ResetCpuUndervoltTo0Millivolts));
    public static string TuningView_ResetLiveTuningWhereAvailableAndNormalizeS => Get(nameof(TuningView_ResetLiveTuningWhereAvailableAndNormalizeS));
    public static string TuningView_ResetNvidiaGpuOverclockToDefaults => Get(nameof(TuningView_ResetNvidiaGpuOverclockToDefaults));
    public static string TuningView_ResetTo0mv => Get(nameof(TuningView_ResetTo0mv));
    public static string TuningView_ResetToDefault => Get(nameof(TuningView_ResetToDefault));
    public static string TuningView_RestoreAmdGpuClocksAndPowerToStockValues => Get(nameof(TuningView_RestoreAmdGpuClocksAndPowerToStockValues));
    public static string TuningView_RestoreGpuClocksMemoryAndPowerLimitToStock => Get(nameof(TuningView_RestoreGpuClocksMemoryAndPowerLimitToStock));
    public static string TuningView_RestoreTheAmdCpuPowerLimitsToThisModelSSto => Get(nameof(TuningView_RestoreTheAmdCpuPowerLimitsToThisModelSSto));
    public static string TuningView_RestoreTheCpuPowerLimitsToThisModelSStockV => Get(nameof(TuningView_RestoreTheCpuPowerLimitsToThisModelSStockV));
    public static string TuningView_RollBackAllTuningToSafeDefaults => Get(nameof(TuningView_RollBackAllTuningToSafeDefaults));
    public static string TuningView_SafeRollback => Get(nameof(TuningView_SafeRollback));
    public static string TuningView_SetsTheThermalThrottleTargetLowerValuesRed => Get(nameof(TuningView_SetsTheThermalThrottleTargetLowerValuesRed));
    public static string TuningView_Status => Get(nameof(TuningView_Status));
    public static string TuningView_SustainedPower => Get(nameof(TuningView_SustainedPower));
    public static string TuningView_SustainedTdp => Get(nameof(TuningView_SustainedTdp));
    public static string TuningView_TargetW => Get(nameof(TuningView_TargetW));
    public static string TuningView_TccOffset => Get(nameof(TuningView_TccOffset));
    public static string TuningView_TccOffsetThermalThrottlePoint => Get(nameof(TuningView_TccOffsetThermalThrottlePoint));
    public static string TuningView_TctlLimit => Get(nameof(TuningView_TctlLimit));
    public static string TuningView_TestApply => Get(nameof(TuningView_TestApply));
    public static string TuningView_TestGpuOverclockTemporarily => Get(nameof(TuningView_TestGpuOverclockTemporarily));
    public static string TuningView_ThrottleAt70C => Get(nameof(TuningView_ThrottleAt70C));
    public static string TuningView_ThrottleAt85C => Get(nameof(TuningView_ThrottleAt85C));
    public static string TuningView_ThrottlePoint => Get(nameof(TuningView_ThrottlePoint));
    public static string TuningView_TuningStatus => Get(nameof(TuningView_TuningStatus));
    public static string TuningView_TurboPower => Get(nameof(TuningView_TurboPower));
    public static string TuningView_WhatTheNvidiaDriverIsEnforcingRightNowALap => Get(nameof(TuningView_WhatTheNvidiaDriverIsEnforcingRightNowALap));
    public static string TuningView_WhetherTheConfirmedGpuOcProfileIsCurrently => Get(nameof(TuningView_WhetherTheConfirmedGpuOcProfileIsCurrently));
    public static string TuningView_YourGpuDoesnTSupportClockOffsetAdjustments => Get(nameof(TuningView_YourGpuDoesnTSupportClockOffsetAdjustments));
    public static string TuningView_YourSystemDoesnTSupportCpuUndervoltingPowe => Get(nameof(TuningView_YourSystemDoesnTSupportCpuUndervoltingPowe));

    // Auto-extracted literals from AboutWindow.xaml
    public static string AboutWindow_Close => Get(nameof(AboutWindow_Close));

    // Auto-extracted literals from AdvancedView.xaml
    public static string AdvancedView_Apply => Get(nameof(AdvancedView_Apply));
    public static string AdvancedView_Quiet => Get(nameof(AdvancedView_Quiet));

    // Auto-extracted literals from BloatwareManagerView.xaml
    public static string BloatwareManagerView_All => Get(nameof(BloatwareManagerView_All));
    public static string BloatwareManagerView_App => Get(nameof(BloatwareManagerView_App));
    public static string BloatwareManagerView_Est => Get(nameof(BloatwareManagerView_Est));
    public static string BloatwareManagerView_High => Get(nameof(BloatwareManagerView_High));
    public static string BloatwareManagerView_Low => Get(nameof(BloatwareManagerView_Low));
    public static string BloatwareManagerView_Med => Get(nameof(BloatwareManagerView_Med));
    public static string BloatwareManagerView_Name => Get(nameof(BloatwareManagerView_Name));
    public static string BloatwareManagerView_Remove => Get(nameof(BloatwareManagerView_Remove));
    public static string BloatwareManagerView_Risk_X => Get(nameof(BloatwareManagerView_Risk_X));
    public static string BloatwareManagerView_Scan => Get(nameof(BloatwareManagerView_Scan));
    public static string BloatwareManagerView_Type => Get(nameof(BloatwareManagerView_Type));

    // Auto-extracted literals from DashboardView.xaml
    public static string DashboardView_Power => Get(nameof(DashboardView_Power));

    // Auto-extracted literals from DiagnosticExportControl.xaml
    public static string DiagnosticExportControl_Cancel => Get(nameof(DiagnosticExportControl_Cancel));

    // Auto-extracted literals from FanCalibrationControl.xaml
    public static string FanCalibrationControl_Status => Get(nameof(FanCalibrationControl_Status));
    public static string FanCalibrationControl_Stop => Get(nameof(FanCalibrationControl_Stop));

    // Auto-extracted literals from FanControlView.xaml
    public static string FanControlView_Add => Get(nameof(FanControlView_Add));
    public static string FanControlView_Auto => Get(nameof(FanControlView_Auto));
    public static string FanControlView_Custom => Get(nameof(FanControlView_Custom));
    public static string FanControlView_Direct => Get(nameof(FanControlView_Direct));
    public static string FanControlView_Export => Get(nameof(FanControlView_Export));
    public static string FanControlView_Gaming => Get(nameof(FanControlView_Gaming));
    public static string FanControlView_Import => Get(nameof(FanControlView_Import));
    public static string FanControlView_Max => Get(nameof(FanControlView_Max));
    public static string FanControlView_Ms => Get(nameof(FanControlView_Ms));
    public static string FanControlView_Save => Get(nameof(FanControlView_Save));
    public static string FanControlView_Silent => Get(nameof(FanControlView_Silent));

    // Auto-extracted literals from GameLibraryView.xaml
    public static string GameLibraryView_Launch => Get(nameof(GameLibraryView_Launch));

    // Auto-extracted literals from GameProfileManagerView.xaml
    public static string GameProfileManagerView_Delete => Get(nameof(GameProfileManagerView_Delete));

    // Auto-extracted literals from GeneralView.xaml
    public static string GeneralView_Load => Get(nameof(GeneralView_Load));
    public static string GeneralView_Manual => Get(nameof(GeneralView_Manual));
    public static string GeneralView_Ram => Get(nameof(GeneralView_Ram));
    public static string GeneralView_System => Get(nameof(GeneralView_System));
    public static string GeneralView_Temp => Get(nameof(GeneralView_Temp));
    public static string GeneralView_Used => Get(nameof(GeneralView_Used));

    // Auto-extracted literals from HardwareMonitoringDashboard.xaml
    public static string HardwareMonitoringDashboard_Cycles => Get(nameof(HardwareMonitoringDashboard_Cycles));
    public static string HardwareMonitoringDashboard_Good => Get(nameof(HardwareMonitoringDashboard_Good));
    public static string HardwareMonitoringDashboard_Mhz => Get(nameof(HardwareMonitoringDashboard_Mhz));
    public static string HardwareMonitoringDashboard_Normal => Get(nameof(HardwareMonitoringDashboard_Normal));
    public static string HardwareMonitoringDashboard_Watts => Get(nameof(HardwareMonitoringDashboard_Watts));

    // Auto-extracted literals from InputPromptWindow.xaml
    public static string InputPromptWindow_Input => Get(nameof(InputPromptWindow_Input));
    public static string InputPromptWindow_Prompt => Get(nameof(InputPromptWindow_Prompt));

    // Auto-extracted literals from LightingView.xaml
    public static string LightingView_Blue => Get(nameof(LightingView_Blue));
    public static string LightingView_Colour => Get(nameof(LightingView_Colour));
    public static string LightingView_Cyan => Get(nameof(LightingView_Cyan));
    public static string LightingView_Effect => Get(nameof(LightingView_Effect));
    public static string LightingView_Green => Get(nameof(LightingView_Green));
    public static string LightingView_Leds => Get(nameof(LightingView_Leds));
    public static string LightingView_Medium => Get(nameof(LightingView_Medium));
    public static string LightingView_Mode => Get(nameof(LightingView_Mode));
    public static string LightingView_Off => Get(nameof(LightingView_Off));
    public static string LightingView_Orange => Get(nameof(LightingView_Orange));
    public static string LightingView_Pink => Get(nameof(LightingView_Pink));
    public static string LightingView_Probe => Get(nameof(LightingView_Probe));
    public static string LightingView_Purple => Get(nameof(LightingView_Purple));
    public static string LightingView_Red => Get(nameof(LightingView_Red));
    public static string LightingView_Speed => Get(nameof(LightingView_Speed));
    public static string LightingView_Theme => Get(nameof(LightingView_Theme));
    public static string LightingView_White => Get(nameof(LightingView_White));
    public static string LightingView_Yellow => Get(nameof(LightingView_Yellow));

    // Auto-extracted literals from MainWindow.xaml
    public static string MainWindow_About => Get(nameof(MainWindow_About));
    public static string MainWindow_GotIt => Get(nameof(MainWindow_GotIt));

    // Auto-extracted literals from MemoryOptimizerView.xaml
    public static string MemoryOptimizerView_Add => Get(nameof(MemoryOptimizerView_Add));
    public static string MemoryOptimizerView_Clean => Get(nameof(MemoryOptimizerView_Clean));
    public static string MemoryOptimizerView_Commit => Get(nameof(MemoryOptimizerView_Commit));
    public static string MemoryOptimizerView_Safe => Get(nameof(MemoryOptimizerView_Safe));
    public static string MemoryOptimizerView_Toggle => Get(nameof(MemoryOptimizerView_Toggle));

    // Auto-extracted literals from OsdOverlayWindow.xaml
    public static string OsdOverlayWindow_Mbps => Get(nameof(OsdOverlayWindow_Mbps));

    // Auto-extracted literals from QuickPopupWindow.xaml
    public static string QuickPopupWindow_Curve => Get(nameof(QuickPopupWindow_Curve));
    public static string QuickPopupWindow_N165hz => Get(nameof(QuickPopupWindow_N165hz));

    // Auto-extracted literals from SettingsView.xaml
    public static string SettingsView_Daily => Get(nameof(SettingsView_Daily));
    public static string SettingsView_Driver => Get(nameof(SettingsView_Driver));
    public static string SettingsView_Fans => Get(nameof(SettingsView_Fans));
    public static string SettingsView_Github_X => Get(nameof(SettingsView_Github_X));
    public static string SettingsView_Layout => Get(nameof(SettingsView_Layout));
    public static string SettingsView_Sensor => Get(nameof(SettingsView_Sensor));
    public static string SettingsView_Tuning => Get(nameof(SettingsView_Tuning));
    public static string SettingsView_Weekly => Get(nameof(SettingsView_Weekly));

    // Auto-extracted literals from SystemOptimizerView.xaml
    public static string SystemOptimizerView_Open => Get(nameof(SystemOptimizerView_Open));

    // Auto-extracted literals from TuningView.xaml
    public static string TuningView_Keep => Get(nameof(TuningView_Keep));

    // Auto-extracted literals from AboutWindow.xaml

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from BloatwareManagerView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from DiagnosticExportControl.xaml

    // Auto-extracted literals from FanCalibrationControl.xaml

    // Auto-extracted literals from FanControlView.xaml

    // Auto-extracted literals from GameLibraryView.xaml

    // Auto-extracted literals from GameProfileManagerView.xaml

    // Auto-extracted literals from GeneralView.xaml

    // Auto-extracted literals from HardwareMonitoringDashboard.xaml

    // Auto-extracted literals from InputPromptWindow.xaml

    // Auto-extracted literals from LightingView.xaml

    // Auto-extracted literals from MainWindow.xaml

    // Auto-extracted literals from MemoryOptimizerView.xaml

    // Auto-extracted literals from OsdOverlayWindow.xaml

    // Auto-extracted literals from QuickPopupWindow.xaml

    // Auto-extracted literals from SettingsView.xaml

    // Auto-extracted literals from SystemOptimizerView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string FanControlViewModel_BadgeCurves => Get(nameof(FanControlViewModel_BadgeCurves));
    public static string FanControlViewModel_BadgeProfileOnly => Get(nameof(FanControlViewModel_BadgeProfileOnly));
    public static string FanControlViewModel_CalibrateRecalibrate => Get(nameof(FanControlViewModel_CalibrateRecalibrate));
    public static string FanControlViewModel_CalibrateStart => Get(nameof(FanControlViewModel_CalibrateStart));
    public static string FanControlViewModel_CalibrationMapNone => Get(nameof(FanControlViewModel_CalibrationMapNone));
    public static string FanControlViewModel_CalibrationMapOk => Get(nameof(FanControlViewModel_CalibrationMapOk));
    public static string FanControlViewModel_CalibrationMapRefreshed => Get(nameof(FanControlViewModel_CalibrationMapRefreshed));
    public static string FanControlViewModel_CalibrationWizardClosed => Get(nameof(FanControlViewModel_CalibrationWizardClosed));
    public static string FanControlViewModel_CustomCurveTooltipNo => Get(nameof(FanControlViewModel_CustomCurveTooltipNo));
    public static string FanControlViewModel_CustomCurveTooltipOk => Get(nameof(FanControlViewModel_CustomCurveTooltipOk));
    public static string FanControlViewModel_DirectTooltipNo => Get(nameof(FanControlViewModel_DirectTooltipNo));
    public static string FanControlViewModel_DirectTooltipOk => Get(nameof(FanControlViewModel_DirectTooltipOk));
    public static string FanControlViewModel_HeaderHintPresets => Get(nameof(FanControlViewModel_HeaderHintPresets));
    public static string FanControlViewModel_HeaderHintProfileOnly => Get(nameof(FanControlViewModel_HeaderHintProfileOnly));
    public static string FanControlViewModel_LinkBadgeIndependent => Get(nameof(FanControlViewModel_LinkBadgeIndependent));
    public static string FanControlViewModel_LinkBadgeLinked => Get(nameof(FanControlViewModel_LinkBadgeLinked));
    public static string FanControlViewModel_LinkDescIndependent => Get(nameof(FanControlViewModel_LinkDescIndependent));
    public static string FanControlViewModel_LinkDescLinked => Get(nameof(FanControlViewModel_LinkDescLinked));
    public static string FanControlViewModel_OemAutoFailed => Get(nameof(FanControlViewModel_OemAutoFailed));
    public static string FanControlViewModel_OemAutoRestored => Get(nameof(FanControlViewModel_OemAutoRestored));
    public static string FanControlViewModel_StartupHintCategoryOff => Get(nameof(FanControlViewModel_StartupHintCategoryOff));
    public static string FanControlViewModel_StartupHintOff => Get(nameof(FanControlViewModel_StartupHintOff));
    public static string FanControlViewModel_SubtitleEditor => Get(nameof(FanControlViewModel_SubtitleEditor));
    public static string FanControlViewModel_SubtitleProfile => Get(nameof(FanControlViewModel_SubtitleProfile));
    public static string FanControlViewModel_SubtitleProfile80 => Get(nameof(FanControlViewModel_SubtitleProfile80));
    public static string FanControlViewModel_SubtitleUnavailable => Get(nameof(FanControlViewModel_SubtitleUnavailable));
    public static string FanControlViewModel_SubtitleUnavailableDirect => Get(nameof(FanControlViewModel_SubtitleUnavailableDirect));
    public static string HardwareMonitoringDashboard_BatteryCapacityUnavailable => Get(nameof(HardwareMonitoringDashboard_BatteryCapacityUnavailable));
    public static string HardwareMonitoringDashboard_BatteryNotReported => Get(nameof(HardwareMonitoringDashboard_BatteryNotReported));
    public static string HardwareMonitoringDashboard_StateUnavailable => Get(nameof(HardwareMonitoringDashboard_StateUnavailable));
    public static string MainViewModel_CleanupDryDone => Get(nameof(MainViewModel_CleanupDryDone));
    public static string MainViewModel_CleanupDryRunning => Get(nameof(MainViewModel_CleanupDryRunning));
    public static string MainViewModel_CleanupErrors => Get(nameof(MainViewModel_CleanupErrors));
    public static string MainViewModel_CleanupRemoved => Get(nameof(MainViewModel_CleanupRemoved));
    public static string MainViewModel_CleanupRemoving => Get(nameof(MainViewModel_CleanupRemoving));
    public static string MainViewModel_CleanupRestorePoint => Get(nameof(MainViewModel_CleanupRestorePoint));
    public static string MainViewModel_LinkStatusDecoupled => Get(nameof(MainViewModel_LinkStatusDecoupled));
    public static string MainViewModel_LinkStatusLinked => Get(nameof(MainViewModel_LinkStatusLinked));
    public static string MainViewModel_LogsHide => Get(nameof(MainViewModel_LogsHide));
    public static string MainViewModel_LogsShow => Get(nameof(MainViewModel_LogsShow));
    public static string MainViewModel_MonitoringFull => Get(nameof(MainViewModel_MonitoringFull));
    public static string MainViewModel_MonitoringLowOverhead => Get(nameof(MainViewModel_MonitoringLowOverhead));
    public static string QuickPopup_DecoupledMode => Get(nameof(QuickPopup_DecoupledMode));
    public static string QuickPopup_FanPerfLinked => Get(nameof(QuickPopup_FanPerfLinked));
    public static string QuickPopup_LinkedMode => Get(nameof(QuickPopup_LinkedMode));
    public static string TrayIcon_AcPower => Get(nameof(TrayIcon_AcPower));
    public static string TrayIcon_Battery => Get(nameof(TrayIcon_Battery));
    public static string TrayIcon_Charging => Get(nameof(TrayIcon_Charging));
    public static string TrayIcon_Decoupled => Get(nameof(TrayIcon_Decoupled));
    public static string TrayIcon_Enabled => Get(nameof(TrayIcon_Enabled));
    public static string TrayIcon_KeepAboveOff => Get(nameof(TrayIcon_KeepAboveOff));
    public static string TrayIcon_KeepAboveOn => Get(nameof(TrayIcon_KeepAboveOn));
    public static string TrayIcon_Linked => Get(nameof(TrayIcon_Linked));
    public static string TrayIcon_OnBattery => Get(nameof(TrayIcon_OnBattery));
    public static string TrayIcon_StayOnTopOff => Get(nameof(TrayIcon_StayOnTopOff));
    public static string TrayIcon_StayOnTopOn => Get(nameof(TrayIcon_StayOnTopOn));
    public static string TuningStatus_VerifiedAcknowledged => Get(nameof(TuningStatus_VerifiedAcknowledged));
    public static string TuningStatus_VerifiedMismatch => Get(nameof(TuningStatus_VerifiedMismatch));
    public static string TuningStatus_VerifiedNotControlled => Get(nameof(TuningStatus_VerifiedNotControlled));
    public static string TuningStatus_VerifiedReadback => Get(nameof(TuningStatus_VerifiedReadback));
    public static string UpdateViewModel_YouAreRunningLatest => Get(nameof(UpdateViewModel_YouAreRunningLatest));
    public static string UpdateViewModel_YouAreRunningLatestCmp => Get(nameof(UpdateViewModel_YouAreRunningLatestCmp));

    // Auto-extracted literals from AboutWindow.xaml

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from BloatwareManagerView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from DiagnosticExportControl.xaml

    // Auto-extracted literals from FanCalibrationControl.xaml

    // Auto-extracted literals from FanControlView.xaml

    // Auto-extracted literals from GameLibraryView.xaml

    // Auto-extracted literals from GameProfileManagerView.xaml

    // Auto-extracted literals from GeneralView.xaml

    // Auto-extracted literals from HardwareMonitoringDashboard.xaml

    // Auto-extracted literals from InputPromptWindow.xaml

    // Auto-extracted literals from LightingView.xaml

    // Auto-extracted literals from MainWindow.xaml

    // Auto-extracted literals from MemoryOptimizerView.xaml

    // Auto-extracted literals from OsdOverlayWindow.xaml

    // Auto-extracted literals from QuickPopupWindow.xaml

    // Auto-extracted literals from SettingsView.xaml

    // Auto-extracted literals from SystemOptimizerView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string BloatwareManagerViewModel_Scanning => Get(nameof(BloatwareManagerViewModel_Scanning));
    public static string DashboardViewModel_Active => Get(nameof(DashboardViewModel_Active));
    public static string DashboardViewModel_BatteryTelemetryUnavailable => Get(nameof(DashboardViewModel_BatteryTelemetryUnavailable));
    public static string DashboardViewModel_CpuTelemetryUnavailable => Get(nameof(DashboardViewModel_CpuTelemetryUnavailable));
    public static string DashboardViewModel_DgpuIdle => Get(nameof(DashboardViewModel_DgpuIdle));
    public static string DashboardViewModel_FanModeDisplayProp => Get(nameof(DashboardViewModel_FanModeDisplayProp));
    public static string DashboardViewModel_GpuTelemetryUnavailable => Get(nameof(DashboardViewModel_GpuTelemetryUnavailable));
    public static string DashboardViewModel_Idle => Get(nameof(DashboardViewModel_Idle));
    public static string DashboardViewModel_Load => Get(nameof(DashboardViewModel_Load));
    public static string DashboardViewModel_MemoryTelemetryUnavailable => Get(nameof(DashboardViewModel_MemoryTelemetryUnavailable));
    public static string DashboardViewModel_Ssd => Get(nameof(DashboardViewModel_Ssd));
    public static string DashboardViewModel_StorageTelemetryUnavailable => Get(nameof(DashboardViewModel_StorageTelemetryUnavailable));
    public static string FanDiagnosticsViewModel_RgbCheckNotRun => Get(nameof(FanDiagnosticsViewModel_RgbCheckNotRun));
    public static string GameLibraryViewModel_ScanStatus => Get(nameof(GameLibraryViewModel_ScanStatus));
    public static string HardwareMonitoringDashboard_StateInactive => Get(nameof(HardwareMonitoringDashboard_StateInactive));
    public static string HardwareMonitoringDashboard_TrendStable => Get(nameof(HardwareMonitoringDashboard_TrendStable));
    public static string KeyboardDiagnosticsViewModel_DetectionComplete => Get(nameof(KeyboardDiagnosticsViewModel_DetectionComplete));
    public static string LightingViewModel_ActiveTargets => Get(nameof(LightingViewModel_ActiveTargets));
    public static string LightingViewModel_KeyboardRgbNotDetected => Get(nameof(LightingViewModel_KeyboardRgbNotDetected));
    public static string LightingViewModel_NoControllers => Get(nameof(LightingViewModel_NoControllers));
    public static string LightingViewModel_NoSurfaceObservation => Get(nameof(LightingViewModel_NoSurfaceObservation));
    public static string LightingViewModel_NoSyncSession => Get(nameof(LightingViewModel_NoSyncSession));
    public static string LightingViewModel_OpenRgbNotDetected => Get(nameof(LightingViewModel_OpenRgbNotDetected));
    public static string LightingViewModel_RazerNotDetected => Get(nameof(LightingViewModel_RazerNotDetected));
    public static string LightingViewModel_ReadyToRestore => Get(nameof(LightingViewModel_ReadyToRestore));
    public static string LightingViewModel_ReadyToSync => Get(nameof(LightingViewModel_ReadyToSync));
    public static string MemoryOptimizerViewModel_Done => Get(nameof(MemoryOptimizerViewModel_Done));
    public static string MemoryOptimizerViewModel_PhysicalSummary => Get(nameof(MemoryOptimizerViewModel_PhysicalSummary));
    public static string MemoryOptimizerViewModel_TrendSummary => Get(nameof(MemoryOptimizerViewModel_TrendSummary));
    public static string MemoryOptimizerViewModel_VirtualSummary => Get(nameof(MemoryOptimizerViewModel_VirtualSummary));
    public static string PerformanceMode_Performance => Get(nameof(PerformanceMode_Performance));
    public static string SettingsViewModel_AcPowerConnected => Get(nameof(SettingsViewModel_AcPowerConnected));
    public static string SystemControlViewModel_AmdNotDetected => Get(nameof(SystemControlViewModel_AmdNotDetected));
    public static string SystemControlViewModel_AmdNotReadYet => Get(nameof(SystemControlViewModel_AmdNotReadYet));
    public static string SystemControlViewModel_CurveOptimizerTitle => Get(nameof(SystemControlViewModel_CurveOptimizerTitle));
    public static string SystemControlViewModel_DetectedLimits => Get(nameof(SystemControlViewModel_DetectedLimits));
    public static string SystemControlViewModel_MsrLimitsUnavailable => Get(nameof(SystemControlViewModel_MsrLimitsUnavailable));
    public static string SystemControlViewModel_MsrLimitsUnavailable8D2F => Get(nameof(SystemControlViewModel_MsrLimitsUnavailable8D2F));
    public static string SystemControlViewModel_NoRecentUndervolt => Get(nameof(SystemControlViewModel_NoRecentUndervolt));
    public static string SystemControlViewModel_NvapiGuidance => Get(nameof(SystemControlViewModel_NvapiGuidance));
    public static string SystemControlViewModel_NvapiWiderRange => Get(nameof(SystemControlViewModel_NvapiWiderRange));
    public static string SystemControlViewModel_PowerLimitLocked => Get(nameof(SystemControlViewModel_PowerLimitLocked));
    public static string SystemControlViewModel_TestApplyCpu => Get(nameof(SystemControlViewModel_TestApplyCpu));
    public static string SystemControlViewModel_TestApplyCpu30 => Get(nameof(SystemControlViewModel_TestApplyCpu30));
    public static string SystemControlViewModel_TestApplyGpu => Get(nameof(SystemControlViewModel_TestApplyGpu));
    public static string SystemControlViewModel_TestApplyGpu30 => Get(nameof(SystemControlViewModel_TestApplyGpu30));
    public static string SystemControlViewModel_UndervoltBackend => Get(nameof(SystemControlViewModel_UndervoltBackend));
    public static string SystemControlViewModel_UndervoltHelp => Get(nameof(SystemControlViewModel_UndervoltHelp));
    public static string SystemOptimizerViewModel_NoPreflightWarnings => Get(nameof(SystemOptimizerViewModel_NoPreflightWarnings));
    public static string SystemOptimizerViewModel_OptimizationsActive => Get(nameof(SystemOptimizerViewModel_OptimizationsActive));
    public static string SystemOptimizerViewModel_PreflightNotLoaded => Get(nameof(SystemOptimizerViewModel_PreflightNotLoaded));
    public static string SystemOptimizerViewModel_VerificationPending => Get(nameof(SystemOptimizerViewModel_VerificationPending));
    public static string TuningStatus_VerifiedBlocked => Get(nameof(TuningStatus_VerifiedBlocked));
    public static string TuningStatus_VerifiedFailed => Get(nameof(TuningStatus_VerifiedFailed));
    public static string TuningStatus_VerifiedNearestStep => Get(nameof(TuningStatus_VerifiedNearestStep));
    public static string TuningStatus_VerifiedWarning => Get(nameof(TuningStatus_VerifiedWarning));

    // Auto-extracted literals from AdvancedView.xaml
    public static string AdvancedView_Active => Get(nameof(AdvancedView_Active));

    // Auto-extracted literals from DashboardView.xaml
    public static string DashboardView_Memory => Get(nameof(DashboardView_Memory));

    // Auto-extracted literals from TuningView.xaml
    public static string TuningView_Clock => Get(nameof(TuningView_Clock));
    public static string TuningView_Core => Get(nameof(TuningView_Core));

    // Auto-extracted literals from 

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string GameLibraryViewModel_Scanning => Get(nameof(GameLibraryViewModel_Scanning));
    public static string PerformanceMode_Balanced => Get(nameof(PerformanceMode_Balanced));
    public static string PerformanceMode_Quiet => Get(nameof(PerformanceMode_Quiet));
    public static string SettingsViewModel_OnBattery => Get(nameof(SettingsViewModel_OnBattery));
    public static string SystemControlViewModel_UndervoltingTitle => Get(nameof(SystemControlViewModel_UndervoltingTitle));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from DataTrigger setters
    public static string Setter_Skipped => Get(nameof(Setter_Skipped));
    public static string Setter_Failed => Get(nameof(Setter_Failed));
    public static string Setter_Enabled => Get(nameof(Setter_Enabled));
    public static string Setter_Installed_2 => Get(nameof(Setter_Installed_2));
    public static string Setter_Profile => Get(nameof(Setter_Profile));
    public static string Setter_NotDetected => Get(nameof(Setter_NotDetected));
    public static string Setter_NoProfile => Get(nameof(Setter_NoProfile));
    public static string Setter_Running => Get(nameof(Setter_Running));
    public static string Setter_SecureBootIsDisabledLegacyUnsignedDriversA => Get(nameof(Setter_SecureBootIsDisabledLegacyUnsignedDriversA));
    public static string Setter_UsingLegacyWmiBiosF15Fallback => Get(nameof(Setter_UsingLegacyWmiBiosF15Fallback));
    public static string Setter_Removed => Get(nameof(Setter_Removed));
    public static string Setter_SecureBootEnabledUnsignedDriversBlockedPaw => Get(nameof(Setter_SecureBootEnabledUnsignedDriversBlockedPaw));
    public static string Setter_Installed => Get(nameof(Setter_Installed));
    public static string Setter_Disabled => Get(nameof(Setter_Disabled));
    public static string Setter_NoEc => Get(nameof(Setter_NoEc));
    public static string Setter_NotInstalled => Get(nameof(Setter_NotInstalled));
    public static string Setter_EcErr => Get(nameof(Setter_EcErr));

    // Auto-extracted literals from 
    public static string LightingViewModel_ActiveScene => Get(nameof(LightingViewModel_ActiveScene));
    public static string LightingViewModel_IcueNotDetected => Get(nameof(LightingViewModel_IcueNotDetected));
    public static string LightingViewModel_Level => Get(nameof(LightingViewModel_Level));
    public static string LightingViewModel_LogitechNotDetected => Get(nameof(LightingViewModel_LogitechNotDetected));
    public static string LightingViewModel_NotRecorded => Get(nameof(LightingViewModel_NotRecorded));
    public static string LightingViewModel_RazerNotConfigured => Get(nameof(LightingViewModel_RazerNotConfigured));
    public static string SystemControlViewModel_RollbackNotRun => Get(nameof(SystemControlViewModel_RollbackNotRun));
    public static string SystemControlViewModel_TuningDiagLine => Get(nameof(SystemControlViewModel_TuningDiagLine));
    public static string SystemControlViewModel_TuningStatusLine => Get(nameof(SystemControlViewModel_TuningStatusLine));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string BloatwareManagerViewModel_CancelRequested => Get(nameof(BloatwareManagerViewModel_CancelRequested));
    public static string BloatwareManagerViewModel_CreatingRestorePoint => Get(nameof(BloatwareManagerViewModel_CreatingRestorePoint));
    public static string BloatwareManagerViewModel_RestoreSkipped => Get(nameof(BloatwareManagerViewModel_RestoreSkipped));
    public static string LightingViewModel_IcueNotConfigured => Get(nameof(LightingViewModel_IcueNotConfigured));
    public static string LightingViewModel_LogitechNotConfigured => Get(nameof(LightingViewModel_LogitechNotConfigured));
    public static string SystemControlViewModel_ConflictDetected => Get(nameof(SystemControlViewModel_ConflictDetected));
    public static string SystemControlViewModel_ConflictNone => Get(nameof(SystemControlViewModel_ConflictNone));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string SettingsViewModel_AuditFailed => Get(nameof(SettingsViewModel_AuditFailed));
    public static string SettingsViewModel_ConfidenceHigh => Get(nameof(SettingsViewModel_ConfidenceHigh));
    public static string SettingsViewModel_ConfidenceLow => Get(nameof(SettingsViewModel_ConfidenceLow));
    public static string SettingsViewModel_ConfidenceMedium => Get(nameof(SettingsViewModel_ConfidenceMedium));
    public static string SettingsViewModel_NoRecentResumeActivity => Get(nameof(SettingsViewModel_NoRecentResumeActivity));
    public static string SettingsViewModel_NoRecentResumeEvent => Get(nameof(SettingsViewModel_NoRecentResumeEvent));
    public static string SettingsViewModel_NoTimelineRecorded => Get(nameof(SettingsViewModel_NoTimelineRecorded));
    public static string SettingsViewModel_NoTimelineSummary => Get(nameof(SettingsViewModel_NoTimelineSummary));
    public static string SettingsViewModel_SourceDefaultFallback => Get(nameof(SettingsViewModel_SourceDefaultFallback));
    public static string SettingsViewModel_SourceNoDatabaseMatch => Get(nameof(SettingsViewModel_SourceNoDatabaseMatch));
    public static string SettingsViewModel_StandaloneFully => Get(nameof(SettingsViewModel_StandaloneFully));
    public static string SettingsViewModel_StandaloneOptionalDegradation => Get(nameof(SettingsViewModel_StandaloneOptionalDegradation));
    public static string SettingsViewModel_StatusDegraded => Get(nameof(SettingsViewModel_StatusDegraded));
    public static string SettingsViewModel_StatusError => Get(nameof(SettingsViewModel_StatusError));
    public static string SettingsViewModel_StatusHealthy => Get(nameof(SettingsViewModel_StatusHealthy));
    public static string SettingsViewModel_ValueUnknown => Get(nameof(SettingsViewModel_ValueUnknown));
    public static string SettingsViewModel_WarnKeyboardNoMatch => Get(nameof(SettingsViewModel_WarnKeyboardNoMatch));
    public static string SettingsViewModel_WarnNoModelMatch => Get(nameof(SettingsViewModel_WarnNoModelMatch));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string SystemControlViewModel_NoRecovery => Get(nameof(SystemControlViewModel_NoRecovery));
    public static string SystemControlViewModel_NvapiReady => Get(nameof(SystemControlViewModel_NvapiReady));
    public static string SystemControlViewModel_NvapiUnavailable => Get(nameof(SystemControlViewModel_NvapiUnavailable));
    public static string SystemControlViewModel_RecoveryApplied => Get(nameof(SystemControlViewModel_RecoveryApplied));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string DashboardViewModel_HealthDegraded => Get(nameof(DashboardViewModel_HealthDegraded));
    public static string DashboardViewModel_HealthHealthy => Get(nameof(DashboardViewModel_HealthHealthy));
    public static string DashboardViewModel_HealthStale => Get(nameof(DashboardViewModel_HealthStale));
    public static string DashboardViewModel_HealthUnknown => Get(nameof(DashboardViewModel_HealthUnknown));
    public static string DashboardViewModel_JustNow => Get(nameof(DashboardViewModel_JustNow));
    public static string DashboardViewModel_MinutesAgo => Get(nameof(DashboardViewModel_MinutesAgo));
    public static string DashboardViewModel_NoData => Get(nameof(DashboardViewModel_NoData));
    public static string DashboardViewModel_SecondsAgo => Get(nameof(DashboardViewModel_SecondsAgo));
    public static string HardwareMonitoringDashboard_StatsFormat => Get(nameof(HardwareMonitoringDashboard_StatsFormat));
    public static string LightingViewModel_KeyboardLightingUnavailable => Get(nameof(LightingViewModel_KeyboardLightingUnavailable));
    public static string MainViewModel_OverrideActive => Get(nameof(MainViewModel_OverrideActive));
    public static string MainViewModel_OverrideInactive => Get(nameof(MainViewModel_OverrideInactive));
    public static string MemoryOptimizerViewModel_GameAwareOff => Get(nameof(MemoryOptimizerViewModel_GameAwareOff));
    public static string MemoryOptimizerViewModel_GameAwareOn => Get(nameof(MemoryOptimizerViewModel_GameAwareOn));
    public static string SystemControlViewModel_ApplyCurveOptimizer => Get(nameof(SystemControlViewModel_ApplyCurveOptimizer));
    public static string SystemControlViewModel_ApplyUndervolt => Get(nameof(SystemControlViewModel_ApplyUndervolt));
    public static string TempSourceDiag_Checking => Get(nameof(TempSourceDiag_Checking));
    public static string TempSourceDiag_CheckSources => Get(nameof(TempSourceDiag_CheckSources));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string DashboardViewModel_MonitoringSourceSelfSustaining => Get(nameof(DashboardViewModel_MonitoringSourceSelfSustaining));
    public static string MainViewModel_RpmReadbackLabel => Get(nameof(MainViewModel_RpmReadbackLabel));
    public static string Monitor_FanEfficientFormat => Get(nameof(Monitor_FanEfficientFormat));
    public static string Monitor_QualityExcellent => Get(nameof(Monitor_QualityExcellent));
    public static string Monitor_TrendFlat => Get(nameof(Monitor_TrendFlat));
    public static string SystemControlViewModel_CurrentPl1 => Get(nameof(SystemControlViewModel_CurrentPl1));
    public static string SystemControlViewModel_CurrentPl2 => Get(nameof(SystemControlViewModel_CurrentPl2));
    public static string SystemControlViewModel_SecondChannelHeaderCache => Get(nameof(SystemControlViewModel_SecondChannelHeaderCache));
    public static string SystemControlViewModel_SecondChannelSubtitleCache => Get(nameof(SystemControlViewModel_SecondChannelSubtitleCache));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    public static string DashboardViewModel_SourceWmiNvapiSelfSustaining => Get(nameof(DashboardViewModel_SourceWmiNvapiSelfSustaining));
    public static string DashboardViewModel_SourceWmiSelfSustaining => Get(nameof(DashboardViewModel_SourceWmiSelfSustaining));
    public static string DiagnosticsView_EstimatedFromFanLevel => Get(nameof(DiagnosticsView_EstimatedFromFanLevel));
    public static string Monitor_QualityFair => Get(nameof(Monitor_QualityFair));
    public static string Monitor_QualityGood => Get(nameof(Monitor_QualityGood));
    public static string Monitor_QualityPoor => Get(nameof(Monitor_QualityPoor));
    public static string Monitor_TempCritical => Get(nameof(Monitor_TempCritical));
    public static string Monitor_TempHot => Get(nameof(Monitor_TempHot));
    public static string Monitor_TempNormal => Get(nameof(Monitor_TempNormal));
    public static string Monitor_TempWarm => Get(nameof(Monitor_TempWarm));
    public static string Monitor_ThermalCool => Get(nameof(Monitor_ThermalCool));
    public static string Monitor_ThermalOptimal => Get(nameof(Monitor_ThermalOptimal));
    public static string Monitor_TrendDown => Get(nameof(Monitor_TrendDown));
    public static string Monitor_TrendUp => Get(nameof(Monitor_TrendUp));
    public static string SystemControlViewModel_CacheOffset => Get(nameof(SystemControlViewModel_CacheOffset));
    public static string SystemControlViewModel_CurrentFormat => Get(nameof(SystemControlViewModel_CurrentFormat));
    public static string SystemControlViewModel_IgpuOffset => Get(nameof(SystemControlViewModel_IgpuOffset));
    public static string SystemControlViewModel_L3Cache => Get(nameof(SystemControlViewModel_L3Cache));
    public static string SystemControlViewModel_RadeonGraphics => Get(nameof(SystemControlViewModel_RadeonGraphics));

    public static string MemoryOptimizerView_NativeApiNote => Get(nameof(MemoryOptimizerView_NativeApiNote));
    public static string SystemOptimizerView_ImportantNotesBody => Get(nameof(SystemOptimizerView_ImportantNotesBody));

    // Auto-extracted literals from 

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from
    public static string DashboardViewModel_CpuAuthorityLhm => Get(nameof(DashboardViewModel_CpuAuthorityLhm));
    public static string DashboardViewModel_CpuAuthorityWmi => Get(nameof(DashboardViewModel_CpuAuthorityWmi));
    public static string SystemOptimizerViewModel_VerifyingState => Get(nameof(SystemOptimizerViewModel_VerifyingState));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    public static string MemoryOptimizerViewModel_Disabled => Get(nameof(MemoryOptimizerViewModel_Disabled));
    public static string MemoryOptimizerViewModel_Enabled => Get(nameof(MemoryOptimizerViewModel_Enabled));
    public static string MemoryOptimizerViewModel_StateUnavailable => Get(nameof(MemoryOptimizerViewModel_StateUnavailable));

    // Auto-extracted literals from 

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string SystemOptimizer_InputFullscreenDesc => Get(nameof(SystemOptimizer_InputFullscreenDesc));
    public static string SystemOptimizer_InputFullscreenName => Get(nameof(SystemOptimizer_InputFullscreenName));
    public static string SystemOptimizer_InputGameBarDesc => Get(nameof(SystemOptimizer_InputGameBarDesc));
    public static string SystemOptimizer_InputGameBarName => Get(nameof(SystemOptimizer_InputGameBarName));
    public static string SystemOptimizer_InputGameDvrDesc => Get(nameof(SystemOptimizer_InputGameDvrDesc));
    public static string SystemOptimizer_InputGameDvrName => Get(nameof(SystemOptimizer_InputGameDvrName));
    public static string SystemOptimizer_InputMouseAccelDesc => Get(nameof(SystemOptimizer_InputMouseAccelDesc));
    public static string SystemOptimizer_InputMouseAccelName => Get(nameof(SystemOptimizer_InputMouseAccelName));
    public static string SystemOptimizer_LastVerified => Get(nameof(SystemOptimizer_LastVerified));
    public static string SystemOptimizer_NetworkDeliveryDesc => Get(nameof(SystemOptimizer_NetworkDeliveryDesc));
    public static string SystemOptimizer_NetworkDeliveryName => Get(nameof(SystemOptimizer_NetworkDeliveryName));
    public static string SystemOptimizer_NetworkNagleDesc => Get(nameof(SystemOptimizer_NetworkNagleDesc));
    public static string SystemOptimizer_NetworkNagleName => Get(nameof(SystemOptimizer_NetworkNagleName));
    public static string SystemOptimizer_NetworkTcpAckDesc => Get(nameof(SystemOptimizer_NetworkTcpAckDesc));
    public static string SystemOptimizer_NetworkTcpAckName => Get(nameof(SystemOptimizer_NetworkTcpAckName));
    public static string SystemOptimizer_NetworkTcpNoDelayDesc => Get(nameof(SystemOptimizer_NetworkTcpNoDelayDesc));
    public static string SystemOptimizer_NetworkTcpNoDelayName => Get(nameof(SystemOptimizer_NetworkTcpNoDelayName));
    public static string SystemOptimizer_NoDrift => Get(nameof(SystemOptimizer_NoDrift));
    public static string SystemOptimizer_PowerForegroundDesc => Get(nameof(SystemOptimizer_PowerForegroundDesc));
    public static string SystemOptimizer_PowerForegroundName => Get(nameof(SystemOptimizer_PowerForegroundName));
    public static string SystemOptimizer_PowerGameModeDesc => Get(nameof(SystemOptimizer_PowerGameModeDesc));
    public static string SystemOptimizer_PowerGameModeName => Get(nameof(SystemOptimizer_PowerGameModeName));
    public static string SystemOptimizer_PowerGpuSchedDesc => Get(nameof(SystemOptimizer_PowerGpuSchedDesc));
    public static string SystemOptimizer_PowerGpuSchedName => Get(nameof(SystemOptimizer_PowerGpuSchedName));
    public static string SystemOptimizer_PowerUltimatePerfDesc => Get(nameof(SystemOptimizer_PowerUltimatePerfDesc));
    public static string SystemOptimizer_PowerUltimatePerfName => Get(nameof(SystemOptimizer_PowerUltimatePerfName));
    public static string SystemOptimizer_PreflightHighRisk => Get(nameof(SystemOptimizer_PreflightHighRisk));
    public static string SystemOptimizer_PreflightLine => Get(nameof(SystemOptimizer_PreflightLine));
    public static string SystemOptimizer_ServiceDiagTrackDesc => Get(nameof(SystemOptimizer_ServiceDiagTrackDesc));
    public static string SystemOptimizer_ServiceDiagTrackName => Get(nameof(SystemOptimizer_ServiceDiagTrackName));
    public static string SystemOptimizer_ServiceSearchDesc => Get(nameof(SystemOptimizer_ServiceSearchDesc));
    public static string SystemOptimizer_ServiceSearchName => Get(nameof(SystemOptimizer_ServiceSearchName));
    public static string SystemOptimizer_ServiceSearchWarn => Get(nameof(SystemOptimizer_ServiceSearchWarn));
    public static string SystemOptimizer_ServiceSysMainDesc => Get(nameof(SystemOptimizer_ServiceSysMainDesc));
    public static string SystemOptimizer_ServiceSysMainName => Get(nameof(SystemOptimizer_ServiceSysMainName));
    public static string SystemOptimizer_ServiceTelemetryDesc => Get(nameof(SystemOptimizer_ServiceTelemetryDesc));
    public static string SystemOptimizer_ServiceTelemetryName => Get(nameof(SystemOptimizer_ServiceTelemetryName));
    public static string SystemOptimizer_Storage8dot3Desc => Get(nameof(SystemOptimizer_Storage8dot3Desc));
    public static string SystemOptimizer_Storage8dot3Name => Get(nameof(SystemOptimizer_Storage8dot3Name));
    public static string SystemOptimizer_Storage8dot3Warn => Get(nameof(SystemOptimizer_Storage8dot3Warn));
    public static string SystemOptimizer_StorageLastAccessDesc => Get(nameof(SystemOptimizer_StorageLastAccessDesc));
    public static string SystemOptimizer_StorageLastAccessName => Get(nameof(SystemOptimizer_StorageLastAccessName));
    public static string SystemOptimizer_StorageTrimDesc => Get(nameof(SystemOptimizer_StorageTrimDesc));
    public static string SystemOptimizer_StorageTrimName => Get(nameof(SystemOptimizer_StorageTrimName));
    public static string SystemOptimizer_VisualAnimationsDesc => Get(nameof(SystemOptimizer_VisualAnimationsDesc));
    public static string SystemOptimizer_VisualAnimationsName => Get(nameof(SystemOptimizer_VisualAnimationsName));
    public static string SystemOptimizer_VisualTransparencyDesc => Get(nameof(SystemOptimizer_VisualTransparencyDesc));
    public static string SystemOptimizer_VisualTransparencyName => Get(nameof(SystemOptimizer_VisualTransparencyName));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    // Auto-extracted literals from 
    public static string SettingsView_BatteryTip100 => Get(nameof(SettingsView_BatteryTip100));
    public static string SettingsView_BatteryTip6070 => Get(nameof(SettingsView_BatteryTip6070));
    public static string SettingsView_BatteryTip80 => Get(nameof(SettingsView_BatteryTip80));
    public static string SettingsView_BatteryTipsTitle => Get(nameof(SettingsView_BatteryTipsTitle));
    public static string SystemOptimizer_VerificationFailed => Get(nameof(SystemOptimizer_VerificationFailed));
    public static string SystemOptimizer_VerifiedCurrentState => Get(nameof(SystemOptimizer_VerifiedCurrentState));

    // Auto-extracted literals from AdvancedView.xaml

    // Auto-extracted literals from DashboardView.xaml

    // Auto-extracted literals from TuningView.xaml

    }
}
