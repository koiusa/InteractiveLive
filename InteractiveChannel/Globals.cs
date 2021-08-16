using sh_akira;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using UnityMemoryMappedFile;

namespace InteractiveChannel
{
    public class Globals
    {
        public static MemoryMappedFileClient Client;
        public static YouTubeApi YouTubeApi;

        public static void Connect(string pipeName)
        {
            YouTubeApi = new YouTubeApi();
            Client = new MemoryMappedFileClient();
            Client.Start(pipeName);
        }

        public static string CurrentLanguage = "Japanese";

        public static string GetCurrentAppDir()
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (path.Last() == '\\') path = path.Substring(0, path.Length - 1);
            path += "\\";
            return path;
        }

        [Serializable]
        public class CommonSettingsWPF
        {
            public string CurrentPathOnSettingFileDialog = ""; //設定ファイルダイアログパス
            public string YoutubeApiSettingFilePath = ""; //YouTubeファイル
            //初期値
            [OnDeserializing()]
            internal void OnDeserializingMethod(StreamingContext context)
            {
                CurrentPathOnSettingFileDialog = "";
                YoutubeApiSettingFilePath = "";
            }
        }

        public static CommonSettingsWPF CurrentCommonSettingsWPF = new CommonSettingsWPF();

        //共通設定の書き込み
        public static void SaveCommonSettings()
        {
            try
            {
                string path = Path.GetFullPath(GetCurrentAppDir() + "/../Settings/commonWPF.json");
                var directoryName = Path.GetDirectoryName(path);
                if (Directory.Exists(directoryName) == false) Directory.CreateDirectory(directoryName);
                File.WriteAllText(path, Json.Serializer.ToReadable(Json.Serializer.Serialize(CurrentCommonSettingsWPF)));
            }
            catch (Exception) { }
        }

        //共通設定の読み込み
        public static void LoadCommonSettings()
        {
            string path = Path.GetFullPath(GetCurrentAppDir() + "/../Settings/commonWPF.json");
            if (!File.Exists(path))
            {
                return;
            }
            try
            {
                CurrentCommonSettingsWPF = Json.Serializer.Deserialize<CommonSettingsWPF>(File.ReadAllText(path)); //設定を読み込み
                LoadYoutubeApiSetting();
            }
            catch (Exception)
            {
                //エラー発生時は初期値にする
                CurrentCommonSettingsWPF = new CommonSettingsWPF();
                SaveCommonSettings();
            }
        }

        public static string ExistDirectoryOrNull(string path)
        {
            return Directory.Exists(path) ? path : null;
        }

        private static void LoadYoutubeApiSetting()
        {
            var path = CurrentCommonSettingsWPF.YoutubeApiSettingFilePath;
            if (!string.IsNullOrEmpty(path))
            {
                var youtubeSetting = Json.Serializer.Deserialize<YouTubeApi.Setting>(File.ReadAllText(path)); //設定を読み込み
                YouTubeApi.CurrentSetting = new YouTubeApi.Setting
                {
                    AuthType = youtubeSetting.AuthType,
                    ApiKey = youtubeSetting.ApiKey,
                    ClientSecret = youtubeSetting.ClientSecret,
                    ApplicationName = youtubeSetting.ApplicationName
                };
            }
        }
    }
}
