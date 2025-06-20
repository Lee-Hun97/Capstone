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
        string forLoadPath = Path.Combine(Application.persistentDataPath, AppData.Instance.bundleName);//굳이 AppData를 참조 해야하는가? -> 이름을 알아야 다른 파일에서 어떻게 저장될지 예측 가능

        if (File.Exists(modelsSavePath))
        {
            File.Copy(modelsSavePath, forLoadPath, overwrite: true);
            Debug.Log($"번들을 복사했습니다: {modelsSavePath} → {forLoadPath}");
            Debug.Log(File.Exists(modelsSavePath));
            Debug.Log(File.Exists(forLoadPath));

            AppSceneManger.Instance.ChangeScene(Scene_name.MRScene);//번들이 존재할 시 번들을 복사하고 이동
        }
        else
        {
            Debug.LogError($"번들 파일이 존재하지 않습니다: {modelsSavePath}");
            Show();
        }
    }

    public void Show(float duration = 1f)
    {
        ErrorPopUPObject.SetActive(true);
        CancelInvoke(); // 중복 방지
        Invoke(nameof(Hide), duration);
    }

    void Hide()
    {
        ErrorPopUPObject.SetActive(false);
    }
}
