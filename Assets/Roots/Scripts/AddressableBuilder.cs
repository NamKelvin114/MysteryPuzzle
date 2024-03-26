#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public static class AddressableBuilder
{
    // [InitializeOnLoadMethod]
    // private static void Initialize() { BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler); }
    //
    // private static void BuildPlayerHandler(
    //     BuildPlayerOptions options)
    // {
    //     AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
    //
    //     AddressableAssetSettings.BuildPlayerContent();
    //
    //     BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
    // }
}
#endif