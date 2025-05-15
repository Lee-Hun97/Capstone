using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TouchBundleSpawner : MonoBehaviour
{
	const string BundleFileName = "testbundle";
	const string AssetName = "Basic Ceramic";

	ARRaycastManager raycastMgr;
	AssetBundle bundle;
	GameObject prefab;
	List<ARRaycastHit> hits = new List<ARRaycastHit>();

	void Awake()
	{
		raycastMgr = GetComponent<ARRaycastManager>();
		string path = Path.Combine(Application.persistentDataPath, BundleFileName);
		bundle = File.Exists(path) ? AssetBundle.LoadFromFile(path) : null;
		if (bundle != null)
			prefab = bundle.LoadAsset<GameObject>(AssetName);
		else
			Debug.LogError($"Failed to load bundle at {path}");
	}

	void Update()
	{
		if (Input.touchCount == 0) return;
		var touch = Input.GetTouch(0);
		if (touch.phase != TouchPhase.Began) return;

		if (raycastMgr.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon) && prefab != null)
		{
			var pose = hits[0].pose;
			// Instantiate bundle prefab
			var go = Instantiate(prefab, pose.position, pose.rotation);
			go.name = AssetName;
			// Attach CombinedCupGuide component
			if (go.GetComponent<CombinedCupGuide>() == null)
				go.AddComponent<CombinedCupGuide>();
		}
	}

	void OnDestroy()
	{
		if (bundle != null) bundle.Unload(false);
	}
}
