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
    private string serverURL = "http://172.19.3.57:5000";//������ ���� �ش� ���� �ٲ�����Ѵ�. Flask������ ���� url�� üũ***

    private string captureImageFolderPath = Path.Combine(Application.temporaryCachePath, "CapturedImages");
    private string user3DModelPath = Path.Combine(Application.persistentDataPath, "User3DModels");//���� ��� ������ �ؾ��ϱ⿡ ������ ������ ���� persistentpath ���

    //�߿� �������̱⿡ �б� ��������, ������ �ʿ��� ���� set�� ����
    public string ServerURL { get { return serverURL; } }
    public string ServerLoginURL { get { return serverURL + "/login" ; } }//json �����͸� ����
    public string ServerImageUploadURL { get { return serverURL + "/upload"; } }//{ id, filepath }
    public string ServerModelGetbyNameURL { get { return serverURL + "/get_model_by_name"; } }//{ name, id }
    public string ServerModelURL { get { return serverURL + "/get_model"; } }//{ name, id }
    public string ServerModelSaveURL {  get { return serverURL + "/save_model"; } }//{ name, id }
    public string GetTimeStampURL {  get { return serverURL + "/latest_upload"; } }//{ id }
    public string ServerGetSavedModelsTimestampURL {  get { return serverURL + "/get_saved_models"; } }//{ id
    public string RunRCURL { get { return serverURL + "/process_rc"; } }//{ id, timestamp }
    public string CaptureImageFolderPath { get { return captureImageFolderPath; } }
    public string User3DModelPath { get { return user3DModelPath; } }

    private string user_id = "";
    public string EMAIL;
    public string PW;//���� private���� �����ʿ�

    private string[] timeStamps = new string[3];
    public string CurentTimeStamp = "";

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
        public string created_at;
        public string name;
        public string timestamp;
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
        user_id = userid;
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
                    timeStamps[i] = item.timestamp;
                    i++;
                }
            }
            else
            {
                Debug.LogError("���� ��û ����: " + request.error);
            }
        }


        for (int n = 0; n < 3; n++)
        {
            yield return new WaitForSeconds(5f);
            StartCoroutine(LoadModelFromServer(ServerModelURL, n));
        }
    }

    IEnumerator LoadModelFromServer(string serverModelUrl, int index)
    {
        string timestamp = timeStamps[index];
        string url = $"{serverModelUrl}?user_id={user_id}&timestamp={timestamp}";

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
