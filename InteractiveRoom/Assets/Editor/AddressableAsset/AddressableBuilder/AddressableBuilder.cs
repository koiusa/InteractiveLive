using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class AddressableBuilder
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        // �r���h�{�^���̉������t�b�N
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }

    private static void BuildPlayerHandler(BuildPlayerOptions options)
    {
        // �K�v�ɉ�����Clean
        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);

        // Addressable���r���h
        AddressableAssetSettings.BuildPlayerContent();

        // Player���r���h
        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
    }
}