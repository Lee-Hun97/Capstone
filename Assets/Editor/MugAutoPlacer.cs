using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class sdf : AssetPostprocessor
{
	// FBX 파일 임포트 후 한 번에 처리되는 훅
	static void OnPostprocessAllAssets(
		string[] importedAssets,
		string[] deletedAssets,
		string[] movedAssets,
		string[] movedFromAssetPaths)
	{
		foreach (var assetPath in importedAssets)
		{
			// /Assets/Mugs/ 폴더 아래 .fbx만 대상
			if (!assetPath.StartsWith("Assets/Mugs/") || !assetPath.EndsWith(".fbx"))
				continue;

			// 에디터가 열려 있는 씬에 이미 같은 이름이 있다면 스킵
			string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
			if (GameObject.Find(assetName) != null)
				continue;

			// FBX 에셋(프리팹 형태)을 불러와서 씬에 인스턴스화
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
			if (prefab == null)
				continue;

			var instance = PrefabUtility.InstantiatePrefab(prefab,
									EditorSceneManager.GetActiveScene())
								as GameObject;
			if (instance != null)
			{
				// 원하는 기본 위치나 회전값 설정
				instance.name = assetName;
				instance.transform.position = Vector3.zero;
				instance.transform.rotation = Quaternion.identity;

				// 씬에 변경이 생겼음을 표시
				EditorSceneManager.MarkSceneDirty(
					EditorSceneManager.GetActiveScene());

				Debug.Log($"[MugAutoPlacer] 씬에 자동 배치: {assetName}");
			}
		}
	}
}