//  AddressableAssetAddressClassCreator.cs
//  http://kan-kikuchi.hatenablog.com/entry/AddressableAssetAddressClassCreator
//
//  Created by kan.kikuchi on 2019.09.16.

using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

/// <summary>
/// AddressableAsset��Address���Ǘ�����萔�N���X��������������N���X
/// </summary>
public class AddressableAssetAddressClassCreator : AssetPostprocessor
{

    //�ύX���`�F�b�N����f�B���N�g���̃p�X
    private static readonly string TARGET_DIRECTORY_PATH = "Assets/AddressableAssetsData/AssetGroups";

    //�萔�N���X�𐶐�����f�B���N�g���̃p�X
    private static readonly string EXPORT_DIRECTORY_PATH = "Assets/Scripts";

    //=================================================================================
    //�ύX�̊Ď�
    //=================================================================================

#if !UNITY_CLOUD_BUILD

    //�Ώۂ̃f�B���N�g���ȉ��̕ύX���`�F�b�N
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        List<string[]> assetsList = new List<string[]>(){
      importedAssets, deletedAssets, movedAssets, movedFromAssetPaths
    };

        List<string> targetDirectoryNameList = new List<string>(){
      TARGET_DIRECTORY_PATH,
    };

        //�ύX����������萔�N���X�쐬
        if (ExistsDirectoryInAssets(assetsList, targetDirectoryNameList))
        {
            Create();
        }
    }

    //���͂��ꂽassets�̃p�X�̒��ɁA�e�f�B���N�g���̖��O����targetDirectoryNameList�̂��̂���ł����邩
    private static bool ExistsDirectoryInAssets(List<string[]> assetPathsList, List<string> targetDirectoryNameList)
    {
        return assetPathsList
          .Any(assetPaths => assetPaths
           .Select(assetPath => Path.GetDirectoryName(assetPath))
           .Intersect(targetDirectoryNameList).Any());
    }

#endif

    //=================================================================================
    //�쐬
    //=================================================================================

    //�萔�N���X�쐬
    [MenuItem("Tools/Create/AddressableAsset Constants Class")]
    private static void Create()
    {

        //�A�h���X�ƃ��x�����܂Ƃ߂���
        var addressDict = new Dictionary<string, string>();
        var labelDict = new Dictionary<string, string>();

        //�Ώۂ̃f�B���N�g���ȉ��̃A�Z�b�g��S�Ď擾���A�A�h���X�ƃ��x�����L�^
        foreach (var group in LoadAll<AddressableAssetGroup>(TARGET_DIRECTORY_PATH))
        {
            foreach (var entry in group.entries)
            {
                if (addressDict.ContainsKey(entry.address))
                {
                    Debug.LogError($"{entry.address}�Ƃ����A�h���X���d�����Ă��܂��I");
                }
                addressDict[entry.address] = entry.address;

                foreach (var label in entry.labels)
                {
                    labelDict[label] = label;
                }
            }
        }

        //�L�^����Dictionary����萔�N���X�𐶐�
        ConstantsClassCreator.Create("AddressableAssetAddress", "AddressableAsset��Address���Ǘ�����萔�N���X", EXPORT_DIRECTORY_PATH, addressDict);
        ConstantsClassCreator.Create("AddressableAssetLabel", "AddressableAsset��Label���Ǘ�����萔�N���X", EXPORT_DIRECTORY_PATH, labelDict);
    }

    //�f�B���N�g���̃p�X(Assets����)�ƌ^��ݒ肵�AObject��ǂݍ��ށB���݂��Ȃ��ꍇ�͋��List��Ԃ�
    private static List<T> LoadAll<T>(string directoryPath) where T : Object
    {
        List<T> assetList = new List<T>();

        //�w�肵���f�B���N�g���ɓ����Ă���S�t�@�C�����擾(�q�f�B���N�g�����܂�)
        string[] filePathArray = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

        //�擾�����t�@�C���̒�����A�Z�b�g�������X�g�ɒǉ�����
        foreach (string filePath in filePathArray)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
            if (asset != null)
            {
                assetList.Add(asset);
            }
        }

        return assetList;
    }

}