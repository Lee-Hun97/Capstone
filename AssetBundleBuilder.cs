using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBundleBuilder
{
    public static void BuildModelAssetBundle()
    {
        string modelPath = "Assets/ImportedModels/model.fbx";
        string assetBundleDirectory = "AssetBundles";

        if (!Directory.Exists(assetBundleDirectory))
            Directory.CreateDirectory(assetBundleDirectory);

        // 모델을 불러오고, 이름 지정해서 AssetBundle 생성
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (model == null)
        {
            Debug.LogError("모델 로딩 실패: " + modelPath);
            return;
        }

        // 임시 프리팹 생성
        string prefabPath = "Assets/TempModel.prefab";
        PrefabUtility.SaveAsPrefabAsset(model, prefabPath);

        AssetImporter importer = AssetImporter.GetAtPath(prefabPath);
        importer.assetBundleName = "modelbundle";

        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
            BuildAssetBundleOptions.None,
            BuildTarget.Android); // or StandaloneWindows / iOS
    }
}
