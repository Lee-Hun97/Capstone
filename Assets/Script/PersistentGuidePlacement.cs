using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager), typeof(ARAnchorManager))]
public class PersistentGuidePlacement : MonoBehaviour
{
	[Header("Smoothing")]
	[Range(1f, 30f)] public float smoothingSpeed = 10f;

	const string BundleFileName = "testbundle";
	const string AssetName = "Basic Ceramic";

	AssetBundle bundle;
	GameObject prefab;

	private ARTrackedImageManager trackedImageManager;
	private ARAnchorManager anchorManager;

	private class AnchorEntry
	{
		public ARAnchor anchor;
		public GameObject guideInstance;
		public bool isLocked;
	}
	private readonly Dictionary<string, AnchorEntry> anchors = new Dictionary<string, AnchorEntry>();
	private bool freezeUpdates = false;

	private void Awake()
	{
		trackedImageManager = GetComponent<ARTrackedImageManager>();
		anchorManager = GetComponent<ARAnchorManager>();

		string path = Path.Combine(Application.persistentDataPath, BundleFileName);
		bundle = File.Exists(path) ? AssetBundle.LoadFromFile(path) : null;
		if (bundle != null)
		{
			prefab = bundle.LoadAsset<GameObject>(AssetName);
		}
		else
			Debug.LogError($"Failed to load bundle at {path}");
	}

	private void OnEnable()
	{
		trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
	}

	private void OnDisable()
	{
		trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
	}

	private void Update()
	{
		foreach (var entry in anchors.Values)
		{
			var target = entry.anchor.transform;
			var guide = entry.guideInstance.transform;
			guide.position = Vector3.Lerp(guide.position, target.position, Time.deltaTime * smoothingSpeed);
			guide.rotation = Quaternion.Slerp(guide.rotation, target.rotation, Time.deltaTime * smoothingSpeed);
		}
	}

	private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs evt)
	{
		// Debug: log tracking events
		Debug.Log($"OnTrackedImagesChanged called: added={evt.added.Count}, updated={evt.updated.Count}");

		// Create guide on first detection (added or first updated)
		if (!anchors.Any())
		{
			foreach (var img in evt.added.Concat(evt.updated))
			{
				if (img.trackingState == TrackingState.Tracking)
				{
					CreateGuide(img);
					Debug.Log("wow");
					break;
				}
			}
		}

		// Update anchor positions if not locked
		if (!freezeUpdates)
		{
			foreach (var img in evt.updated)
			{
				if (anchors.TryGetValue(img.referenceImage.name, out var entry) && !entry.isLocked && img.trackingState == TrackingState.Tracking)
				{
					entry.anchor.transform.SetPositionAndRotation(img.transform.position, img.transform.rotation);
				}
			}
		}
	}

	private void CreateGuide(ARTrackedImage img)
	{
		var name = img.referenceImage.name;
		if (anchors.ContainsKey(name))
			return;

		var anchorGO = new GameObject($"{name}_Anchor");
		anchorGO.transform.SetPositionAndRotation(img.transform.position, img.transform.rotation);
		var anchor = anchorGO.AddComponent<ARAnchor>();
		if (anchor == null)
		{
			Destroy(anchorGO);
			return;
		}

		var guide = Instantiate(prefab, anchorGO.transform, false);
		guide.transform.localPosition = Vector3.zero;
		guide.transform.localRotation = Quaternion.identity;

		if (guide.GetComponent<CombinedCupGuide>() == null)
		{
			guide.AddComponent<CombinedCupGuide>();
			Debug.Log("Success");
		}

		anchors[name] = new AnchorEntry { anchor = anchor, guideInstance = guide, isLocked = false };
	}

	/// <summary>
	/// Call when alignment is confirmed to lock anchors
	/// </summary>
	public void ConfirmAlignment()
	{
		freezeUpdates = true;
		foreach (var entry in anchors.Values)
			entry.isLocked = true;
	}

	/// <summary>
	/// Reset all anchors and unlock updates
	/// </summary>
	public void ResetAlignment()
	{
		foreach (var entry in anchors.Values)
			Destroy(entry.anchor.gameObject);
		anchors.Clear();
		freezeUpdates = false;
	}
}
