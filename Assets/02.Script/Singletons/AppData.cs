using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class AppData : Singlton<AppData>
{
    //��� �����͸� AppData�� ������ �־ ������ ���ϰ� �ٸ� ������ ���ʿ��� �����͸� �������ʵ��� ����
    //������ ��� �����͸� �̰��� ����ش�.
    private string serverURL = "http://<����IP>:5000/upload";//���� �̸����� ���� �ʿ�
    private string serverModelUrl = ""; // ������ �ִ� .obj ���
    private string captureImageFolderPath = Path.Combine(Application.temporaryCachePath, "CapturedImage");
    private string user3DModelPath = Path.Combine(Application.persistentDataPath, "User3DModels");//���� ��� ������ �ؾ��ϱ⿡ ������ ������ ���� persistentpath ���

    //�߿� �������̱⿡ �б� ��������, ������ �ʿ��� ���� set�� ����
    public string ServerURL { get { return serverURL; } }
    public string CaptureImageFolderPath { get { return captureImageFolderPath; } }
    public string User3DModelPath { get { return user3DModelPath; } }

    public string ID;
    public string PW;
    public GameObject[] GameObjects = new GameObject[3];//����ڿ��� ������ 3���� �� ���� ���
    public GameObject[] loadedModels = new GameObject[3]; // �޾ƿ� GameObjects

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("AppData Initialized");
    }

    protected override void Start()
    {
        base.Start();
    }

    public void SetInfo(string id, string pw)
    {
        ID = id;
        PW = pw;

        for(int i =0;i<3; i++)
        {
            StartCoroutine(LoadModelFromServer(serverModelUrl, i));
        }
    }

    //private void SetModelData(GameObject[] gameObjects)//������� ���嵥���͸� �ҷ���
    //{
    //    for (int i = 0; i < gameObjects.Length; i++)
    //    {
    //        GameObjects[i] = gameObject;
    //    }
    //}

    IEnumerator LoadModelFromServer(string url, int index)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string objData = request.downloadHandler.text;

            string tempPath = user3DModelPath + "/tempModel_" + index + ".obj";
            System.IO.File.WriteAllText(tempPath, objData);
        }
        else
        {
            Debug.LogError($"�� {index} �ٿ�ε� ����: {request.error}");
        }
    }

    void OnApplicationQuit()
    {
        if (Directory.Exists(captureImageFolderPath))
        {
            Directory.Delete(captureImageFolderPath, true);
            //Debug.Log("�ӽ� �̹��� ���� ����");
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("AppData destroyed");
    }
}
