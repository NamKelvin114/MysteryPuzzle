#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public static class Temp
{
    [MenuItem("Tools/RenameAddressable")]
    public static void ExecuteChangeAddressableName()
    {
        string[] guids2 = AssetDatabase.FindAssets("t:GameObject", new[] {"Assets\\Roots\\Prefabs\\Levels"});
        for (int j = 0; j < guids2.Length; j++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids2[j]);
            //AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(guids2[j] ,AddressableAssetSettingsDefaultObject.Settings.FindGroup("NewPackLevels"), false, true);
            var previous = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guids2[j]).address;
            var results = previous.Split(' ');
            AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guids2[j]).address = results[0];
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

#endif