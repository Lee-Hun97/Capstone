using System.Collections;
using System.Collections.Generic;
using Dummiesman;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class AppData : Singlton<AppData>
{
    //��� �����͸� AppData�� ������ �־ ������ ���ϰ� �ٸ� ������ ���ʿ��� �����͸� �������ʵ��� ����
    //������ ��� �����͸� �̰��� ����ش�.
    private string serverURL = "http://<����IP>:5000/upload";//���� �̸����� ���� �ʿ�
    private string captureImageFolderPath = @"C:/Work/CapturedImages";//�� ��ε� ������ ���� �� ����
    private string scriptPath = @"C:/Work/myscript.rcscript";//�ڵ����� ��� ��� ����

    //�߿� �������̱⿡ �б� ��������, ������ �ʿ��� ���� set�� ����
    public string ServerURL { get { return serverURL; } }
    public string CaptureImageFolderPath { get { return captureImageFolderPath; } }
    public string ScriptPath { get { return scriptPath; } }

    public string ID;
    public string PW;
    public GameObject[] GameObjects = new GameObject[3];//����ڿ��� ������ 3���� �� ���� ���

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("AppData destroyed");
    }

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("AppData Initialized");
    }

    public void SetInfo(string id, string pw)
    {
        ID = id;
        PW = pw;
    }

    public void GetInfo()
    {
        Debug.Log($"{ID}, {PW}");
    }

    private void SetModelData(GameObject[] gameObjects)//������� ���嵥���͸� �ҷ���
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObjects[i] = gameObject;
        }
    }

    public string[] modelUrls = new string[3]; // ������ �ִ� .obj ��ε�
    public GameObject[] loadedModels = new GameObject[3]; // �޾ƿ� GameObjects

    //IEnumerator LoadModelFromServer(string url, int index)
    //{
    //    UnityWebRequest request = UnityWebRequest.Get(url);
    //    yield return request.SendWebRequest();

    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        string objData = request.downloadHandler.text;

    //        // �� �Ʒ��� ����ϴ� ���̺귯���� �°� �ٲ�� ��
    //        string tempPath = Application.persistentDataPath + "/tempModel_" + index + ".obj";
    //        System.IO.File.WriteAllText(tempPath, objData);

    //        // OBJImporter�� ����Ͽ� ���� �ε�
    //        GameObject model = LoadOBJ(tempPath);
    //        model.name = "Model_" + index;
    //        model.transform.position = new Vector3(index * 2f, 0, 0); // �ӽ� ��ġ

    //        loadedModels[index] = model;
    //    }
    //    else
    //    {
    //        Debug.LogError($"�� {index} �ٿ�ε� ����: {request.error}");
    //    }
    //}
}
