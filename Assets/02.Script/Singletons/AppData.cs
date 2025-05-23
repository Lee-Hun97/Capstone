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
    //모든 데이터를 AppData가 가지고 있어서 변경이 편리하고 다른 곳에서 불필요한 데이터를 가지지않도록 구성
    //가능한 모든 데이터를 이곳에 모아준다.
    private string serverURL = "http://172.30.1.39:5000";//서버에 따라서 해당 값을 바꿔줘야한다. Flask서버를 열때 url을 체크***

    private string captureImageFolderPath = Path.Combine(Application.temporaryCachePath, "CapturedImages");
    private string user3DModelPath = Path.Combine(Application.persistentDataPath, "User3DModels");//모델의 경우 재사용을 해야하기에 삭제의 위험이 적은 persistentpath 사용

    //중요 데이터이기에 읽기 전용으로, 변경이 필요한 값은 set도 설정
    public string ServerURL { get { return serverURL; } }
    public string ServerLoginURL { get { return serverURL + "/login"; } }//json 데이터를 전달
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
    public string PW;//전부 private으로 변경필요

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
        user_id = email;//나중에 서버 쪽과 이야기해서 userid로 변경
        EMAIL = email;
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
        int i = 0;
        string url = $"{ServerGetSavedModelsTimestampURL}?user_id={user_id}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log("받은 JSON: " + json);

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
                Debug.LogError("서버 요청 실패: " + request.error);
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
            request.downloadHandler = new DownloadHandlerBuffer();//방어 코드?
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                //string objData = request.downloadHandler.text;
                byte[] data = request.downloadHandler.data;

                // 저장
                string savePath = Path.Combine(user3DModelPath, fileName);//번들로 저장 형태가 변경되었기에
                //System.IO.File.WriteAllText(savePath, objData);
                System.IO.File.WriteAllBytes(savePath, data);

                //경로에 잘 저장되었는지 확인
                if (File.Exists(savePath)) Debug.Log($"{savePath}에 {index}모델 저장 완료, size: {new FileInfo(savePath).Length} bytes");
                else Debug.Log($"저장 중 문제 발생.(LoadModelFromServer함수)  > {savePath} / {fileName}");
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
