using System;
using System.Windows;

namespace InteractiveChannel
{
    public static class LanguageSelector
    {
        public static void SetAutoLanguage()
        {
            if (System.Globalization.CultureInfo.CurrentCulture.Name == "ja-JP")
            {
                ChangeLanguage("Japanese");
            }
        }

        public static void ChangeLanguage(string language)
        {
            var dictionary = new ResourceDictionary();
            dictionary.Source = new Uri($"/InteractiveChannel;component/Resources/{language}.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries[0] = dictionary;
            Globals.CurrentLanguage = language;
            UnityMemoryMappedFile.KeyConfig.Language = language;
        }

        public static string Get(string key)
        {
            return Application.Current.Resources.MergedDictionaries[0][key] as string;
        }

        public static string GetByTypeName(string typename)
        {
            return Get("NoAssign");
        }
    }
}