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
	[Header("샘플링 설정")]
	[Tooltip("수평 슬라이스 개수. 높을수록 정확하지만 비용 증가")]
	public int sampleCount = 100;

	// UI 컴포넌트는 태그로 자동 검색
	private InputField volumeInputField;
	private Button generateButton;

	// 내부 프로파일
	private float[] heights;
	private float[] cumVolumes;

	// 가이드라인 오브젝트
	private GameObject circleGuidelineObj;

	// 가이드라인 재질 및 속성
	private Material guidelineMat;
	private const float guidelineWidth = 0.005f;
	private static readonly Color guidelineColor = new Color(1f, 0f, 0f, 0.5f);

	private MeshFilter meshFilter;

	void Awake()
	{
		// 1) 루트 MeshFilter 우선 시도
		meshFilter = GetComponent<MeshFilter>();
		// 2) 없거나 sharedMesh가 null이면 자식의 MeshFilter를 찾아본다
		if (meshFilter == null || meshFilter.sharedMesh == null)
			meshFilter = GetComponentInChildren<MeshFilter>();

		// 3) 여전히 없으면 스크립트 비활성화
		if (meshFilter == null || meshFilter.sharedMesh == null)
		{
			Debug.LogError("CombinedCupGuide: 메시를 찾을 수 없습니다!");
			enabled = false;
			return;
		}

		var defaultLR = GetComponent<LineRenderer>();
		if (defaultLR != null)
			defaultLR.enabled = false;

		// 가이드라인 머티리얼 생성…
		var shader = Shader.Find("Sprites/Default");
		guidelineMat = new Material(shader);
		guidelineMat.color = guidelineColor;

		BuildVolumeProfile();
	}

	void Start()
	{
		// 태그로 UI 자동 검색
		var goIF = GameObject.FindWithTag("VolumeInputField");
		if (goIF != null) volumeInputField = goIF.GetComponent<InputField>();
		var goBtn = GameObject.FindWithTag("GenerateButton");
		if (goBtn != null) generateButton = goBtn.GetComponent<Button>();

		if (volumeInputField == null || generateButton == null)
		{
			Debug.LogError("CombinedCupGuide: UI 컴포넌트를 찾지 못했습니다. 태그를 확인하세요.");
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
			Debug.LogError($"CombinedCupGuide: 잘못된 숫자 형식 - '{volumeInputField.text}'");
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

		foreach (var mr in GetComponentsInChildren<MeshRenderer>())
			mr.enabled = false;
		// 1) 실세계 부피(m³)
		float worldV = Mathf.Max(0f, targetVolume_ml * 1e-6f);

		// 2) 로컬 스케일로 인한 부피 배율
		Vector3 s = transform.localScale;
		float scaleVolume = s.x * s.y * s.z;

		// 3) 로컬 좌표계에서의 목표 부피
		float localTargetV = worldV / scaleVolume;

		// 4) 누적 프로파일 최대값 클램프
		float maxLocalV = cumVolumes.Last();
		if (localTargetV > maxLocalV) localTargetV = maxLocalV;

		// 5) 로컬부피 배열에서 인덱스 찾기
		int idx = Array.FindIndex(cumVolumes, v => v >= localTargetV);
		idx = Mathf.Clamp(idx, 1, sampleCount - 1);

		// 6) 선형 보간해 정확한 hLocal 계산
		float v0 = cumVolumes[idx - 1], v1 = cumVolumes[idx];
		float t = (v1 > v0) ? (localTargetV - v0) / (v1 - v0) : 0f;
		float hLocal = Mathf.Lerp(heights[idx - 1], heights[idx], t);

		// 7) 로컬 hLocal 그대로 가이드라인 그리기
		DrawSectionHull(hLocal);


		Debug.Log($"GenerateGuide called with ml = {targetVolume_ml}");
		Debug.Log($"Drawing guide at height: {hLocal}");

	}

	void DrawSectionHull(float hLocal)
	{
		var mesh = meshFilter.sharedMesh;
		var hull2D = ComputeCrossSectionHull(mesh, hLocal);
		if (hull2D.Count < 3)
		{
			Debug.LogWarning($"CombinedCupGuide: 단면 포인트 부족(h={hLocal}): {hull2D.Count}");
			return;
		}

		if (circleGuidelineObj != null)
			Destroy(circleGuidelineObj);

		// 새 GameObject 생성
		circleGuidelineObj = new GameObject("CircleGuideline");

		// ★ 여기서 머그(this) 오브젝트의 자식으로 등록!  
		//    worldPositionStays = false 로 해야 로컬 좌표 그대로 붙습니다.
		circleGuidelineObj.transform.SetParent(this.transform, false);

		// LineRenderer 세팅 (로컬 공간)
		var lr = circleGuidelineObj.AddComponent<LineRenderer>();
		lr.useWorldSpace = false;
		lr.loop = true;
		lr.positionCount = hull2D.Count;
		lr.startWidth = guidelineWidth;
		lr.endWidth = guidelineWidth;
		lr.material = guidelineMat;

		// 로컬 좌표 그대로 찍어 주면,
		// 머그 오브젝트의 위치/회전/스케일이 자동으로 반영됩니다.
		for (int i = 0; i < hull2D.Count; i++)
		{
			Vector2 p = hull2D[i];
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
