using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAdjustment : MonoBehaviour
{
    [SerializeField]private Camera mainCamera; // ����, �ƿ��� ī�޶�
    public Transform targetObject; // ȸ���� ������Ʈ
    public float rotationSpeed = 0.2f;
    public float zoomSpeed = 0.1f;
    public float minZoomDis = -100f;
    public float maxZoomDis = -1f;

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

            // ���� ������ �Ÿ�
            float currentDist = Vector2.Distance(touch0.position, touch1.position);

            // ���� ������ �Ÿ�
            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;
            float prevDist = Vector2.Distance(prevTouch0, prevTouch1);

            // ��ȭ�� ���
            float delta = currentDist - prevDist;
            delta = Mathf.Clamp(delta, -2f, 2f);

            Vector3 cameraPos = mainCamera.transform.position;
            cameraPos.z += -delta * zoomSpeed;
            Debug.Log(cameraPos.z);

            // Clamp: Ȯ��/��� ���� ����
            cameraPos.z = Mathf.Clamp(cameraPos.z, minZoomDis, maxZoomDis);
            mainCamera.transform.position = cameraPos;

        }
    }
}
