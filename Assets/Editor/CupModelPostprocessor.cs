// Assets/Editor/CupModelPostprocessor.cs
using UnityEngine;
using UnityEditor;

/// <summary>
/// 모델 임포트 시 Read/Write 및 CombinedCupGuide 자동 추가 처리
/// Assets/Mugs/ 폴더에 위치한 FBX에만 적용됩니다.
/// </summary>
public class CupModelPostprocessor : AssetPostprocessor
{
	// 모델 임포트 직전(Read/Write 활성화)
	void OnPreprocessModel()
	{
		if (assetPath.StartsWith("Assets/Mugs/") && assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
		{
			var importer = (ModelImporter)assetImporter;
			importer.isReadable = true;
			Debug.Log($"[Preprocess] Read/Write Enabled 활성화: {assetPath}");
		}
	}

	// 모델 임포트 직후(CombinedCupGuide 자동 추가)
	void OnPostprocessModel(GameObject root)
	{
		if (!assetPath.StartsWith("Assets/Mugs/"))
			return;

		if (root.GetComponent<CombinedCupGuide>() == null)
		{
			root.AddComponent<CombinedCupGuide>();
			Debug.Log($"[Postprocess] CombinedCupGuide가 '{root.name}'에 자동 추가되었습니다.");
		}
	}
}
