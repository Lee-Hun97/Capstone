using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(LineRenderer))]
public class CombinedCupGuide : MonoBehaviour
{
	[Header("���ø� ����")]
	[Tooltip("���� �����̽� ����. �������� ��Ȯ������ ��� ����")]
	public int sampleCount = 100;

	// UI ������Ʈ�� �±׷� �ڵ� �˻�
	private InputField volumeInputField;
	private Button generateButton;

	// ���� ��������
	private float[] heights;
	private float[] cumVolumes;

	// ���̵���� ������Ʈ
	private GameObject circleGuidelineObj;

	// ���̵���� ���� �� �Ӽ�
	private Material guidelineMat;
	private const float guidelineWidth = 0.005f;
	private static readonly Color guidelineColor = new Color(1f, 0f, 0f, 0.5f);

	private MeshFilter meshFilter;

	void Awake()
	{
		// 1) ��Ʈ MeshFilter �켱 �õ�
		meshFilter = GetComponent<MeshFilter>();
		// 2) ���ų� sharedMesh�� null�̸� �ڽ��� MeshFilter�� ã�ƺ���
		if (meshFilter == null || meshFilter.sharedMesh == null)
			meshFilter = GetComponentInChildren<MeshFilter>();

		// 3) ������ ������ ��ũ��Ʈ ��Ȱ��ȭ
		if (meshFilter == null || meshFilter.sharedMesh == null)
		{
			Debug.LogError("CombinedCupGuide: �޽ø� ã�� �� �����ϴ�!");
			enabled = false;
			return;
		}

		var defaultLR = GetComponent<LineRenderer>();
		if (defaultLR != null)
			defaultLR.enabled = false;

		// ���̵���� ��Ƽ���� ������
		guidelineMat = new Material(Shader.Find("Unlit/Color"));
		guidelineMat.color = guidelineColor;

		BuildVolumeProfile();
	}

	void Start()
	{
		// �±׷� UI �ڵ� �˻�
		var goIF = GameObject.FindWithTag("VolumeInputField");
		if (goIF != null) volumeInputField = goIF.GetComponent<InputField>();
		var goBtn = GameObject.FindWithTag("GenerateButton");
		if (goBtn != null) generateButton = goBtn.GetComponent<Button>();

		if (volumeInputField == null || generateButton == null)
		{
			Debug.LogError("CombinedCupGuide: UI ������Ʈ�� ã�� ���߽��ϴ�. �±׸� Ȯ���ϼ���.");
			enabled = false;
			return;
		}

		generateButton.onClick.AddListener(OnGenerateClicked);
	}

	void OnGenerateClicked()
	{
		if (float.TryParse(volumeInputField.text, out float ml))
			GenerateGuide(ml);
		else
			Debug.LogError($"CombinedCupGuide: �߸��� ���� ���� - '{volumeInputField.text}'");
	}

	void BuildVolumeProfile()
	{
		var mesh = meshFilter.sharedMesh;
		var verts = mesh.vertices;

		float minY = verts.Min(v => v.y);
		float maxY = verts.Max(v => v.y);

		heights = new float[sampleCount];
		cumVolumes = new float[sampleCount];

		float dh = (maxY - minY) / (sampleCount - 1);
		float prevArea = 0f;
		cumVolumes[0] = 0f;

		for (int i = 0; i < sampleCount; i++)
		{
			float h = minY + dh * i;
			heights[i] = h;

			var hull2D = ComputeCrossSectionHull(mesh, h);
			float area = hull2D.Count >= 3 ? Mathf.Abs(PolygonArea(hull2D)) : 0f;

			if (i > 0)
				cumVolumes[i] = cumVolumes[i - 1] + 0.5f * (prevArea + area) * dh;
			prevArea = area;
		}
	}

	public void GenerateGuide(float targetVolume_ml)
	{
		Debug.Log($"CombinedCupGuide.GenerateGuide ȣ��: {targetVolume_ml} mL");

		if (circleGuidelineObj != null)
			Destroy(circleGuidelineObj);

		float target_m3 = Mathf.Max(0f, targetVolume_ml * 1e-6f);
		float maxV = cumVolumes.Last();
		if (target_m3 > maxV) target_m3 = maxV;

		int idx = Array.FindIndex(cumVolumes, v => v >= target_m3);
		idx = Mathf.Clamp(idx, 1, sampleCount - 1);

		float v0 = cumVolumes[idx - 1];
		float v1 = cumVolumes[idx];
		float t = (v1 > v0) ? (target_m3 - v0) / (v1 - v0) : 0f;
		float hLocal = Mathf.Lerp(heights[idx - 1], heights[idx], t);

		DrawSectionHull(hLocal);
	}

	void DrawSectionHull(float hLocal)
	{
		var mesh = meshFilter.sharedMesh;
		var hull2D = ComputeCrossSectionHull(mesh, hLocal);
		if (hull2D.Count < 3)
		{
			Debug.LogWarning($"CombinedCupGuide: �ܸ� ����Ʈ ����(h={hLocal}): {hull2D.Count}");
			return;
		}

		circleGuidelineObj = new GameObject("CircleGuideline");
		circleGuidelineObj.transform.SetParent(transform, false);
		var lr = circleGuidelineObj.AddComponent<LineRenderer>();
		lr.useWorldSpace = false;
		lr.loop = true;
		lr.positionCount = hull2D.Count;
		lr.startWidth = guidelineWidth;
		lr.endWidth = guidelineWidth;
		lr.material = guidelineMat;

		for (int i = 0; i < hull2D.Count; i++)
		{
			var p = hull2D[i];
			lr.SetPosition(i, new Vector3(p.x, hLocal, p.y));
		}
	}

	List<Vector2> ComputeCrossSectionHull(Mesh mesh, float hLocal)
	{
		var verts = mesh.vertices;
		var tris = mesh.triangles;
		var cutPts = new List<Vector3>();
		for (int i = 0; i < tris.Length; i += 3)
		{
			TryEdge(verts[tris[i]], verts[tris[i + 1]], hLocal, cutPts);
			TryEdge(verts[tris[i + 1]], verts[tris[i + 2]], hLocal, cutPts);
			TryEdge(verts[tris[i + 2]], verts[tris[i]], hLocal, cutPts);
		}
		var pts2D = cutPts
			.Select(p => new Vector2(
				Mathf.Round(p.x * 1e4f) / 1e4f,
				Mathf.Round(p.z * 1e4f) / 1e4f))
			.Distinct()
			.ToList();
		return pts2D.Count >= 3 ? ConvexHull(pts2D) : new List<Vector2>();
	}

	void TryEdge(Vector3 a, Vector3 b, float h, List<Vector3> outPts)
	{
		bool ba = a.y <= h;
		bool bb = b.y <= h;
		if (ba ^ bb)
			outPts.Add(Vector3.Lerp(a, b, (h - a.y) / (b.y - a.y)));
	}

	List<Vector2> ConvexHull(List<Vector2> pts)
	{
		pts.Sort((a, b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));
		var lower = new List<Vector2>();
		foreach (var p in pts)
		{
			while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
				lower.RemoveAt(lower.Count - 1);
			lower.Add(p);
		}
		var upper = new List<Vector2>();
		for (int i = pts.Count - 1; i >= 0; i--)
		{
			var p = pts[i];
			while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
				upper.RemoveAt(upper.Count - 1);
			upper.Add(p);
		}
		lower.RemoveAt(lower.Count - 1);
		upper.RemoveAt(upper.Count - 1);
		lower.AddRange(upper);
		return lower;
	}

	float Cross(Vector2 o, Vector2 a, Vector2 b)
		=> (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);

	float PolygonArea(List<Vector2> poly)
	{
		float area = 0f;
		for (int i = 0; i < poly.Count; i++)
		{
			var p0 = poly[i];
			var p1 = poly[(i + 1) % poly.Count];
			area += p0.x * p1.y - p1.x * p0.y;
		}
		return area * 0.5f;
	}
}
