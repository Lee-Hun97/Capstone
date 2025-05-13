using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAdjustment : MonoBehaviour
{
    [SerializeField]private Camera mainCamera; // ����, �ƿ��� ī�޶�
    public Transform targetObject; // ȸ���� ������Ʈ
    public float rotationSpeed = 0.2f;
    public float zoomSpeed = 1f;
    public float minZoomDis = -1f;
    public float maxZoomDis = -100f;

    private Vector2 prevTouchPos0, prevTouchPos1;

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                targetObject.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // ���� �������� ��ġ ����Ʈ �Ÿ�
            float currentDist = Vector2.Distance(touch0.position, touch1.position);
            // ���� �������� ��ġ ����Ʈ �Ÿ�
            float prevDist = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);

            float deltaDist = currentDist - prevDist;

            // Ȯ��/���
            float newScale = mainCamera.transform.position.z + deltaDist * zoomSpeed;
            newScale = Mathf.Clamp(newScale, minZoomDis, maxZoomDis);
            mainCamera.transform.position = new Vector3(0, 0, newScale);
        }
    }
}
