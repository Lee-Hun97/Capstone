using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class AppData : Singlton<AppData>
{
    //��� �����͸� AppData�� ������ �־ ������ ���ϰ� �ٸ� ������ ���ʿ��� �����͸� �������ʵ��� ����
    //������ ��� �����͸� �̰��� ����ش�.
    private string serverURL = "http://172.30.1.39:5000";//������ ���� �ش� ���� �ٲ�����Ѵ�. Flask������ ���� url�� üũ***

    private string captureImageFolderPath = Path.Combine(Application.temporaryCachePath, "CapturedImages");
    private string user3DModelPath = Path.Combine(Application.persistentDataPath, "User3DModels");//���� ��� ������ �ؾ��ϱ⿡ ������ ������ ���� persistentpath ���

    //�߿� �������̱⿡ �б� ��������, ������ �ʿ��� ���� set�� ����
    public string ServerURL { get { return serverURL; } }
    public string ServerLoginURL { get { return serverURL + "/login"; } }//json �����͸� ����
    public string ServerImageUploadURL { get { return serverURL + "/upload"; } }//{ id, filepath }
    public string ServerModelGetbyNameURL { get { return serverURL + "/get_model_by_name"; } }//{ name, id }
    public string ServerModelURL { get { return serverURL + "/get_model"; } }//{ name, id }
    public string ServerModelSaveURL { get { return serverURL + "/save_model"; } }//{ name, id }
    public string GetTimeStampURL { get { return serverURL + "/latest_upload"; } }//{ id }
    public string ServerGetSavedModelsTimestampURL { get { return serverURL + "/get_saved_models"; } }//{ id
    public string RunRCURL { get { return serverURL + "/process_rc"; } }//{ id, timestamp }
    public string GetBundleUrl { get { return ServerURL + "/get_bundle";}} //{ id, timestamp }
    public string CaptureImageFolderPath { get { return captureImageFolderPath; } }
    public string User3DModelPath { get { return user3DModelPath; } }

    private string user_id = "";
    public string EMAIL;
    public string PW;//���� private���� �����ʿ�

    private string[] timeStamps = new string[3] { "-1", "-1", "-1" };
    public string CurentTimeStamp = "";

    public string bundleName { get {return "model.bundle"; } }

    public loadedModelsInfo[] C_loadedmodelsInfo = new loadedModelsInfo[3];

    [System.Serializable]
    public class loadedModelsInfo
    {
        public GameObject gameObject = new GameObject();
        public string path = "";
    }

    [System.Serializable]
    public class ResultItem
    {
        public string name;
        public string timestamp;
        public string created_at;
    }

    [System.Serializable]
    public class ResultResponse
    {
        public string status;
        public List<ResultItem> models;
    }

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("AppData Initialized");

        CheckFolders();
    }

    protected override void Start()
    {
        base.Start();
    }

    public string GetUserID()
    {
        return user_id;
    }

    private void CheckFolders()
    {
        if (!Directory.Exists(user3DModelPath))
        {
            Directory.CreateDirectory(user3DModelPath);
        }
        if (!Directory.Exists(captureImageFolderPath))
        {
            Directory.CreateDirectory(captureImageFolderPath);
        }
    }

    public void SetInfo(string email, string pw, string userid)
    {
        user_id = email;//���߿� ���� �ʰ� �̾߱��ؼ� userid�� ����
        EMAIL = email;
        PW = pw;

        StartCoroutine(GetModelsTimestampFromFlask());
    }

    //private void SetModelData(GameObject[] gameObjects)//������� ���嵥���͸� �ҷ���
    //{
    //    for (int i = 0; i < gameObjects.Length; i++)
    //    {
    //        GameObjects[i] = gameObject;
    //    }
    //}

    IEnumerator GetModelsTimestampFromFlask()
    {
        int i = 0;
        string url = $"{ServerGetSavedModelsTimestampURL}?user_id={user_id}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("���� JSON: " + json);

                ResultResponse results = JsonConvert.DeserializeObject<ResultResponse>(json);
                foreach (var item in results.models)
                {
                    if (item != null || item.timestamp != null || item.timestamp != "") timeStamps[i] = item.timestamp;
                    else timeStamps[i] = "-1";

                    Debug.Log($"{i} {timeStamps[i]}");
                    i++;
                }
            }
            else
            {
                Debug.LogError("���� ��û ����: " + request.error);
            }
        }


        for (int n = 0; n < timeStamps.Length; n++)
        {
            if (timeStamps[n] != "-1")
            {
                yield return new WaitForSeconds(1f);
                StartCoroutine(LoadModelFromServer(GetBundleUrl, n));
            }
        }
    }

    IEnumerator LoadModelFromServer(string serverModelUrl, int index)
    {
        string timestamp = timeStamps[index];
        if (timestamp == "-1") yield break;

        string url = $"{serverModelUrl}?user_id={user_id}&timestamp={timestamp}";
        string fileName = $"model{index}.bundle";

        Debug.Log($"{timestamp} {url} {fileName}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();//��� �ڵ�?
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                //string objData = request.downloadHandler.text;
                byte[] data = request.downloadHandler.data;

                // ����
                string savePath = Path.Combine(user3DModelPath, fileName);//����� ���� ���°� ����Ǿ��⿡
                //System.IO.File.WriteAllText(savePath, objData);
                System.IO.File.WriteAllBytes(savePath, data);

                //��ο� �� ����Ǿ����� Ȯ��
                if (File.Exists(savePath)) Debug.Log($"{savePath}�� {index}�� ���� �Ϸ�, size: {new FileInfo(savePath).Length} bytes");
                else Debug.Log($"���� �� ���� �߻�.(LoadModelFromServer�Լ�)  > {savePath} / {fileName}");
            }
            else
            {
                Debug.LogError("�������� �� �ٿ�ε� ����: " + request.error);
            }
        }
    }


    void OnApplicationQuit()//������ ���� ����Ȯ��
    {
        if (Directory.Exists(captureImageFolderPath))
        {
            Directory.Delete(captureImageFolderPath, true);
        }
        if (Directory.Exists(user3DModelPath))
        {
            Directory.Delete(user3DModelPath, true);
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Debug.Log("AppData destroyed");
    }
}
