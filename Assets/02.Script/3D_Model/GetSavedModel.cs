using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using System.Reflection;

public class GetSavedModel : MonoBehaviour
{
    public GameObject ErrorPopUPObject;

    public Button[] Buttons = new Button[3];

    private void Start()
    {

        for (int i = 0; i < 3; i++)
        {
            string fileName = $"model{i}.bundle";
            string modelsSavePath = Path.Combine(AppData.Instance.User3DModelPath, fileName);

            if (!File.Exists(modelsSavePath))
            {
                Buttons[i].interactable = false;
            }
        }
    }
    public void GetIndexModel(int index)
    {
        string fileName = $"model{index}.bundle";
        string modelsSavePath = Path.Combine(AppData.Instance.User3DModelPath, fileName);
        string forLoadPath = Path.Combine(Application.persistentDataPath, AppData.Instance.bundleName);//���� AppData�� ���� �ؾ��ϴ°�? -> �̸��� �˾ƾ� �ٸ� ���Ͽ��� ��� ������� ���� ����

        if (File.Exists(modelsSavePath))
        {
            File.Copy(modelsSavePath, forLoadPath, overwrite: true);
            Debug.Log($"������ �����߽��ϴ�: {modelsSavePath} �� {forLoadPath}");
            Debug.Log(File.Exists(modelsSavePath));
            Debug.Log(File.Exists(forLoadPath));

            AppSceneManger.Instance.ChangeScene(Scene_name.MRScene);//������ ������ �� ������ �����ϰ� �̵�
        }
        else
        {
            Debug.LogError($"���� ������ �������� �ʽ��ϴ�: {modelsSavePath}");
            Show();
        }
    }

    public void Show(float duration = 1f)
    {
        ErrorPopUPObject.SetActive(true);
        CancelInvoke(); // �ߺ� ����
        Invoke(nameof(Hide), duration);
    }

    void Hide()
    {
        ErrorPopUPObject.SetActive(false);
    }
}
