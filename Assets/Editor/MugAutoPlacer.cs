using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class sdf : AssetPostprocessor
{
	// FBX ���� ����Ʈ �� �� ���� ó���Ǵ� ��
	static void OnPostprocessAllAssets(
		string[] importedAssets,
		string[] deletedAssets,
		string[] movedAssets,
		string[] movedFromAssetPaths)
	{
		foreach (var assetPath in importedAssets)
		{
			// /Assets/Mugs/ ���� �Ʒ� .fbx�� ���
			if (!assetPath.StartsWith("Assets/Mugs/") || !assetPath.EndsWith(".fbx"))
				continue;

			// �����Ͱ� ���� �ִ� ���� �̹� ���� �̸��� �ִٸ� ��ŵ
			string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
			if (GameObject.Find(assetName) != null)
				continue;

			// FBX ����(������ ����)�� �ҷ��ͼ� ���� �ν��Ͻ�ȭ
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
			if (prefab == null)
				continue;

			var instance = PrefabUtility.InstantiatePrefab(prefab,
									EditorSceneManager.GetActiveScene())
								as GameObject;
			if (instance != null)
			{
				// ���ϴ� �⺻ ��ġ�� ȸ���� ����
				instance.name = assetName;
				instance.transform.position = Vector3.zero;
				instance.transform.rotation = Quaternion.identity;

				// ���� ������ �������� ǥ��
				EditorSceneManager.MarkSceneDirty(
					EditorSceneManager.GetActiveScene());

				Debug.Log($"[MugAutoPlacer] ���� �ڵ� ��ġ: {assetName}");
			}
		}
	}
}