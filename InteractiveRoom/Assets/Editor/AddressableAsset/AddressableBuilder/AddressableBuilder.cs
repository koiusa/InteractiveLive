using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class AddressableBuilder
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        // ビルドボタンの押下をフック
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }

    private static void BuildPlayerHandler(BuildPlayerOptions options)
    {
        // 必要に応じてClean
        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);

        // Addressableをビルド
        AddressableAssetSettings.BuildPlayerContent();

        // Playerをビルド
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
    }
}