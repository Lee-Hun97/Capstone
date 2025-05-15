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
		// �ݺ� �׸� �� �ֵ��� ���� ����
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
		// ARPlane.boundary: 2D ��ǥ ����Ʈ
		foreach (Vector2 p in args.plane.boundary)
		{
			// ���� XY ��� ���� X,Z �� ��ȯ
			points.Add(new Vector3(p.x, 0f, p.y));
		}
		// LineRenderer�� �ݿ�
		lineRenderer.positionCount = points.Count;
		lineRenderer.SetPositions(points.ToArray());
	}
}
