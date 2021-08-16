using Google.Apis.YouTube.v3.Data;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UnityMemoryMappedFile;

namespace InteractiveChannel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int CurrentWindowNum = 0;
        private PipeCommands.LogNotify lastLog = null;
        public Action onStartStream;
        public Action onLoadSetting;

        public MainWindow()
        {
            InitializeComponent();
            if (App.CommandLineArgs == null || App.CommandLineArgs.Length < 2 || App.CommandLineArgs.First().StartsWith("/pipeName") == false)
            {
                this.Close();
                return;
            }
            Globals.Connect(App.CommandLineArgs[1]);
            Globals.Client.ReceivedEvent += Client_Received;
            UpdateWindowTitle();
            Globals.LoadCommonSettings();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            Globals.YouTubeApi.onStateChange = UpdateYoutubeApiProgress;
            Globals.YouTubeApi.onGetLiveStream = UpdateLiveCaption;
            Globals.YouTubeApi.onSetLiveStream = UpdateLiveCaption;
        }
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                await Globals.Client?.SendCommandAsync(new PipeCommands.ExitControlPanel { });
            }
            catch { }
            Application.Current.Windows.Cast<Window>().ToList().ForEach(d => { if (d != this) d.Close(); });
            Globals.Client.Dispose();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (LiveStreamButton.IsChecked.Value)
            {
                onStartStream?.Invoke();
                Globals.YouTubeApi.StartLive(VideoIdText.Text);
            }
            else
            {
                Globals.YouTubeApi.EndLive();
            }
        }

        private async void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("MoveButton_Click");
            Random r = new Random();
            await Globals.Client.SendCommandAsync(new PipeCommands.MoveObject { force = (float)Math.Pow((r.NextDouble() * 10), r.Next(1, 4)) });
        }


        private async void GetCurrentDataButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("GetCurrentDataButton_Click");
            await Globals.Client.SendCommandWaitAsync(new PipeCommands.GetCurrentPosition(), d =>
            {
                var ret = (PipeCommands.ReturnCurrentPosition)d;
                Dispatcher.Invoke(() => ReceiveTextBlock.Text = $"{ret.Data}");
            });
        }


        private void Client_Received(object sender, DataReceivedEventArgs e)
        {
            if (e.CommandType == typeof(PipeCommands.SendMessage))
            {
                var d = (PipeCommands.SendMessage)e.Data;
                MessageBox.Show($"[Client]ReceiveFromServer:{d.Message}");
            }
        }

        private async void LoadSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.LoadCommonSettings();

            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Setting File(*.json)|*.json";
            ofd.InitialDirectory = Globals.ExistDirectoryOrNull(Globals.CurrentCommonSettingsWPF.CurrentPathOnSettingFileDialog);
            if (ofd.ShowDialog() == true)
            {
                await Globals.Client.SendCommandAsync(new PipeCommands.LoadSettings { Path = ofd.FileName });

                if (Globals.CurrentCommonSettingsWPF.CurrentPathOnSettingFileDialog != System.IO.Path.GetDirectoryName(ofd.FileName))
                {
                    Globals.CurrentCommonSettingsWPF.CurrentPathOnSettingFileDialog = System.IO.Path.GetDirectoryName(ofd.FileName);
                    Globals.SaveCommonSettings();
                    onLoadSetting?.Invoke();
                }
            }
        }

        private async void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.LoadCommonSettings();

            var sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "Setting File(*.json)|*.json";
            sfd.InitialDirectory = Globals.ExistDirectoryOrNull(Globals.CurrentCommonSettingsWPF.CurrentPathOnSettingFileDialog);
            if (sfd.ShowDialog() == true)
            {
                await Globals.Client.SendCommandAsync(new PipeCommands.SaveSettings { Path = sfd.FileName });

                if (Globals.CurrentCommonSettingsWPF.CurrentPathOnSettingFileDialog != System.IO.Path.GetDirectoryName(sfd.FileName))
                {
                    Globals.CurrentCommonSettingsWPF.CurrentPathOnSettingFileDialog = System.IO.Path.GetDirectoryName(sfd.FileName);
                    Globals.SaveCommonSettings();
                }
            }
        }
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new SettingWindow();
            win.Owner = this;
            win.ShowDialog();
            UpdateWindowTitle();
        }

        private void ChatWindowOpen_Click(object sender, RoutedEventArgs e)
        {
            var win = new LiveChatWindow();
            win.Owner = this;
            win.Show();
            UpdateWindowTitle();
        }

        private void UpdateYoutubeApiProgress(YouTubeApi.LiveBroadcastType state)
        {
            VideoIdText.IsEnabled = true;
            if (state == YouTubeApi.LiveBroadcastType.live)
            {
                VideoIdText.IsEnabled = false;
                LiveStreamStatus.Text = $"{LanguageSelector.Get("Completed")}";
            }
            else
            {
                LiveStreamStatus.Text = $"{LanguageSelector.Get("StandBy")}";
            }
        }

        private void UpdateLiveCaption(Video video)
        {
            LiveStreamInfo.Text = video.Snippet.Title + Environment.NewLine + video.Snippet.Description;
        }

        private void UpdateWindowTitle()
        {
            Title = $"{LanguageSelector.Get("MainWindowTitle")} ({(CurrentWindowNum == 0 ? LanguageSelector.Get("MainWindowTitleLoading") : CurrentWindowNum.ToString())})";
        }

        private void StatusBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lastLog != null)
            {
                string trace = "[" + lastLog.type.ToString() + "] " + lastLog.condition + "\n" + lastLog.stackTrace;
                Clipboard.SetText(trace);
                MessageBox.Show("Trace log has copied.", "Trace log", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            //System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void TitleModifyButton_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.YouTubeApi.CurrentState == YouTubeApi.LiveBroadcastType.live)
            {
                var newVideo = new Video
                {
                    Snippet = new VideoSnippet
                    {
                        Title = LiveTitleText.Text,
                        Description = LiveDescriptionText.Text
                    }
                };
                Globals.YouTubeApi.SetLiveStream(newVideo);
            }
        }
    }
}
