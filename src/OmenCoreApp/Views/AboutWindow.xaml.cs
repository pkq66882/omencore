using OmenCore.Localization;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using OmenCore.Services;
using OmenCore.Utils;

namespace OmenCore.Views
{
    public partial class AboutWindow : Window
    {
        private readonly AutoUpdateService _updateService;
        private bool _isCheckingUpdates;

        public AboutWindow()
        {
            InitializeComponent();
            _updateService = new AutoUpdateService(App.Logging, "https://api.github.com/repos/theantipopau/omencore/releases/latest");
            VersionText.Text = string.Format(Strings.AboutWindow_VersionFormat, AppVersionProvider.GetVersionString());
        }

        private async void CheckUpdatesClicked(object sender, RoutedEventArgs e)
        {
            if (_isCheckingUpdates) return;
            
            _isCheckingUpdates = true;
            CheckUpdatesButton.IsEnabled = false;
            SetStatus(Strings.AboutWindow_CheckingForUpdates, "TextSecondaryBrush");
            
            try
            {
                var result = await _updateService.CheckForUpdatesAsync();
                
                if (result.UpdateAvailable && result.LatestVersion != null)
                {
                    var latest = result.LatestVersion;
                    SetStatus(string.Format(Strings.AboutWindow_UpdateAvailableFormat, latest.VersionString), "SuccessBrush");

                    if (string.IsNullOrWhiteSpace(latest.DownloadUrl))
                    {
                        var openRelease = MessageBox.Show(
                            "A new version is available on GitHub. Open the release page now?",
                            "Update Available",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (openRelease == MessageBoxResult.Yes)
                        {
                            OpenUrl(latest.ChangelogUrl);
                        }

                        return;
                    }

                    var msgResult = MessageBox.Show(
                        $"A new version is available!\n\nCurrent: {result.CurrentVersion.VersionString}\nLatest: {latest.VersionString}\n\nWould you like to download and install it now?",
                        "Update Available",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (msgResult == MessageBoxResult.Yes)
                    {
                        SetStatus(Strings.AboutWindow_Downloading, "TextSecondaryBrush");
                        var installerPath = await _updateService.DownloadUpdateAsync(latest);

                        if (installerPath != null)
                        {
                            SetStatus(Strings.AboutWindow_Installing, "TextSecondaryBrush");
                            var installResult = await _updateService.InstallUpdateAsync(installerPath);
                            if (!installResult.Success)
                            {
                                SetStatus(installResult.Message, "AccentBrush");
                            }
                        }
                        else
                        {
                                SetStatus(Strings.AboutWindow_DownloadFailedManual, "AccentBrush");
                                if (!string.IsNullOrWhiteSpace(latest.ChangelogUrl))
                                {
                                    var openGitHub = MessageBox.Show(
                                        "The update could not be downloaded or verified.\n\nOpen GitHub releases to download manually?",
                                        "Update Download Failed",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Warning);
                                    if (openGitHub == MessageBoxResult.Yes)
                                        OpenUrl(latest.ChangelogUrl);
                                }
                        }
                    }
                }
                else
                {
                    SetStatus(result.Message == "You are running the latest version." ? Strings.UpdateViewModel_RunningLatest : result.Message, "TextSecondaryBrush");
                }
            }
            catch (Exception ex)
            {
                SetStatus(string.Format(Strings.AboutWindow_CheckFailedFormat, ex.Message), "AccentBrush");
            }
            finally
            {
                _isCheckingUpdates = false;
                CheckUpdatesButton.IsEnabled = true;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void SetStatus(string message, string brushResourceKey)
        {
            UpdateStatusText.Text = message;
            if (FindResource(brushResourceKey) is Brush brush)
            {
                UpdateStatusText.Foreground = brush;
            }
        }

        private void OpenUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
