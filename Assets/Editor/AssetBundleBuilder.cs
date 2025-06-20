using UnityEditor;
using System.IO;

public class AssetBundleBuilder
{
	[MenuItem("Build/Build All AssetBundles")]
	public static void BuildAllBundles()
	{
		var outputPath = "Assets/AssetBundles";
		if (!Directory.Exists(outputPath))
			Directory.CreateDirectory(outputPath);
		// 사용하지 않는 이름 정리
		AssetDatabase.RemoveUnusedAssetBundleNames();
		// 번들 빌드
		BuildPipeline.BuildAssetBundles(
			outputPath,
			BuildAssetBundleOptions.None,
			EditorUserBuildSettings.activeBuildTarget
		);
		AssetDatabase.Refresh();
		UnityEngine.Debug.Log("✅ AssetBundles 빌드 완료: " + outputPath);
	}
}