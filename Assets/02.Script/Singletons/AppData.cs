using System.Collections;
using System.Collections.Generic;
using System.IO;
using Dummiesman;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class AppData : Singlton<AppData>
{
    //모든 데이터를 AppData가 가지고 있어서 변경이 편리하고 다른 곳에서 불필요한 데이터를 가지지않도록 구성
    //가능한 모든 데이터를 이곳에 모아준다.
    private string serverURL = "http://172.19.31.77:5000";
    private string serverLoginURL = "http://172.19.31.77:5000/login";
    private string serverModelsTimeStampURL = "http://172.19.31.77:5000/get_saved_models"; //{ id }
    private string serverModelGetURL = "http://127.0.0.1:5000/get_model"; //{ name, id }
    private string serverImageSaveURL = "http://127.0.0.1:5000/upload"; ////{ id, filepath }
    private string createURL = "http://127.0.0.1:5000/latest_upload"; //{ id }
    private string serverModelSaveURL = "http://127.0.0.1:5000/save_model"; //{ name, id }

    private string captureImageFolderPath = Path.Combine(Application.temporaryCachePath, "CapturedImages");
    private string user3DModelPath = Path.Combine(Application.persistentDataPath, "User3DModels");//모델의 경우 재사용을 해야하기에 삭제의 위험이 적은 persistentpath 사용

    //중요 데이터이기에 읽기 전용으로, 변경이 필요한 값은 set도 설정
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
        public string[] result;  // 혹은 List<string>
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

    //private void SetModelData(GameObject[] gameObjects)//사용자의 저장데이터를 불러옴
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
                Debug.Log("받은 JSON: " + json);

                ResultResponse response = JsonUtility.FromJson<ResultResponse>(json);
                
                foreach (string timestamp in response.result)
                {
                    timeStamps[i] = timestamp;
                }
            }
            else
            {
                Debug.LogError("서버 요청 실패: " + request.error);
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

                // 저장
                string savePath = Path.Combine(Application.temporaryCachePath, index + ".obj");
                System.IO.File.WriteAllText(savePath, objData);

                // 파싱
                OBJLoader loader = new OBJLoader();

                C_loadedmodelsInfo[index].gameObject = loader.Load(savePath);
                C_loadedmodelsInfo[index].path = savePath;
            }
            else
            {
                Debug.LogError("서버에서 모델 다운로드 실패: " + request.error);
            }
        }
    }

    void OnApplicationQuit()//만약을 위한 삭제확인
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
