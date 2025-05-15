using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RuntimeImportUI : MonoBehaviour
{
	private Button importButton;
	private const string ImportButtonTag = "ImportButton";
	private const string fbxAssetPath = "Assets/Mugs/Basic Ceramic.fbx";

	void Start()
	{
		// 태그로 Import 버튼 찾기
		var go = GameObject.FindWithTag(ImportButtonTag);
		if (go == null)
		{
			Debug.LogError($"RuntimeImportUI: '{ImportButtonTag}' 태그가 붙은 버튼을 찾을 수 없습니다.");
			return;
		}

		importButton = go.GetComponent<Button>();
		if (importButton == null)
		{
			Debug.LogError("RuntimeImportUI: ImportButton 오브젝트에 Button 컴포넌트가 없습니다.");
			return;
		}

		importButton.onClick.AddListener(OnImportClicked);
	}

	void OnImportClicked()
	{
		StartCoroutine(ImportModelCoroutine());
	}

	private IEnumerator ImportModelCoroutine()
	{
		Debug.Log("Import 시작");

#if UNITY_EDITOR
		// 에디터 전용: AssetDatabase로 FBX 에셋 로드
		var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(fbxAssetPath);
		if (prefab != null)
		{
			var go = Instantiate(prefab);
			go.name = prefab.name;
			Debug.Log($"모델 임포트 완료: {go.name}");
		}
		else
		{
			Debug.LogError($"모델 로드 실패: 경로를 확인하세요 ({fbxAssetPath})");
		}
#else
        Debug.LogError("런타임 빌드에서는 AssetDatabase를 사용할 수 없습니다. Addressables/AssetBundle 방식을 사용하세요.");
#endif

		yield return null;
	}
}