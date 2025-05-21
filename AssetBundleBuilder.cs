//using UnityEditor;
//using UnityEngine;
//using System.IO;

//public class AssetBundleBuilder
//{
//    public static void BuildModelAssetBundle()
//    {
//        string modelPath = "Assets/ImportedModels/model.fbx";
//        string assetBundleDirectory = "AssetBundles";

//        if (!Directory.Exists(assetBundleDirectory))
//            Directory.CreateDirectory(assetBundleDirectory);

//        // 모델을 불러오고, 이름 지정해서 AssetBundle 생성
//        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
//        if (model == null)
//        {
//            Debug.LogError("모델 로딩 실패: " + modelPath);
//            return;
//        }

//        // 임시 프리팹 생성
//        string prefabPath = "Assets/TempModel.prefab";
//        PrefabUtility.SaveAsPrefabAsset(model, prefabPath);

//        AssetImporter importer = AssetImporter.GetAtPath(prefabPath);
//        importer.assetBundleName = "modelbundle";

//        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
//            BuildAssetBundleOptions.None,
//            BuildTarget.Android); // or StandaloneWindows / iOS
//    }
//}
using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBundleBuilder
{
    public static void BuildModelAssetBundle()
    {
        // 유니티 실행 시 전달받은 인자 추출
        string[] args = System.Environment.GetCommandLineArgs();
        string userId = null;
        string timestamp = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--user_id" && i + 1 < args.Length)
                userId = args[i + 1];
            if (args[i] == "--timestamp" && i + 1 < args.Length)
                timestamp = args[i + 1];
        }

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(timestamp))
        {
            Debug.LogError("user_id 또는 timestamp 인자 누락");
            return;
        }

        // FBX 모델 경로
        string modelPath = "Assets/ImportedModels/model.fbx";
        string prefabPath = "Assets/TempModel.prefab";

        // 서버 기준의 출력 폴더 구조로 번들 저장
        //string bundleOutputPath = Path.Combine("outputs", $"user_{userId}", timestamp);
        string bundleOutputPath = Path.Combine(@"E:\team_project_server\outputs", $"user_{userId}", timestamp);
        if (!Directory.Exists(bundleOutputPath))
            Directory.CreateDirectory(bundleOutputPath);

        string bundleFileName = "model.bundle";

        // 모델 로딩
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (model == null)
        {
            Debug.LogError("모델 로딩 실패: " + modelPath);
            return;
        }

        // 프리팹 저장 및 번들 이름 지정
        PrefabUtility.SaveAsPrefabAsset(model, prefabPath);
        AssetImporter importer = AssetImporter.GetAtPath(prefabPath);
        importer.assetBundleName = bundleFileName;

        // 번들 생성
        BuildPipeline.BuildAssetBundles(bundleOutputPath,
            BuildAssetBundleOptions.None,
            BuildTarget.Android); // or StandaloneWindows, iOS

        Debug.Log("AssetBundle 생성 완료: " + Path.Combine(bundleOutputPath, bundleFileName));
    }
}
