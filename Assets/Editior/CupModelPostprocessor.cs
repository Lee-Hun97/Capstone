// Assets/Editor/CupModelPostprocessor.cs
using UnityEngine;
using UnityEditor;

/// <summary>
/// �� ����Ʈ �� Read/Write �� CombinedCupGuide �ڵ� �߰� ó��
/// Assets/Mugs/ ������ ��ġ�� FBX���� ����˴ϴ�.
/// </summary>
public class CupModelPostprocessor : AssetPostprocessor
{
	// �� ����Ʈ ����(Read/Write Ȱ��ȭ)
	void OnPreprocessModel()
	{
		if (assetPath.StartsWith("Assets/Mugs/") && assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
		{
			var importer = (ModelImporter)assetImporter;
			importer.isReadable = true;
			Debug.Log($"[Preprocess] Read/Write Enabled Ȱ��ȭ: {assetPath}");
		}
	}

	// �� ����Ʈ ����(CombinedCupGuide �ڵ� �߰�)
	void OnPostprocessModel(GameObject root)
	{
		if (!assetPath.StartsWith("Assets/Mugs/"))
			return;

		if (root.GetComponent<CombinedCupGuide>() == null)
		{
			root.AddComponent<CombinedCupGuide>();
			Debug.Log($"[Postprocess] CombinedCupGuide�� '{root.name}'�� �ڵ� �߰��Ǿ����ϴ�.");
		}
	}
}
