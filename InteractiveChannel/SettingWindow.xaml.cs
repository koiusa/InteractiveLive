using Microsoft.Win32;
using sh_akira;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnityMemoryMappedFile;

namespace InteractiveChannel
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var MainWin = (MainWindow)Owner;
            MainWin.onLoadSetting = LoadYoutubeApiSetting;
            UpdateWindowTitle();
            LoadYoutubeApiSetting();
        }
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await Globals.Client?.SendCommandAsync(new PipeCommands.StatusStringChangedRequest { doSend = false });
        }

        private void FileReference_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = $"{LanguageSelector.Get("SettingWindow_AuthFileFilter")}" + " (*.json)|*.json";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                var button = (Button)sender;
                ((Grid)button.Parent).Children.OfType<TextBlock>().First().Text = dialog.FileName;
            }
        }

        private void YouTubeApiSettingApply_Click(object sender, RoutedEventArgs e)
        {
            Globals.YouTubeApi.CurrentSetting = new YouTubeApi.Setting
            {
                AuthType = (YouTubeApi.AuthType)AuthTypeTab.SelectedIndex,
                ApiKey = ApiKey.Text,
                ClientSecret = OAuthKey.Text,
                ApplicationName = AplicationNameText.Text
            };

            string path = System.IO.Path.GetFullPath(Globals.GetCurrentAppDir() + "/../Settings/YouTubeApiSetting.json");
            var directoryName = System.IO.Path.GetDirectoryName(path);
            if (Directory.Exists(directoryName) == false) Directory.CreateDirectory(directoryName);
            File.WriteAllText(path, Json.Serializer.ToReadable(Json.Serializer.Serialize(Globals.YouTubeApi.CurrentSetting)));
            //YoutubeApi設定のファイルパスを共通設定に保存
            Globals.CurrentCommonSettingsWPF.YoutubeApiSettingFilePath = path;
            Globals.SaveCommonSettings();

            this.Close();
        }

        private void LoadYoutubeApiSetting()
        {
            var path = Globals.CurrentCommonSettingsWPF.YoutubeApiSettingFilePath;
            if (!string.IsNullOrEmpty(path))
            {
                var youtubeSetting = Json.Serializer.Deserialize<YouTubeApi.Setting>(File.ReadAllText(path)); //設定を読み込み
                AuthTypeTab.SelectedIndex = (int)youtubeSetting.AuthType;
                ApiKey.Text = youtubeSetting.ApiKey;
                OAuthKey.Text = youtubeSetting.ClientSecret;
                AplicationNameText.Text = youtubeSetting.ApplicationName;
            }
        }

        private void UpdateWindowTitle()
        {
            Title = $"{LanguageSelector.Get("SettingWindowTitle")}";
        }
    }
}
