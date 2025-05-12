using System.Collections;
using System.Collections.Generic;
using Dummiesman;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class AppData : Singlton<AppData>
{
    //모든 데이터를 AppData가 가지고 있어서 변경이 편리하고 다른 곳에서 불필요한 데이터를 가지지않도록 구성
    //가능한 모든 데이터를 이곳에 모아준다.
    private string serverURL = "http://<서버IP>:5000/upload";//서버 이름으로 변경 필요
    private string captureImageFolderPath = @"C:/Work/CapturedImages";//이 경로도 앱으로 제작 시 변경
    private string scriptPath = @"C:/Work/myscript.rcscript";//핸드폰인 경우 경로 변경

    //중요 데이터이기에 읽기 전용으로, 변경이 필요한 값은 set도 설정
    public string ServerURL { get { return serverURL; } }
    public string CaptureImageFolderPath { get { return captureImageFolderPath; } }
    public string ScriptPath { get { return scriptPath; } }

    public string ID;
    public string PW;
    public GameObject[] GameObjects = new GameObject[3];//사용자에게 제공할 3개의 모델 저장 기능

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

    private void SetModelData(GameObject[] gameObjects)//사용자의 저장데이터를 불러옴
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObjects[i] = gameObject;
        }
    }

    public string[] modelUrls = new string[3]; // 서버에 있는 .obj 경로들
    public GameObject[] loadedModels = new GameObject[3]; // 받아온 GameObjects

    //IEnumerator LoadModelFromServer(string url, int index)
    //{
    //    UnityWebRequest request = UnityWebRequest.Get(url);
    //    yield return request.SendWebRequest();

    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        string objData = request.downloadHandler.text;

    //        // ↓ 아래는 사용하는 라이브러리에 맞게 바꿔야 함
    //        string tempPath = Application.persistentDataPath + "/tempModel_" + index + ".obj";
    //        System.IO.File.WriteAllText(tempPath, objData);

    //        // OBJImporter를 사용하여 모델을 로드
    //        GameObject model = LoadOBJ(tempPath);
    //        model.name = "Model_" + index;
    //        model.transform.position = new Vector3(index * 2f, 0, 0); // 임시 위치

    //        loadedModels[index] = model;
    //    }
    //    else
    //    {
    //        Debug.LogError($"모델 {index} 다운로드 실패: {request.error}");
    //    }
    //}
}
