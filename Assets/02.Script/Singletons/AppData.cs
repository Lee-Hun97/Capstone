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
    private string serverURL = "http://172.19.31.77:5000";
    private string serverLoginURL = "http://172.19.31.77:5000/login";
    private string serverModelsTimeStampURL = "http://172.19.31.77:5000/get_saved_models"; //{ id }
    private string serverModelGetURL = "http://127.0.0.1:5000/get_model"; //{ name, id }
    private string serverImageSaveURL = "http://127.0.0.1:5000/upload"; ////{ id, filepath }
    private string createURL = "http://127.0.0.1:5000/latest_upload"; //{ id }
    private string serverModelSaveURL = "http://127.0.0.1:5000/save_model"; //{ name, id }

    private string captureImageFolderPath = Path.Combine(Application.temporaryCachePath, "CapturedImages");
    private string user3DModelPath = Path.Combine(Application.persistentDataPath, "User3DModels");//���� ��� ������ �ؾ��ϱ⿡ ������ ������ ���� persistentpath ���

    //�߿� �������̱⿡ �б� ��������, ������ �ʿ��� ���� set�� ����
    public string ServerURL { get { return serverURL; } }
    public string ServerLoginURL { get { return serverLoginURL; } }

    public string ServerImageSaveURL { get { return captureImageFolderPath; } }
    public string ServerModelGetURL { get { return user3DModelPath; } }
    public string ServerModelSaveURL {  get { return serverModelSaveURL; } }

    public string ID;
    public string PW;

    private string[] timeStamps = new string[3];

    public loadedModelsInfo[] C_loadedmodelsInfo = new loadedModelsInfo[3];

    [System.Serializable]
    public class loadedModelsInfo
    {
        public GameObject gameObject = new GameObject();
        public string path = "";
    }

    [System.Serializable]
    public class ResultResponse
    {
        public string status;
        public string[] result;  // Ȥ�� List<string>
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

    public void SetInfo(string id, string pw)
    {
        ID = id;
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
        WWWForm form = new WWWForm();
        form.AddField("user_id", ID);
        int i = 0;

        using (UnityWebRequest request = UnityWebRequest.Post(serverModelsTimeStampURL, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("���� JSON: " + json);

                ResultResponse response = JsonUtility.FromJson<ResultResponse>(json);
                
                foreach (string timestamp in response.result)
                {
                    timeStamps[i] = timestamp;
                }
            }
            else
            {
                Debug.LogError("���� ��û ����: " + request.error);
            }
        }

        for (int n = 0; n < 3; n++)
        {
            StartCoroutine(LoadModelFromServer(serverModelGetURL, n));
        }
    }

    IEnumerator LoadModelFromServer(string serverModelUrl, int index)
    {
        string timestamp = timeStamps[index];
        string url = $"{serverModelUrl}?user_id={ID}&timestamp={timestamp}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string objData = request.downloadHandler.text;

                // ����
                string savePath = Path.Combine(Application.temporaryCachePath, index + ".obj");
                System.IO.File.WriteAllText(savePath, objData);

                // �Ľ�
                OBJLoader loader = new OBJLoader();

                C_loadedmodelsInfo[index].gameObject = loader.Load(savePath);
                C_loadedmodelsInfo[index].path = savePath;
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
