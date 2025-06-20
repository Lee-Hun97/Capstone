using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

[RequireComponent(typeof(ARPlane))]
[RequireComponent(typeof(LineRenderer))]
public class ARPlaneOutline : MonoBehaviour
{
	ARPlane arPlane;
	LineRenderer lineRenderer;
	List<Vector3> points = new List<Vector3>();

	void Awake()
	{
		arPlane = GetComponent<ARPlane>();
		lineRenderer = GetComponent<LineRenderer>();
		// 반복 그릴 수 있도록 루프 설정
		lineRenderer.loop = true;
	}

	void OnEnable()
	{
		arPlane.boundaryChanged += OnBoundaryChanged;
	}

	void OnDisable()
	{
		arPlane.boundaryChanged -= OnBoundaryChanged;
	}

	void OnBoundaryChanged(ARPlaneBoundaryChangedEventArgs args)
	{
		points.Clear();
		// ARPlane.boundary: 2D 좌표 리스트
		foreach (Vector2 p in args.plane.boundary)
		{
			// 로컬 XY 평면 상의 X,Z 로 변환
			points.Add(new Vector3(p.x, 0f, p.y));
		}
		// LineRenderer에 반영
		lineRenderer.positionCount = points.Count;
		lineRenderer.SetPositions(points.ToArray());
	}
}
